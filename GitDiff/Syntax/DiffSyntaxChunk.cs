using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDiff.Syntax
{
    public static class DiffSyntaxChunk
    {
        public static string Prefix => "@@";

        public static DiffInfoFile Parse(string? fileName, GitDiffResult diffResult, ref int commitIdx)
        {
            string commitLine;
            int lineNumber;
            int index;

            if (string.IsNullOrEmpty(fileName)) throw new InvalidOperationException();

            DiffInfoFile diffInfoFile = new DiffInfoFile(fileName);

            // Remove the prefix
            commitLine = diffResult.Commits[commitIdx].Substring(Prefix.Length).Trim();

            // Find the '+', we don't care about '-'
            index = commitLine.IndexOf('+');
            if (index < 0) throw new InvalidOperationException();
            commitLine = commitLine.Substring(index + 1);

            // Get the start line
            index = commitLine.IndexOf(',');
            if (index < 0) throw new InvalidOperationException();
            lineNumber = int.Parse(commitLine.Substring(0, index));

            // Move the index to the first line
            commitIdx += 1;

            // Start parsing
            do
            {
                commitLine = diffResult.Commits[commitIdx];

                if (!commitLine.StartsWith(DiffSyntaxOldLine.Prefix))
                {
                    if (commitLine.StartsWith(DiffSyntaxNewLine.Prefix))
                    {
                        diffInfoFile.AddLine(new DiffInfoLine(lineNumber, DiffSyntaxNewLine.Parse(commitLine)));
                    }
                    else if (!commitLine.StartsWith(' '))
                    {
                        break;
                    }

                    lineNumber += 1;
                }

                commitIdx += 1;
                if (commitIdx >= diffResult.CommitsCount) break;

            } while (true);

            return diffInfoFile;
        }
    }
}