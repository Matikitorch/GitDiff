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

        public List<SmellInfo> CodeSmellInfo
        { get; } = new List<SmellInfo>();

        public int Counts
        { get { return CodeSmellInfo.Count; } }

        public void AddSmell(SmellInfo smellInfo)
        {
            CodeSmellInfo.Add(smellInfo);
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
            const char smellHeaderChar = '*';

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

            foreach (SmellInfo smellInfo in CodeSmellInfo)
            {
                sb.AppendLine("*** " + smellInfo.DiffInfo.DiffFile.FileName + " ***");
                sb.AppendLine("Line: " + smellInfo.StartLineNumber + ".." + smellInfo.EndLineNumber);

                foreach (string line in smellInfo.Lines)
                {
                    sb.AppendLine("+" + line);
                }

                sb.AppendLine();
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
        public SmellInfo(DiffInfoLine diffLine)
        {
            if (diffLine == null) throw new ArgumentException();

            DiffLines.Add(diffLine);
        }

        public SmellInfo(List<DiffInfoLine> diffLines)
        {
            if ((diffLines == null) || (diffLines.Count == 0)) throw new ArgumentException();

            DiffLines.AddRange(diffLines);
        }

        public List<DiffInfoLine> DiffLines
        { get; } = new List<DiffInfoLine>();

        public DiffInfoLine DiffInfo
        { get { return DiffLines[0]; } }

        public int LineCount
        { get { return DiffLines.Count; } }

        public int StartLineNumber
        {
            get
            {
                if (_startLineNumber == -1)
                {
                    _startLineNumber = DiffInfo.LineNumber;
                    foreach (DiffInfoLine diffInfoLine in DiffLines)
                    {
                        if (diffInfoLine.LineNumber < _startLineNumber) _startLineNumber = diffInfoLine.LineNumber;
                    }
                }

                return _startLineNumber;
            }
        }
        private int _startLineNumber = -1;

        public int EndLineNumber
        {
            get
            {
                if (_endLineNumber == -1)
                {
                    _endLineNumber = DiffInfo.LineNumber;
                    foreach (DiffInfoLine diffInfoLine in DiffLines)
                    {
                        if (diffInfoLine.LineNumber > _endLineNumber) _endLineNumber = diffInfoLine.LineNumber;
                    }
                }

                return _endLineNumber;
            }
        }
        private int _endLineNumber = -1;

        public List<string> Lines
        {
            get
            {
                if (_lines == null)
                {
                    _lines = new List<string>();
                    foreach (DiffInfoLine diffInfoLine in DiffLines)
                    {
                        _lines.Add(diffInfoLine.Line);
                    }
                }

                return _lines;
            }
        }
        private List<string> _lines = null;
    }
}