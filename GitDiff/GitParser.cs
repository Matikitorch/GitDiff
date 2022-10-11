using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDiff
{
    public static class GitParser
    {

        public static GitInfo Parse(List<string> results)
        {
            if (results == null) throw new ArgumentNullException(nameof(results));
            if (results.Count == 0) throw new ArgumentException("Nothing to parse.");

            GitInfo gitInfo = new GitInfo();

            foreach (string result in results)
            {
                ParseResult(gitInfo, result);
            }

            return gitInfo;
        }

        private static void ParseResult(GitInfo gitInfo, string result)
        {
            string firstWord;
            int index;

            do
            {
                if (result.StartsWith(' '))
                {
                    // Do nothing...
                }
                else
                {
                    index = result.IndexOf(' ');
                    if (index < 0) break;

                    firstWord = result.Substring(0, index);

                    if (string.Compare(firstWord, "diff", StringComparison.CurrentCultureIgnoreCase) == 0)
                    {
                        ParseDiff(gitInfo, result);
                    }
                    else if (string.Compare(firstWord, "index", StringComparison.CurrentCultureIgnoreCase) == 0)
                    {
                        ParseIndex(gitInfo, result);
                    }
                    else if (string.Compare(firstWord, "---", StringComparison.CurrentCultureIgnoreCase) == 0)
                    {
                        ParseOldFile(gitInfo, result);
                    }
                    else if (string.Compare(firstWord, "+++", StringComparison.CurrentCultureIgnoreCase) == 0)
                    {
                        ParseNewFile(gitInfo, result);
                    }
                    else if (string.Compare(firstWord, "@@", StringComparison.CurrentCultureIgnoreCase) == 0)
                    {
                        ParseChunk(gitInfo, result);
                    }
                    else if (string.Compare(firstWord, "-", StringComparison.CurrentCultureIgnoreCase) == 0)
                    {
                        ParseOldLine(gitInfo, result);
                    }
                    else if (string.Compare(firstWord, "+", StringComparison.CurrentCultureIgnoreCase) == 0)
                    {
                        ParseNewLine(gitInfo, result);
                    }
                    else if (string.Compare(firstWord, "\\", StringComparison.CurrentCultureIgnoreCase) == 0)
                    {
                        ParseEnd(gitInfo, result);
                    }
                    else
                    {
                        throw new NotSupportedException("Argument \"" + result + "\" is not supported.");
                    }
                }
            } while (false);
        }

        private static void ParseDiff(GitInfo info, string result)
        {
            string oldFile, newFile;
            int oldFileIndex, newFileIndex;

            oldFileIndex = result.IndexOf("a/");
            if (oldFileIndex < 0) throw new ArgumentException("Failed to find \"a/\" in \"" + result + "\"");

            newFileIndex = result.IndexOf("b/");
            if (newFileIndex < 0) throw new ArgumentException("Failed to find \"b/\" in \"" + result + "\"");

            oldFile = result.Substring(oldFileIndex + 2, newFileIndex - 2);
            newFile = result.Substring(newFileIndex + 2);

            info.OldFile = oldFile;
            info.NewFile = newFile;
        }

        private static void ParseIndex(GitInfo info, string result)
        {
            int index;

            result = result.Substring("index ".Length);

            index = result.IndexOf("..");
            if (index < 0) throw new ArgumentException("Failed to find \"..\" in \"" + result + "\"");
            info.OldIndex = result.Substring(0, index);

            result = result.Substring(index + 2);

            index = result.IndexOf(" ");
            if (index < 0) throw new ArgumentException("Failed to find whitespace in \"" + result + "\"");
            info.NewIndex = result.Substring(0, index);
        }

        private static void ParseOldFile(GitInfo info, string result)
        {
            int index;

            index = result.IndexOf("a/");
            if (index < 0) throw new ArgumentException("Failed to find \"a/\" in \"" + result + "\"");

            info.OldFile = result.Substring(index + 2);
        }

        private static void ParseNewFile(GitInfo info, string result)
        {
            int index;

            index = result.IndexOf("b/");
            if (index < 0) throw new ArgumentException("Failed to find \"b/\" in \"" + result + "\"");

            info.NewFile = result.Substring(index + 2);
        }

        private static void ParseChunk(GitInfo info, string result)
        {
            string startLine, numOfLines;
            int index;

            result = result.Substring(3); // Removes "@@ "

            while (result.Length > 0)
            {
                if (result.StartsWith("@@")) break;
                if (!result.StartsWith("-") && !result.StartsWith("+")) throw new ArgumentException("Failed to find \"-\" or \"+\" in \"" + result + "\"");

                index = result.IndexOf(",");
                startLine = result.Substring(1, index - 1);

                result = result.Substring(index + 1);

                index = result.IndexOf(" ");
                numOfLines = result.Substring(0, index);

                info.Chunks.Add(new GitInfo.Chunk(int.Parse(startLine), int.Parse(numOfLines)));

                result = result.Substring(index + 1);
            }
        }

        private static void ParseOldLine(GitInfo info, string result)
        {
            info.OldLines.Add(result.TrimStart('-').Trim());
        }

        private static void ParseNewLine(GitInfo info, string result)
        {
            info.NewLines.Add(result.TrimStart('+').Trim());
        }

        private static void ParseEnd(GitInfo info, string result)
        {
            // Do nothing...
        }
    }
}