using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace GitDiff
{
    public static class GitCmdLog
    {
        /// <summary>
        /// Invokes a 'git log'
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<GitLogResult> Invoke(string path)
        {
            using (PowerShell ps = PowerShell.Create())
            {
                ps.AddScript("cd " + "\"" + path + "\"");
                ps.AddScript("git log --pretty=oneline");

                return ParseGitLog(ps.Invoke<string>());
            }
        }

        /// <summary>
        /// Parses the result of a 'git log' into names and commit IDs
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        private static List<GitLogResult> ParseGitLog(Collection<string> results)
        {
            string commitName, commitID;
            int idx;

            List<GitLogResult> logResults = new List<GitLogResult>();

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

    /// <summary>
    /// Data container for a 'git log'
    /// </summary>
    public class GitLogResult
    {
        public GitLogResult(string commitName, string commitID)
        {
            CommitName = commitName;
            CommitID = commitID;
        }

        public string CommitName
        { get; }

        public string CommitID
        { get; }
    }
}
