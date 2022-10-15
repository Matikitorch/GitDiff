using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDiff.Syntax;

namespace GitDiff
{
    public static class GitDiffParser
    {
        public static List<DiffInfo> Parse(List<GitDiffResult> results, DiffSyntaxFactory diffSyntaxFactory)
        {
            if ((diffSyntaxFactory == null) || (diffSyntaxFactory.Count == 0)) throw new ArgumentNullException(nameof(diffSyntaxFactory));
            if (results == null) throw new ArgumentNullException(nameof(results));
            if (results.Count == 0) throw new ArgumentException("Nothing to parse.");

            List<DiffInfo> gitInfos = new();
            foreach (GitDiffResult diffResult in results)
            {
                ParseDiffResult(diffSyntaxFactory, diffResult, out DiffInfo diffInfo);

                gitInfos.Add(diffInfo);
            }

            return gitInfos;
        }

        private static void ParseDiffResult(DiffSyntaxFactory diffSyntaxFactory, GitDiffResult diffResult, out DiffInfo diffInfo)
        {
            bool isHeader = true;

            diffInfo = new(diffResult.Name, diffResult.ID);

            foreach (string commitLine in diffResult.Commits)
            {
                if (!diffSyntaxFactory.MatchSyntax(ref isHeader, commitLine, out DiffSyntax diffSyntax))
                {
                    UnsupportedDiff.Add(diffResult, isHeader, commitLine);
                }
                else
                {
                    diffSyntax.ParseCommit(diffInfo, commitLine);
                }
            }
        }
    }
}