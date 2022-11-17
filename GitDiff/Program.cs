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
            Console.WriteLine("Project directory: " + ProjectDirectory);
            Console.WriteLine("Results directory: " + ResultsDirectory);
            Console.WriteLine("Number of commits: " + CommitLayers.ToString());
            Console.WriteLine("File extension filter(s): " + String.Join(',', FileExtensionFilter));

            // Create a consumer to pick up the results of the code smell analysis
            Consumer myConsumer = new Consumer(ResultsDirectory);

            // Get a list of commits
            GitCmd git = new GitCmd();
            List<DiffInfoCommit> diffInfos = git.GetCommits(ProjectDirectory, CommitLayers, FileExtensionFilter);

            // Create a new code smell factory
            CodeSmellFactory codeSmellFactory = new CodeSmellFactory();

            // Analyze the results
            codeSmellFactory.Analyze(diffInfos, myConsumer.ResultsQueue);

            while (!myConsumer.IsEmpty) Thread.Sleep(1);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }

    public class Consumer
    {
        public Consumer(string resultDirectory)
        {
            ResultDirectory = resultDirectory;

            Task.Factory.StartNew(() => ThreadMain(), TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(() => Dots());
        }

        public string ResultDirectory
        { get; }

        public ConcurrentQueue<CodeSmellResults> ResultsQueue
        { get; } = new ConcurrentQueue<CodeSmellResults>();

        public bool IsEmpty
        { get { return ResultsQueue.IsEmpty; } }

        private void ClearConsoleLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.CursorTop);
        }

        private void Dots()
        {
            Console.WriteLine();
            while (!tokenSource.IsCancellationRequested)
            {
                for (int i = 0; i < 4; i++)
                {
                    ClearConsoleLine();
                    Console.Write("Analyzing" + new string('.', i));
                    Console.SetCursorPosition(0, Console.CursorTop); // Move the cursor back just for better visualization
                    if (tokenSource.Token.WaitHandle.WaitOne(TimeSpan.FromMilliseconds(250))) break;
                }
            }
            Console.WriteLine(Environment.NewLine);
        }
        private CancellationTokenSource tokenSource = new CancellationTokenSource();

        private void ThreadMain()
        {
            bool cancelDots = true;

            while (true)
            {
                while (ResultsQueue.TryDequeue(out CodeSmellResults codeSmellResults))
                {
                    if (cancelDots)
                    {
                        tokenSource.Cancel();
                        Thread.Sleep(TimeSpan.FromMilliseconds(100));
                        cancelDots = false;
                    }

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
