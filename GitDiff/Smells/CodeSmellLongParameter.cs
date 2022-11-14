using GitDiff.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GitDiff.Smells
{
    public class CodeSmellLongParameter : CodeSmell
    {
        private static readonly int MaxNumOfParameters = 5;

        public CodeSmellLongParameter()
            : base(CodeSmellSeverity.Warning)
        { }

        public override string Title => "Long Parameter List";

        public override string Description => "Function signature contains too many parameters";

        public override string Suggestion => "Consider creating an object that contains all of the required parameters or refactor the function into multiple functions";

        public override CodeSmellResult Analyze(DiffInfoCommit diffInfoCommit)
        {
            int opn, cls, count;
            string line;

            CodeSmellResult codeSmellResult = new CodeSmellResult(this, diffInfoCommit);

            foreach (DiffInfoFile diffInfoFile in diffInfoCommit.DiffFile)
            {
                foreach (DiffInfoLine diffInfoLine in diffInfoFile.NewLines)
                {
                    line = diffInfoLine.Line.Trim();

                    do
                    {
                        if (line.EndsWith(';')) break;

                        opn = line.IndexOf('(');
                        cls = line.IndexOf(')');
                        if ((opn < 0) || (cls < 0) || (cls < opn)) break;

                        line = line.Substring(opn, cls - opn);

                        count = line.Count(c => c == ',');
                        if (count > 0) count += 1;

                        if (count >= MaxNumOfParameters)
                        {
                            codeSmellResult.AddSmell(new SmellInfo(diffInfoLine));
                        }
                    } while (false);
                }
            }

            return codeSmellResult;
        }

        public override void ToString(StringBuilder sb, SmellInfo smellInfo)
        {
            sb.AppendLine("[" + smellInfo.FirstDiff.LineNumber + "]: " + smellInfo.FirstDiff.Line.Trim());
        }
    }
}