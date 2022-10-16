using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDiff.Smells
{
    public class CodeSmellResult
    {
        public CodeSmellResult(CodeSmell codeSmell)
        {
            CodeSmell = codeSmell;
        }

        public CodeSmell CodeSmell
        { get; }

        public List<SmellInfo> CodeSmellInfo
        { get; } = new();

        public int Counts
        { get { return CodeSmellInfo.Count; } }

        public void AddSmell(SmellInfo smellInfo)
        {
            CodeSmellInfo.Add(smellInfo);
        }

        public string PrintResult()
        {
            const char headerChar = '-';
            const int headerLen = 100;
            const int lineNumberMin = 10;
            const int lineNumberGap = 2;
            bool isFirst;
            string str;

            StringBuilder sb = new();

            foreach (SmellInfo smellInfo in CodeSmellInfo)
            {

                sb.AppendLine(new string(headerChar, headerLen));
                sb.AppendLine(new string(' ', (int)Math.Floor((CodeSmell.Title.Length / 2.0) - (CodeSmell.Title.Length))) + CodeSmell.Title.ToUpper());
                sb.AppendLine(new string(headerChar, headerLen));
                sb.AppendLine();

                sb.AppendLine("Severity: " + CodeSmell.Severity.ToString());
                sb.AppendLine("Description: " + CodeSmell.Description);
                sb.AppendLine("Suggestion: " + CodeSmell.Suggestion);
                sb.AppendLine();

                sb.AppendLine("File: " + smellInfo.FileName);
                sb.AppendLine(new string(headerChar, headerLen / 3));

                isFirst = true;
                foreach(string line in smellInfo.Lines)
                {
                    if (isFirst)
                    {
                        str = smellInfo.StartLineNumber + (smellInfo.EndLineNumber.HasValue ? (".." + smellInfo.EndLineNumber) : string.Empty);
                        str += new string(' ', (str.Length > (lineNumberMin - lineNumberGap) ? lineNumberGap : lineNumberMin - str.Length));

                        isFirst = false;
                    }
                    else
                    {
                        str = new string(' ', lineNumberMin);
                    }

                    sb.AppendLine(str + line);
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }
    }

    public class SmellInfo
    {
        public SmellInfo(string fileName, int lineNumber, string line)
        {
            FileName = fileName;
            StartLineNumber = lineNumber;
            EndLineNumber = null;
            Lines.Add(line);
        }

        public SmellInfo(string fileName, int startLineNumber, int endLineNumber, List<string> lines)
        {
            FileName = fileName;
            StartLineNumber = startLineNumber;
            EndLineNumber = endLineNumber;
            Lines.AddRange(lines);
        }

        public string FileName
        { get; }

        public int StartLineNumber
        { get; }

        public int? EndLineNumber
        { get; internal set; }

        public List<string> Lines
        { get; } = new();
    }
}
