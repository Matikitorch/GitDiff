using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDiff.Git;

namespace GitDiff.Syntax
{
    public static class DiffSyntaxDiff
    {
        public static string Prefix => "diff";

        public static DiffInfoCommit Parse(GitDiffResult diffResult, params string[] supportedExtensions)
        {
            DiffInfoFile diffInfoFile;
            string commitLine;
            string fileName;
            int commitIdx;

            DiffInfoCommit diffInfoCommit = new DiffInfoCommit(diffResult.CommitName, diffResult.CommitID);

            commitIdx = 0;
            while (commitIdx < diffResult.Count)
            {
                commitLine = diffResult.CommitResults[commitIdx];

                if (commitLine.StartsWith(DiffSyntaxDiff.Prefix))
                {
                    fileName = null;

                    // Find the 'fileName' and spin until 'chunk'
                    do
                    {
                        if (commitLine.StartsWith(DiffSyntaxNewFile.Prefix))
                        {
                            fileName = DiffSyntaxNewFile.Parse(commitLine);
                        }

                        if (++commitIdx >= diffResult.Count) break;
                        commitLine = diffResult.CommitResults[commitIdx];
                    } while (!commitLine.StartsWith(DiffSyntaxChunk.Prefix));

                    // We should've gotten a file name
                    if (string.IsNullOrEmpty(fileName))
                    {
                        commitIdx += 1;
                        continue;
                    }

                    if ((supportedExtensions.Length == 0) || (fileName.Contains('.') && supportedExtensions.Any(fileName.Substring(fileName.LastIndexOf('.')).Contains)))
                    {
                        // Parse one or more 'chunk's
                        while (commitLine.StartsWith(DiffSyntaxChunk.Prefix))
                        {
                            diffInfoFile = DiffSyntaxChunk.Parse(diffInfoCommit, fileName, diffResult, ref commitIdx);
                            if (diffInfoFile.Count > 0)
                            {
                                diffInfoCommit.AddFile(diffInfoFile);
                            }

                            if (commitIdx >= diffResult.Count) break;
                            commitLine = diffResult.CommitResults[commitIdx];
                        }
                    }
                }
                else
                {
                    commitIdx += 1;
                }
            }

            return diffInfoCommit;
        }
    }
}