using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace GitDiff.Git
{
    public class GitCmdDiff
    {
        /// <summary>
        /// Invokes a 'git diff'
        /// </summary>
        /// <param name="logResults"></param>
        /// <param name="path"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        public List<GitDiffResult> Invoke(List<GitLogResult> logResults, string path, uint depth = uint.MaxValue)
        {
            List<GitDiffResult> diffResults = new List<GitDiffResult>();
            GitLogResult newCommit, oldCommit;
            Collection<string> commits;

            for (int i = 0; (i < logResults.Count - 1) && (i < depth); i++)
            {
                newCommit = logResults[i];
                oldCommit = logResults[i + 1];

                using (PowerShell ps = PowerShell.Create())
                {
                    ps.AddScript("cd " + "\"" + path + "\"");
                    ps.AddScript("git diff " + oldCommit.CommitID + "..." + newCommit.CommitID);

                    commits = ps.Invoke<string>();
                    if (commits != null && commits.Count > 0)
                    {
                        diffResults.Add(new GitDiffResult(newCommit.CommitName, newCommit.CommitID, commits.ToList()));
                    }
                }
            }

            return diffResults;
        }
    }

    /// <summary>
    /// Data container for a 'git diff'
    /// </summary>
    public class GitDiffResult
    {
        public GitDiffResult(string commitName, string commitID, List<string> commitResults)
        {
            CommitName = commitName;
            CommitID = commitID;
            CommitResults = commitResults;
        }

        public string CommitName
        { get; }

        public string CommitID
        { get; }

        public List<string> CommitResults
        { get; }

        public int Count
        { get { return CommitResults.Count; } }
    }
}
