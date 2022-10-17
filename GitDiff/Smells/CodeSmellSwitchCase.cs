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

        public override string Description => "Most software implementations should not require the use of a switch-case statement.";

        public override string Suggestion => "Attempt to replace switch-case statement with if's and else if's.";

        private static Regex RegexSwitchCase
        { get; } = new("^\\s*(switch)\\s*\\(([^\\)]*)\\)");

        public override CodeSmellResult Analyze(DiffInfoCommit diffInfoCommit)
        {
            DiffInfoLine tempInfoLine;
            int idx, lineNumber, braces;

            CodeSmellResult codeSmellResult = new(this);

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
                                if (idx >= diffInfoFile.NewLinesCount) break;
                                tempInfoLine = diffInfoFile.NewLines[idx];

                                if (tempInfoLine.Line.Contains('{') && (braces == 0)) braces += 1;

                                while (braces > 0)
                                {
                                    if (tempInfoLine.LineNumber != (lineNumber + 1)) break;
                                    lineNumber = tempInfoLine.LineNumber;

                                    if (tempInfoLine.Line.Contains('}')) braces -= 1;

                                    lines.Add(tempInfoLine);

                                    idx += 1;
                                    if (idx >= diffInfoFile.NewLinesCount) break;
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
    }
}