using GitDiff.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GitDiff.Smells
{
    public class CodeSmellSwitchCase : CodeSmell
    {
        public CodeSmellSwitchCase()
            : base(CodeSmellSeverity.Warning)
        { }

        public override string Title => "Switch-Case";

        public override string Description => "Most software implementations should not require the use of a switch-case statement";

        public override string Suggestion => "Attempt to replace switch-case statements with if's and else if's";

        private static Regex RegexSwitchCase
        { get; } = new Regex("^\\s*(switch)\\s*\\(([^\\)]*)\\)");

        public override CodeSmellResult Analyze(DiffInfoCommit diffInfoCommit)
        {
            DiffInfoLine tempInfoLine;
            int idx, lineNumber, braces;

            CodeSmellResult codeSmellResult = new CodeSmellResult(this, diffInfoCommit);

            foreach (DiffInfoFile diffInfoFile in diffInfoCommit.DiffFile)
            {
                foreach (DiffInfoLine diffInfoLine in diffInfoFile.NewLines)
                {
                    if (RegexSwitchCase.IsMatch(diffInfoLine.Line))
                    {
                        List<DiffInfoLine> lines = new List<DiffInfoLine>() { diffInfoLine };

                        do
                        {
                            idx = diffInfoFile.NewLines.IndexOf(diffInfoLine);
                            if (idx >= 0)
                            {
                                lineNumber = diffInfoLine.LineNumber;
                                braces = 0;

                                tempInfoLine = diffInfoFile.NewLines[idx];

                                // Make this more robust, as of now the '{' must be in-line or on the next line of the switch
                                if (tempInfoLine.Line.Contains('{'))
                                {
                                    braces += 1;
                                }

                                idx += 1;
                                if (idx >= diffInfoFile.Count) break;
                                tempInfoLine = diffInfoFile.NewLines[idx];

                                if (tempInfoLine.Line.Contains('{') && (braces == 0)) braces += 1;

                                // Find the matching brace
                                while (braces > 0)
                                {
                                    if (tempInfoLine.LineNumber != (lineNumber + 1)) break;
                                    lineNumber = tempInfoLine.LineNumber;

                                    if (tempInfoLine.Line.Contains('}')) braces -= 1;

                                    lines.Add(tempInfoLine);

                                    idx += 1;
                                    if (idx >= diffInfoFile.Count) break;
                                    tempInfoLine = diffInfoFile.NewLines[idx];

                                    if (tempInfoLine.Line.Contains('{')) braces += 1;
                                }
                            }
                        } while (false);

                        codeSmellResult.AddSmell(new SmellInfo(lines));
                    }
                }
            }

            return codeSmellResult;
        }

        public override void ToString(StringBuilder sb, SmellInfo smellInfo)
        {
            int startLine, lineNumber, whitespaceIdx;
            List<string> linesToAdd = new List<string>();

            sb.AppendLine("File: " + smellInfo.FirstDiff.DiffFile.FileName);

            startLine = smellInfo.StartLineNumber;
            lineNumber = smellInfo.StartLineNumber - 1;

            whitespaceIdx = int.MaxValue;
            for (int i = 0; i < smellInfo.LineCount; i++)
            {
                int idx = 0;
                if (smellInfo.DiffLines[i].Line.Length > 0)
                {
                    while (smellInfo.DiffLines[i].Line[idx] == ' ')
                    {
                        idx += 1;
                        if (idx == smellInfo.DiffLines[i].Line.Length) break;
                    }
                }

                if (idx < whitespaceIdx)
                {
                    whitespaceIdx = idx;
                }
            }

            for (int i = 0; i < smellInfo.LineCount; i++)
            {
                DiffInfoLine diffInfoLine = smellInfo.DiffLines[i];

                linesToAdd.Add(diffInfoLine.Line.Substring(whitespaceIdx));

                if (startLine < 0)
                {
                    startLine = diffInfoLine.LineNumber;
                }

                if (((lineNumber + 1) != diffInfoLine.LineNumber) || (i == (smellInfo.LineCount - 1)))
                {
                    PrintLines(sb, startLine, diffInfoLine.LineNumber, linesToAdd);
                    startLine = -1;
                }

                lineNumber = diffInfoLine.LineNumber;
            }
        }

        private void PrintLines(StringBuilder sb, int startLine, int endLine, List<string> lines)
        {
            if (lines.Count == 0) return;

            sb.AppendLine("Line: " + startLine + ".." + endLine);

            foreach (string line in lines)
            {
                sb.AppendLine(line);
            }

            sb.AppendLine();
            lines.Clear();
        }
    }
}