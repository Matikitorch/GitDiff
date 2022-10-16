using GitDiff.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDiff.Smells
{
    public class CodeSmellFactory
    {
        public CodeSmellFactory(IEnumerable<CodeSmell> codeSmells)
        {
            CodeSmellList.AddRange(codeSmells);
        }

        public List<CodeSmell> CodeSmellList
        { get; } = new();

        public int CodeSmellCount
        { get { return CodeSmellList.Count; } }

        public List<CodeSmellResult> Analyze(DiffInfoCommit diffInfoCommit)
        {
            if (CodeSmellCount == 0) throw new InvalidOperationException();
            List<CodeSmellResult> codeSmellResults = new();

            foreach(CodeSmell codeSmell in CodeSmellList)
            {
                codeSmellResults.Add(codeSmell.Analyze(diffInfoCommit));
            }

            return codeSmellResults;
        }
    }
}
