using GitDiff.Syntax;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GitDiff.Smells
{
    /// <summary>
    /// Contains all of the supported code smells used for analyzing
    /// </summary>
    public class CodeSmellFactory
    {
        public delegate void CodeSmellResultHook(CodeSmellResults codeSmellResults);

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

        /// <summary>
        /// Analyzes an entire commit against all of the supported code smells
        /// </summary>
        /// <param name="diffInfoCommits"></param>
        /// <param name="codeSmellResultsQueue"></param>
        /// <returns></returns>
        public void Analyze(List<DiffInfoCommit> diffInfoCommits, ConcurrentQueue<CodeSmellResults> codeSmellResultsQueue)
        {
            if (Count == 0) throw new InvalidOperationException();

            foreach(DiffInfoCommit diffInfo in diffInfoCommits)
            {
                CodeSmellResults codeSmellResults = new CodeSmellResults();

                foreach (CodeSmell codeSmell in CodeSmellList)
                {
                    codeSmellResults.Add(codeSmell.Analyze(diffInfo));
                }

                if (codeSmellResults.HasSmells)
                {
                    codeSmellResultsQueue.Enqueue(codeSmellResults);
                }
            }
        }
    }
}
