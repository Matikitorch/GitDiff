﻿using System;
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
            string dir = @"C:\Projects\Visual Studio\GitDiffTestSolution";
            List<string> myList = new List<string>();

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

            foreach (PSObject psObject in results)
            {
                myList.Add(psObject.ToString());
            }

            GitParser.Parse(myList);

            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}