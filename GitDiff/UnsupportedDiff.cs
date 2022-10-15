using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDiff
{
    public static class UnsupportedDiff
    {
        public static List<UnsupportedCommit> CommitsNotSupported
        { get; } = new();

        public static void Add(GitDiffResult diffResult, bool isHeader, string commit)
        {
            CommitsNotSupported.Add(new UnsupportedCommit(diffResult.Name, diffResult.ID, isHeader, commit));
        }
    }

    public class UnsupportedCommit
    {
        public UnsupportedCommit(string name, string iD, bool isHeader, string line)
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
