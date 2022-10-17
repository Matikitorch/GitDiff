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
        public CodeSmellFactory(IEnumerable<CodeSmell> codeSmells)
        {
            CodeSmellList.AddRange(codeSmells);
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
        public List<CodeSmellResult> Analyze(DiffInfoCommit diffInfoCommit)
        {
            if (Count == 0) throw new InvalidOperationException();
            List<CodeSmellResult> codeSmellResults = new List<CodeSmellResult>();

            foreach (CodeSmell codeSmell in CodeSmellList)
            {
                CodeSmellResult codeSmellResult = codeSmell.Analyze(diffInfoCommit);

                if (codeSmellResult.Counts > 0)
                {
                    codeSmellResults.Add(codeSmellResult);
                }
            }

            return codeSmellResults;
        }
    }
}