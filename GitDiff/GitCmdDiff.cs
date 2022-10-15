using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace GitDiff
{
    public class GitCmdDiff
    {
        public static List<GitDiffResult> Invoke(string dir, uint depth = uint.MaxValue)
        {
            List<GitDiffResult> diffResults = new();
            List<GitLogResult> logResults;
            GitLogResult newCommit, oldCommit;
            Collection<string> commits;

            // Get a list of all the commits
            logResults = GitCmdLog.Invoke(dir);

            for (int i = 0; (i < logResults.Count - 1) && (i < depth); i++)
            {
                newCommit = logResults[i];
                oldCommit = logResults[i + 1];

                using (PowerShell ps = PowerShell.Create())
                {
                    ps.AddScript("cd " + "\"" + dir + "\"");
                    ps.AddScript("git diff " + oldCommit.CommitID + " " + newCommit.CommitID);

                    commits = ps.Invoke<string>();
                    if ((commits != null) && (commits.Count > 0))
                    {
                        diffResults.Add(new GitDiffResult(newCommit.Name, newCommit.CommitID, commits.ToList()));
                    }
                }
            }

            return diffResults;
        }
    }

    public class GitDiffResult
    {
        public GitDiffResult(string name, string id, List<string> results)
        {
            Name = name;
            ID = id;
            Commits = results;
        }

        public string Name
        { get; }

        public string ID
        { get; }

        public List<string> Commits
        { get; }
    }
}
