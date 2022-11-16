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

        public ConcurrentQueue<CodeSmellResults> CodeSmellResultsQueue
        { get; } = new ConcurrentQueue<CodeSmellResults>();

        /// <summary>
        /// Analyzes an entire commit against all of the supported code smells
        /// </summary>
        /// <param name="diffInfoCommits"></param>
        /// <param name="codeSmellResultHook"></param>
        /// <returns></returns>
        public Task Analyze(List<DiffInfoCommit> diffInfoCommits, params CodeSmellResultHook[] codeSmellResultHook)
        {
            if (Count == 0) throw new InvalidOperationException();

            ParallelLoopResult parallelLoopResult = Parallel.ForEach(diffInfoCommits, diffInfo =>
            {
                CodeSmellResults codeSmellResults = new CodeSmellResults();

                foreach (CodeSmell codeSmell in CodeSmellList)
                {
                    codeSmellResults.Add(codeSmell.Analyze(diffInfo));
                }

                if (codeSmellResults.Count > 0) CodeSmellResultsQueue.Enqueue(codeSmellResults);
            });

            return Task.Run(() =>
            {
                while (!parallelLoopResult.IsCompleted || !CodeSmellResultsQueue.IsEmpty)
                {
                    if (CodeSmellResultsQueue.TryDequeue(out CodeSmellResults codeSmellResults))
                    {
                        foreach (CodeSmellResultHook hook in codeSmellResultHook)
                        {
                            hook(codeSmellResults);
                        }
                    }
                }
            });
        }
    }
}