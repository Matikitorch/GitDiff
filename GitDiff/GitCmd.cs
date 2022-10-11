using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace GitDiff
{
    public class GitCmd
    {
        public GitCmd(string url)
        {

        }

        public void Foo()
        {
            string dir = @"C:\Projects\Visual Studio\GitDiffTestSolution";

            Collection<PSObject> results;

            using (PowerShell ps = PowerShell.Create())
            {
                ps.AddScript("cd " + "\"" + dir + "\"");
                ps.AddScript("git diff HEAD^ HEAD");

                results = ps.Invoke();

                if ((results is not null) && (results.Count > 0))
                {
                    Console.WriteLine(String.Join(Environment.NewLine, results));
                }
            }


            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
