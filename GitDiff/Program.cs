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
        /// Directory of project
        /// </summary>
        private static readonly string Directory = @"C:\Projects\Visual Studio\Powershell";

        static void Main(string[] args)
        {
            // Get a list of commits
            GitCmd git = new  GitCmd();
            List<DiffInfoCommit> diffInfos = git.GetCommits(Directory, 500, ".cs");

            // Create a new code smell factory
            CodeSmellFactory codeSmellFactory = new CodeSmellFactory();
            codeSmellFactory.AddCodeSmell(new CodeSmellSwitchCase());
            codeSmellFactory.AddCodeSmell(new CodeSmellDuplicate());
            codeSmellFactory.AddCodeSmell(new CodeSmellLongParameter());

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