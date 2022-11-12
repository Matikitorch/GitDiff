using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Management.Automation;
using GitDiff.Syntax;
using GitDiff.Smells;
using GitDiff.Git;

namespace GitDiff
{
    public class Program
    {
        /// <summary>
        /// Project's directory
        /// </summary>
        private static string Directory;

        /// <summary>
        /// Number of commits to analyze
        /// </summary>
        private static uint CommitLayers;

        /// <summary>
        /// Only look at files with these extensions
        /// </summary>
        private static string[] FileExtensionFilter;

        static void Main(string[] args)
        {
            // Read in the INI file
            IniFile myIniFile = new IniFile();
            Directory = myIniFile.Read("Directory");
            CommitLayers = uint.Parse(myIniFile.Read("CommitLayers"));
            FileExtensionFilter = myIniFile.Read("FileExtensionFilter").Split(',');

            // Get a list of commits
            GitCmd git = new GitCmd();
            List<DiffInfoCommit> diffInfos = git.GetCommits(Directory, CommitLayers, FileExtensionFilter);

            // Create a new code smell factory
            CodeSmellFactory codeSmellFactory = new CodeSmellFactory();

            // Analyze the results on a commit by commit basis
            foreach (DiffInfoCommit diffInfo in diffInfos)
            {
                CodeSmellResults codeSmellResults = codeSmellFactory.Analyze(diffInfo);

                // Print the results of the smell analysis
                string toPrint = codeSmellResults.GetString();
                if (!string.IsNullOrEmpty(toPrint)) Console.WriteLine(toPrint);
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}