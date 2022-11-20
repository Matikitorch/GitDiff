using GitDiff.Smells;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDiff
{
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
