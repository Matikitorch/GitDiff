using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            DiffInfoCommit diffInfoCommit = new(diffResult.Name, diffResult.ID);

            commitIdx = 0;
            while (commitIdx < diffResult.CommitsCount)
            {
                commitLine = diffResult.Commits[commitIdx];

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

                        if (++commitIdx >= diffResult.CommitsCount) break;
                        commitLine = diffResult.Commits[commitIdx];

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
                            if (diffInfoFile.NewLinesCount > 0)
                            {
                                diffInfoCommit.AddFile(diffInfoFile);
                            }

                            if (commitIdx >= diffResult.CommitsCount) break;
                            commitLine = diffResult.Commits[commitIdx];
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