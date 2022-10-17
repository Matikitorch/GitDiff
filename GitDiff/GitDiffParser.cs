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
        public static List<DiffInfoCommit> Parse(List<GitDiffResult> results, params string[] supportedExtensions)
        {
            DiffInfoCommit diffInfoCommit;
            List<DiffInfoCommit> diffInfoCommits = new();

            foreach (GitDiffResult diffResult in results)
            {
                diffInfoCommit = DiffSyntaxDiff.Parse(diffResult, supportedExtensions);

                if (diffInfoCommit.FileCount > 0)
                {
                    diffInfoCommits.Add(diffInfoCommit);
                }
            }

            return diffInfoCommits;
        }
    }
}