using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Management.Automation;
using GitDiff.Syntax;
using GitDiff.Smells;
using System.Text;

namespace GitDiff
{
    public class Program
    {
        static void Main(string[] args)
        {
            string dir = @"C:\Projects\Visual Studio\Powershell";

            // Create a new code smell factory
            CodeSmellFactory codeSmellFactory = new CodeSmellFactory(new List<CodeSmell>()
            {
                new CodeSmellSwitchCase(),
                new CodeSmellDuplicate(),
                new CodeSmellLongParameter(),
            });

            // Invoke a 'git diff'
            List<GitDiffResult> diffResults = GitCmdDiff.Invoke(dir, 500);

            // Parse the 'git diff' results
            List<DiffInfoCommit> diffInfos = GitDiffParser.Parse(diffResults, ".cs");

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