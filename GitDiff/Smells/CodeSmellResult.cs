using GitDiff.Syntax;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDiff;
using Markdig.Helpers;
using System.Reflection;

namespace GitDiff.Smells
{
    public class CodeSmellResults
    {
        private string EXE = Assembly.GetExecutingAssembly().GetName().Name ?? throw new ArgumentNullException();
        private static bool IsFirst = true;

        public List<CodeSmellResult> CodeSmellResultList
        { get; } = new List<CodeSmellResult>();

        public bool HasSmells
        { get { return CodeSmellResultList.Count > 0; } }

        public void Add(CodeSmellResult codeSmellResult)
        {
            if ((codeSmellResult == null) || !codeSmellResult.HasSmells) return;

            CodeSmellResultList.Add(codeSmellResult);
        }

        public string Print()
        {
            StringBuilder sb = new StringBuilder();

            foreach (CodeSmellResult codeSmellResult in CodeSmellResultList)
            {
                sb.AppendLine(codeSmellResult.ToString());
            }

            return sb.ToString();
        }

        /// <summary>
        /// Column 1: Commit ID
        /// Column 2: Code smell name
        /// Column 3: File name
        /// Column 5: Line number
        /// Column 6: Line contents
        /// </summary>
        public void SaveToCSV(string directory)
        {
            if (CodeSmellResultList.Count == 0) return;
            string fileFullName = directory + "\\" + EXE + ".csv";
            StringBuilder sb = new StringBuilder();

            if (IsFirst)
            {
                IsFirst = false;

                try
                {
                    File.Delete(fileFullName);
                }
                catch { }
            }

            if (!File.Exists(fileFullName))
            {
                File.Create(fileFullName).Close();
                sb.Append("Commit ID,Code Smell Name, File Name, Line Number, Line Contents");
            }

            foreach (CodeSmellResult codeSmellResult in CodeSmellResultList)
            {
                if (!codeSmellResult.HasSmells) continue;

                foreach (SmellInfo smellInfo in codeSmellResult.CodeSmellList)
                {
                    if (smellInfo.LineCount == 0) continue;

                    foreach (DiffInfoLine diffInfoLine in smellInfo.DiffLines)
                    {
                        sb.AppendLine();

                        sb.Append(codeSmellResult.Commit.CommitID + ",");
                        sb.Append(codeSmellResult.CodeSmell.Title + ",");
                        sb.Append(diffInfoLine.DiffFile.FileName + ",");
                        sb.Append(diffInfoLine.LineNumber.ToString() + ",");
                        sb.Append(diffInfoLine.Line.Trim());
                    }
                }
            }

            File.AppendAllText(fileFullName, sb.ToString());
        }
    }

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

        public bool HasSmells
        { get { return Counts > 0; } }

        public void AddSmell(SmellInfo smellInfo)
        {
            CodeSmellList.Add(smellInfo);
        }

        /// <summary>
        /// Returns a user-friendly printable string of all the smelly code
        /// </summary>
        /// <returns></returns>
        public override string ToString()
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
                CodeSmell.ToString(sb, smellInfo);
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
            if ((diffLine == null) || (diffLine.Length == 0) || diffLine.Contains(null)) throw new ArgumentException();

            DiffLines.AddRange(diffLine);
        }

        public SmellInfo(List<DiffInfoLine> diffLines)
        {
            if ((diffLines == null) || (diffLines.Count == 0) || diffLines.Contains(null)) throw new ArgumentException();

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