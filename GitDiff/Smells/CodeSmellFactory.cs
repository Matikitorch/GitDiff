using GitDiff.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDiff.Smells
{
    /// <summary>
    /// Contains all of the supported code smells used for analyzing
    /// </summary>
    public class CodeSmellFactory
    {
        public CodeSmellFactory()
        {
            CodeSmellList.Add(new CodeSmellSwitchCase());
            CodeSmellList.Add(new CodeSmellDuplicate());
            CodeSmellList.Add(new CodeSmellLongParameter());
        }

        public List<CodeSmell> CodeSmellList
        { get; } = new List<CodeSmell>();

        public int Count
        { get { return CodeSmellList.Count; } }

        public void AddCodeSmell(CodeSmell codeSmell)
        {
            if (codeSmell == null) return;

            CodeSmellList.Add(codeSmell);
        }

        /// <summary>
        /// Analyzes an entire commit against all of the supported code smells
        /// </summary>
        /// <param name="diffInfoCommit"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public CodeSmellResults Analyze(DiffInfoCommit diffInfoCommit)
        {
            if (Count == 0) throw new InvalidOperationException();
            CodeSmellResults codeSmellResults = new CodeSmellResults();

            foreach (CodeSmell codeSmell in CodeSmellList)
            {
                codeSmellResults.Add(codeSmell.Analyze(diffInfoCommit));
            }

            return codeSmellResults;
        }
    }
}