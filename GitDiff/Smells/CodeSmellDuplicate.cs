using GitDiff.Syntax;
using Markdig.Syntax.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GitDiff.Smells
{
    public class CodeSmellDuplicate : CodeSmell
    {
        private static readonly int MinNumOfCharacters = 25;

        public CodeSmellDuplicate()
            : base(CodeSmellSeverity.Warning)
        { }

        public override string Title => "Duplicate Lines";

        public override string Description => "Rendundant code can be a nightmare to maintain, try to reduce the number of duplicated lines of code";

        public override string Suggestion => "Refactor code, like pull-up, to remove duplicate lines of code";

        public override CodeSmellResult Analyze(DiffInfoCommit diffInfoCommit)
        {
            CodeSmellResult codeSmellResult = new CodeSmellResult(this, diffInfoCommit);

            ResetMatch();

            foreach (DiffInfoFile diffInfoFile in diffInfoCommit.DiffFile)
            {
                foreach (DiffInfoLine diffInfoLine in diffInfoFile.NewLines)
                {
                    FindMatch(diffInfoLine, codeSmellResult);
                }
            }

            return codeSmellResult;
        }

        private void FindMatch(DiffInfoLine diffInfoLine, CodeSmellResult codeSmellResult)
        {
            if (diffInfoLine.Line.Trim().Length < MinNumOfCharacters) return;

            if (GetHash(diffInfoLine.Line, out object key))
            {
                if (!DuplicateLines.TryGetValue(key, out DiffInfoLine match))
                {
                    DuplicateLines.Add(key, diffInfoLine);
                }
                else
                {
                    if (!DuplicateSmellInfo.TryGetValue(key, out SmellInfo smellInfo))
                    {
                        smellInfo = new SmellInfo(match, diffInfoLine);
                        DuplicateSmellInfo.Add(key, smellInfo);

                        codeSmellResult.AddSmell(smellInfo);
                    }
                    else
                    {
                        smellInfo.AddDiffLine(diffInfoLine);
                    }
                }
            }
        }

        private bool GetHash(string line, out object key)
        {
            key = null;

            // Removes all of the C# keywords
            foreach (string blacklistedString in BlacklistKeywords)
            {
                if (line.Contains(blacklistedString)) return false;
            }

            line = line.Trim();

            if (line.IndexOf(';') < 1) return false;
            if (!line.Any(l => char.IsLetter(l))) return false;

            line = line.Substring(0, line.IndexOf(';')).Trim();

            key = line.GetHashCode();
            return true;
        }
        private readonly Dictionary<object, DiffInfoLine> DuplicateLines = new();
        private readonly Dictionary<object, SmellInfo> DuplicateSmellInfo = new();

        private void ResetMatch()
        {
            DuplicateLines.Clear();
            DuplicateSmellInfo.Clear();
        }

        public override void ToString(StringBuilder sb, SmellInfo smellInfo)
        {
            if (smellInfo.LineCount == 0) return;

            DiffInfoLine longestLine = smellInfo.DiffLines.OrderByDescending(s => s.DiffFile.FileName.Length + s.LineNumber.ToString().Length).First();
            int maxLen = longestLine.DiffFile.FileName.Length + longestLine.LineNumber.ToString().Length;

            foreach (DiffInfoLine diffInfoLine in smellInfo.DiffLines)
            {
                sb.AppendLine("[" + diffInfoLine.DiffFile.FileName + " @ " + diffInfoLine.LineNumber + "]: " + new string(' ', (maxLen - (diffInfoLine.DiffFile.FileName.Length + diffInfoLine.LineNumber.ToString().Length))) + diffInfoLine.Line.Trim());
            }

            sb.AppendLine();
        }

        private static readonly List<string> BlacklistKeywords = new List<string>() {
            "bool", "byte", "sbyte", "short", "ushort", "int", "uint", "long", "ulong", "double", "float", "decimal",
            "string", "char", "void", "object", "typeof", "sizeof", "null", "true", "false", "if", "else", "while", "for", "foreach", "do", "switch",
            "case", "default", "lock", "try", "throw", "catch", "finally", "goto", "break", "continue", "return", "public", "private", "internal",
            "protected", "static", "readonly", "get", "set", "sealed", "const", "fixed", "stackalloc", "volatile", "new", "override", "abstract", "virtual",
            "event", "extern", "ref", "out", "in", "is", "as", "params", "__arglist", "__makeref", "__reftype", "__refvalue", "this", "base",
            "namespace", "using", "class", "struct", "interface", "enum", "delegate", "checked", "unchecked", "unsafe", "operator", "implicit", "explicit"
        };
    }
}