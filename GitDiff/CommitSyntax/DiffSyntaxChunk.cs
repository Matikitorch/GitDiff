using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDiff.Git;

namespace GitDiff.Syntax
{
    public static class DiffSyntaxChunk
    {
        public static string Prefix => "@@";

        public static DiffInfoFile Parse(DiffInfoCommit diffInfoCommit, string? fileName, GitDiffResult diffResult, ref int commitIdx)
        {
            string commitLine;
            int lineNumber;
            int index;

            if (string.IsNullOrEmpty(fileName)) throw new InvalidOperationException();

            DiffInfoFile diffInfoFile = new DiffInfoFile(diffInfoCommit, fileName);

            commitLine = diffResult.CommitResults[commitIdx].Substring(Prefix.Length).Trim();

            // Find the '+', we don't care about '-'
            index = commitLine.IndexOf('+');
            if (index < 0) throw new InvalidOperationException();
            commitLine = commitLine.Substring(index + 1);

            // Get the start line
            index = commitLine.IndexOf(',');
            if (index < 0) throw new InvalidOperationException();
            lineNumber = int.Parse(commitLine.Substring(0, index));

            commitIdx += 1;

            // Start parsing
            do
            {
                commitLine = diffResult.CommitResults[commitIdx];

                if (!commitLine.StartsWith(DiffSyntaxOldLine.Prefix))
                {
                    if (commitLine.StartsWith(DiffSyntaxNewLine.Prefix))
                    {
                        diffInfoFile.AddLine(new DiffInfoLine(diffInfoFile, lineNumber, DiffSyntaxNewLine.Parse(commitLine)));
                    }
                    else if (!commitLine.StartsWith(' '))
                    {
                        break;
                    }

                    lineNumber += 1;
                }

                commitIdx += 1;
                if (commitIdx >= diffResult.Count) break;

            } while (true);

            return diffInfoFile;
        }
    }
}