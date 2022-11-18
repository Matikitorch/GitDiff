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
            StartDots();

            // Create a consumer to pick up the results of the code smell analysis
            Consumer myConsumer = new Consumer(ResultsDirectory)
            {
                OnStart = CancelDots
            };

            // Get a list of commits
            GitCmd git = new GitCmd();
            List<DiffInfoCommit> diffInfos = git.GetCommits(ProjectDirectory, CommitLayers, FileExtensionFilter);

            // Create a new code smell factory
            CodeSmellFactory codeSmellFactory = new CodeSmellFactory();

            // Analyze the results
            codeSmellFactory.Analyze(diffInfos, myConsumer.ResultsQueue);

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

        private static void Dots()
        {
            Console.WriteLine();
            while (!TokenSource.IsCancellationRequested)
            {
                for (int i = 0; i < 4; i++)
                {
                    ClearConsoleLine();
                    Console.Write("Analyzing" + new string('.', i));
                    Console.SetCursorPosition(0, Console.CursorTop); // Move the cursor back just for better visualization
                    if (TokenSource.Token.WaitHandle.WaitOne(TimeSpan.FromMilliseconds(250))) break;
                }
            }
            Console.WriteLine(Environment.NewLine);
        }
        private static CancellationTokenSource TokenSource = new CancellationTokenSource();
        private static Task DotsTask;

        private static void StartDots()
        {
            DotsTask = Task.Run(() => Dots());
        }

        private static void CancelDots()
        {
            TokenSource.Cancel();
            while (!DotsTask.IsCompleted) Thread.Sleep(1);
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

        public bool ResultsQueueIsEmpty
        { get { return ResultsQueue.IsEmpty; } }

        public delegate void ConsumerCallback();

        public ConsumerCallback OnStart
        { get; set; } = null;

        public ConsumerCallback OnEnd
        { get; set; } = null;

        private void ThreadMain()
        {
            while (ResultsQueueIsEmpty) Thread.Sleep(TimeSpan.FromMilliseconds(50));
            if (OnStart is not null) OnStart();
            
            while (true)
            {
                while (ResultsQueue.TryDequeue(out CodeSmellResults codeSmellResults))
                {
                    ConsolePrint(codeSmellResults);
                    CSVPrint(codeSmellResults);
                }

                Thread.Sleep(TimeSpan.FromMilliseconds(10));
            }

            if (OnEnd is not null) OnEnd();
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
