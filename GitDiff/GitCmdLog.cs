using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace GitDiff
{
    public static class GitCmdLog
    {
        public static List<GitLogResult> Invoke(string dir)
        {
            using (PowerShell ps = PowerShell.Create())
            {
                ps.AddScript("cd " + "\"" + dir + "\"");
                ps.AddScript("git log --pretty=oneline");

                return ParseGitLog(ps.Invoke<string>());
            }
        }

        private static List<GitLogResult> ParseGitLog(Collection<string> results)
        {
            string commitName, commitID;
            int idx;

            List<GitLogResult> logResults = new();

            foreach (string result in results)
            {
                idx = result.IndexOf(" ");
                commitID = result.Substring(0, idx);

                idx += 1;
                if (result[idx] == '(')
                {
                    while (result[idx] != ')')
                    {
                        idx += 1;
                    }
                    idx += 1;
                }
                commitName = result.Substring(idx);

                logResults.Add(new GitLogResult(commitName, commitID));
            }

            return logResults;
        }
    }

    public class GitLogResult
    {
        public GitLogResult(string name, string commitID)
        {
            Name = name;
            CommitID = commitID;
        }

        public string Name
        { get; }

        public string CommitID
        { get; }
    }
}
