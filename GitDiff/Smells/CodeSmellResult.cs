using GitDiff.Syntax;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDiff;
using Markdig.Helpers;

namespace GitDiff.Smells
{
    /// <summary>
    /// Data container for a smelly commit
    /// </summary>
    public class CodeSmellResult
    {
        public CodeSmellResult(CodeSmell codeSmell, DiffInfoCommit diffInfoCommit)
        {
            CodeSmell = codeSmell;
            Commit = diffInfoCommit;
        }

        public CodeSmell CodeSmell
        { get; }

        public DiffInfoCommit Commit
        { get; }

        public List<SmellInfo> CodeSmellList
        { get; } = new List<SmellInfo>();

        public int Counts
        { get { return CodeSmellList.Count; } }

        public void AddSmell(SmellInfo smellInfo)
        {
            CodeSmellList.Add(smellInfo);
        }

        /// <summary>
        /// Returns a user-friendly printable string of all the smelly code
        /// </summary>
        /// <returns></returns>
        public string PrintResult()
        {
            const int headerLen = 100;
            const char headerChar = '=';
            const int smellHeaderLen = 40;
            const char smellHeaderChar = '-';

            StringBuilder sb = new StringBuilder();

            sb.AppendLine(new string(headerChar, headerLen));
            sb.AppendLine(CenterString(Commit.CommitName.ToUpper(), headerLen));
            sb.AppendLine(CenterString(Commit.CommitID.ToUpper(), headerLen));
            sb.AppendLine(new string(headerChar, headerLen));
            sb.AppendLine();

            sb.AppendLine(new string(smellHeaderChar, smellHeaderLen));
            sb.AppendLine(CenterString(CodeSmell.Title.ToUpper(), smellHeaderLen));
            sb.AppendLine(new string(smellHeaderChar, smellHeaderLen));

            sb.AppendLine("Severity: " + CodeSmell.Severity.ToString());
            sb.AppendLine("Description: " + CodeSmell.Description);
            sb.AppendLine("Suggestion: " + CodeSmell.Suggestion);
            sb.AppendLine();

            foreach (SmellInfo smellInfo in CodeSmellList)
            {
                CodeSmell.Print(sb, smellInfo);
            }

            return sb.ToString();
        }

        private static string CenterString(string toCenter, int width)
        {
            if (toCenter.Length >= width) return toCenter;
            return new string(' ', (int)Math.Floor((width / 2.0) - (toCenter.Length / 2.0))) + toCenter;
        }
    }

    /// <summary>
    /// Data container for a smelly line of code
    /// </summary>
    public class SmellInfo
    {
        public SmellInfo(params DiffInfoLine[] diffLine)
        {
            if ((diffLine == null) || (diffLine.Length == 0)) throw new ArgumentException();

            DiffLines.AddRange(diffLine);
        }

        public SmellInfo(List<DiffInfoLine> diffLines)
        {
            if ((diffLines == null) || (diffLines.Count == 0)) throw new ArgumentException();

            DiffLines.AddRange(diffLines);
        }

        public List<DiffInfoLine> DiffLines
        { get; } = new List<DiffInfoLine>();

        public DiffInfoLine FirstDiff
        { get { return DiffLines[0]; } }

        public int LineCount
        { get { return DiffLines.Count; } }

        public int StartLineNumber
        {
            get
            {
                int _startLineNumber = FirstDiff.LineNumber;
                foreach (DiffInfoLine diffInfoLine in DiffLines)
                {
                    if (diffInfoLine.LineNumber < _startLineNumber) _startLineNumber = diffInfoLine.LineNumber;
                }

                return _startLineNumber;
            }
        }

        public int EndLineNumber
        {
            get
            {
                int _endLineNumber = FirstDiff.LineNumber;
                foreach (DiffInfoLine diffInfoLine in DiffLines)
                {
                    if (diffInfoLine.LineNumber > _endLineNumber) _endLineNumber = diffInfoLine.LineNumber;
                }

                return _endLineNumber;
            }
        }

        public List<string> Lines
        {
            get
            {
                List<string> _lines = new List<string>();
                foreach (DiffInfoLine diffInfoLine in DiffLines)
                {
                    _lines.Add(diffInfoLine.Line);
                }

                return _lines;
            }
        }

        public void AddDiffLine(DiffInfoLine diffInfoLine)
        {
            if (diffInfoLine == null) return;

            DiffLines.Add(diffInfoLine);
        }
    }
}