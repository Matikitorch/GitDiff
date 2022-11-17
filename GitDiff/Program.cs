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
        private static string ProjectDirectory;
        
        /// <summary>
        /// Directory where any/all results should go
        /// </summary>
        private static string ResultsDirectory;

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
            ProjectDirectory = myIniFile.Read("ProjectDirectory");
            ResultsDirectory = myIniFile.Read("ResultsDirectory");
            CommitLayers = uint.Parse(myIniFile.Read("CommitLayers"));
            FileExtensionFilter = myIniFile.Read("FileExtensionFilter").Split(',');

            // Get a list of commits
            GitCmd git = new GitCmd();
            List<DiffInfoCommit> diffInfos = git.GetCommits(ProjectDirectory, CommitLayers, FileExtensionFilter);

            // Create a new code smell factory
            CodeSmellFactory codeSmellFactory = new CodeSmellFactory();

            // Create a consumer to pick up the results of the code smell analysis
            Consumer myConsumer = new Consumer(ResultsDirectory);

            // Analyze the results
            codeSmellFactory.Analyze(diffInfos, myConsumer.ResultsQueue);

            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }

    public class Consumer
    {
        public Consumer(string resultDirectory)
        {
            ResultDirectory = resultDirectory;

            Task.Factory.StartNew(() => ThreadMain(), TaskCreationOptions.LongRunning);
        }

        public string ResultDirectory
        { get; }

        public ConcurrentQueue<CodeSmellResults> ResultsQueue
        { get; } = new ConcurrentQueue<CodeSmellResults>();

        private void ThreadMain()
        {
            while (true)
            {
                while (ResultsQueue.TryDequeue(out CodeSmellResults codeSmellResults))
                {
                    ConsolePrint(codeSmellResults);
                    CSVPrint(codeSmellResults);
                }

                Thread.Sleep(TimeSpan.FromMilliseconds(10));
            }
        }

        public void ConsolePrint(CodeSmellResults codeSmellResults)
        {
            string toPrint = codeSmellResults.Print();

            if (!string.IsNullOrWhiteSpace(toPrint))
            {
                Console.WriteLine(toPrint);
            }
        }

        public void CSVPrint(CodeSmellResults codeSmellResults)
        {
            codeSmellResults.SaveToCSV(ResultDirectory);
        }
    }
}
