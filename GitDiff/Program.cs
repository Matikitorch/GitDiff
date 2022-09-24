using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Management.Automation;

namespace GitDiff
{
    public class Program
    {
        static void Main(string[] args)
        {
            string dir = @"C:\Projects\Visual Studio\WaveFunctionCollapse";

            Collection<PSObject> results;

            using (PowerShell ps = PowerShell.Create())
            {
                ps.AddScript("cd " + "\"" + dir + "\"");
                ps.AddScript("git diff HEAD^..HEAD~3");

                results = ps.Invoke();

                if ((results is not null) && (results.Count > 0))
                {
                    Console.WriteLine(String.Join(Environment.NewLine, results));
                }
            }

            Console.WriteLine("Hello World!");
        }
    }
}