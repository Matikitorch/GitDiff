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
        /// <summary>
        /// Given a list of results from a 'git diff' command, this function will iterate through all of
        /// the commits and parse it even further into 
        /// </summary>
        /// <param name="diffResults"></param>
        /// <param name="supportedExtensions"></param>
        /// <returns></returns>
        public static List<DiffInfoCommit> Parse(List<GitDiffResult> diffResults, params string[] supportedExtensions)
        {
            DiffInfoCommit diffInfoCommit;
            List<DiffInfoCommit> diffInfoCommits = new List<DiffInfoCommit>();

            foreach (GitDiffResult diffResult in diffResults)
            {
                diffInfoCommit = DiffSyntaxDiff.Parse(diffResult, supportedExtensions);

                if (diffInfoCommit.Count > 0)
                {
                    diffInfoCommits.Add(diffInfoCommit);
                }
            }

            return diffInfoCommits;
        }
    }
}