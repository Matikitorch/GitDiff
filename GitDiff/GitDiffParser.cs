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
        public static List<DiffInfoCommit> Parse(List<GitDiffResult> results)
        {
            List<DiffInfoCommit> diffInfoCommits = new();

            foreach (GitDiffResult diffResult in results)
            {
                diffInfoCommits.Add(DiffSyntaxDiff.Parse(diffResult));
            }

            return diffInfoCommits;
        }
    }
}