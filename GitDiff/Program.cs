using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Management.Automation;
using GitDiff.Syntax;

namespace GitDiff
{
    public class Program
    {
        static void Main(string[] args)
        {
            string dir = @"C:\Projects\Visual Studio\GitDiffTestSolution";

            // Get the results from a "git diff" command
            List<GitDiffResult> diffResults = GitCmdDiff.Invoke(dir);

            // Parse the results
            List<DiffInfo> diffInfos = GitDiffParser.Parse(diffResults, GetSyntaxFactory());

            // Contains a list of all of the commit line that are not currently supported by any syntax format in the syntax factory
            List<UnsupportedCommit> unsupportedCommits = UnsupportedDiff.CommitsNotSupported;

            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static DiffSyntaxFactory GetSyntaxFactory()
        {
            return new DiffSyntaxFactory(new List<DiffSyntax>()
            {
                new DiffSyntaxDiff(),
                new DiffSyntaxIndex(),
                new DiffSyntaxOldFile(),
                new DiffSyntaxNewFile(),
                new DiffSyntaxChunk(),
                new DiffSyntaxOldLine(),
                new DiffSyntaxNewLine(),
                new DiffSyntaxNewFileMode(),
                new DiffSyntaxDeletedFileMode(),
                new DiffSyntaxEnd(),
                new DiffSyntaxEmpty(),
            });
        }
    }
}