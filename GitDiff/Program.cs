using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Management.Automation;
using GitDiff.Syntax;
using GitDiff.Smells;

namespace GitDiff
{
    public class Program
    {
        static void Main(string[] args)
        {
            List<CodeSmellResult> codeSmellResults;

            string dir = @"C:\Projects\Visual Studio\GitDiffTestSolution";

            // Get the results from a "git diff" command
            List<GitDiffResult> diffResults = GitCmdDiff.Invoke(dir);

            // Parse the results
            List<DiffInfoCommit> diffInfos = GitDiffParser.Parse(diffResults);

            // Create a new code smell factory
            CodeSmellFactory codeSmellFactory = new CodeSmellFactory(new List<CodeSmell>()
            {
                new CodeSmellSwitchCase()
            });

            // Analyze the results
            foreach (DiffInfoCommit diffInfo in diffInfos)
            {
                codeSmellResults = codeSmellFactory.Analyze(diffInfo);

                foreach (CodeSmellResult codeSmellResult in codeSmellResults)
                {
                    Console.WriteLine(codeSmellResult.PrintResult());
                }
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}