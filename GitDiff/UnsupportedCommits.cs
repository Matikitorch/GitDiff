using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDiff
{
    public static class UnsupportedCommits
    {
        public static List<UnsupportedCommit> CommitsNotSupported
        { get; } = new();

        public static void Add(GitDiffResult diffResult, string commit, bool isHeader)
        {
            CommitsNotSupported.Add(new UnsupportedCommit(diffResult.Name, diffResult.ID, commit, isHeader));
        }
    }

    public class UnsupportedCommit
    {
        public UnsupportedCommit(string name, string iD, string line, bool isHeader)
        {
            Name = name;
            ID = iD;
            IsHeader = isHeader;
            Line = line;
        }

        public string Name
        { get; }

        public string ID
        { get; }

        public bool IsHeader
        { get; }

        public string Line
        { get; }
    }
}
