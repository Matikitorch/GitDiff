using GitDiff.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDiff.Git
{
    public class GitCmd
    {
        public GitCmd()
        {
            gitLog = new GitCmdLog();
            gitDiff = new GitCmdDiff();
            gitCommitParser = new GitDiffParser();
        }

        private readonly GitCmdLog gitLog;

        private readonly GitCmdDiff gitDiff;

        private readonly GitDiffParser gitCommitParser;

        /// <summary>
        /// Gets a list of commits
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="depth"></param>
        /// <param name="supportedExtensions"></param>
        /// <returns></returns>
        public List<DiffInfoCommit> GetCommits(string directory, uint depth = uint.MaxValue, params string[] supportedExtensions)
        {
            // Invokes the log of the project
            List<GitLogResult> gitLogResults = gitLog.Invoke(directory);

            // Iterates through the log and executes a 'git diff' to get what's changed
            List<GitDiffResult> gitDiffResults = gitDiff.Invoke(gitLogResults, directory, depth);

            // Parses through the commit changes
            List<DiffInfoCommit> gitDiffCommitResults = gitCommitParser.Parse(gitDiffResults, supportedExtensions);

            return gitDiffCommitResults;
        }
    }
}