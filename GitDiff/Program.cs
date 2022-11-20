using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Management.Automation;
using GitDiff.Syntax;
using GitDiff.Smells;
using GitDiff.Git;
using System.Linq;

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
            ProjectDirectory = myIniFile.Read("ProjectDirectory").TrimEnd('\\');
            ResultsDirectory = myIniFile.Read("ResultsDirectory").TrimEnd('\\'); ;
            CommitLayers = uint.Parse(myIniFile.Read("CommitLayers"));
            FileExtensionFilter = myIniFile.Read("FileExtensionFilter").Split(',');

            // Print metadata
            Console.WriteLine("GitDiff");
            Console.WriteLine("Project directory: " + ProjectDirectory);
            Console.WriteLine("Results directory: " + ResultsDirectory);
            Console.WriteLine("Number of commits: " + CommitLayers.ToString());
            Console.WriteLine("File extension filter(s): " + String.Join(',', FileExtensionFilter));
            StartAnalyzingMessage();

            // Create a consumer to pick up the results of the code smell analysis
            Consumer myConsumer = new Consumer(ResultsDirectory)
            {
                OnStart = StopAnalyzingMessage
            };

            // Get a list of commits
            GitCmd git = new GitCmd();
            List<DiffInfoCommit> diffInfos = git.GetCommits(ProjectDirectory, CommitLayers, FileExtensionFilter);

            // Create a new code smell factory
            CodeSmellFactory codeSmellFactory = new CodeSmellFactory();

            // Analyze the results
            codeSmellFactory.Analyze(diffInfos, myConsumer.ResultsQueue);

            // Terminate process
            while (!myConsumer.ResultsQueueIsEmpty) Thread.Sleep(1);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static void ClearConsoleLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.CursorTop);
        }

        private static void AnalyzingMessage()
        {
            Console.WriteLine();
            while (!TokenSource.IsCancellationRequested)
            {
                for (int i = 0; i < 4; i++)
                {
                    ClearConsoleLine();
                    Console.Write("Analyzing" + new string('.', i));
                    if (TokenSource.Token.WaitHandle.WaitOne(TimeSpan.FromMilliseconds(250))) break;
                }
            }
            Console.WriteLine(Environment.NewLine);
        }
        private static CancellationTokenSource TokenSource = new CancellationTokenSource();
        private static Task DotsTask;

        private static void StartAnalyzingMessage()
        {
            DotsTask = Task.Run(() => AnalyzingMessage());
        }

        private static void StopAnalyzingMessage()
        {
            TokenSource.Cancel();
            while (!DotsTask.IsCompleted) Thread.Sleep(1);
        }
    }
}
