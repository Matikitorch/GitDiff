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

        public override string Suggestion => "Attempt to revise the code and implement it with if's and else if's.";

        private static Regex RegexSwitchCase
        { get; } = new(@"^\\s*(switch)\\s*\\(([^\\)]*)\\)");

        public override CodeSmellResult Analyze(DiffInfoCommit diffInfoCommit)
        {
            CodeSmellResult codeSmellResult = new(this);

            foreach (DiffInfoFile diffInfoFile in diffInfoCommit.DiffFile)
            {
                foreach (DiffInfoLine diffInfoLine in diffInfoFile.NewLines)
                {
                    if (RegexSwitchCase.IsMatch(diffInfoLine.Line))
                    {
                        codeSmellResult.AddSmell(new(diffInfoFile.FileName, diffInfoLine.LineNumber, diffInfoLine.Line));
                    }
                }
            }

            return codeSmellResult;
        }
    }
}