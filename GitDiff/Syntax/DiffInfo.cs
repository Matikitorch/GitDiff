using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace GitDiff.Syntax
{
    /// <summary>
    /// Data container for a single commit
    /// </summary>
    public class DiffInfoCommit
    {
        public DiffInfoCommit(string commitName, string commitID)
        {
            CommitName = commitName;
            CommitID = commitID;
        }

        public string CommitName
        { get; }

        public string CommitID
        { get; }

        public List<DiffInfoFile> DiffFile
        { get; } = new List<DiffInfoFile>();

        public int Count
        { get { return DiffFile.Count; } }

        public void AddFile(DiffInfoFile diffFile)
        {
            if (diffFile.Count == 0) return;

            DiffFile.Add(diffFile);
        }
    }

    /// <summary>
    /// Data container for a single commit in a single file
    /// </summary>
    public class DiffInfoFile
    {
        public DiffInfoFile(DiffInfoCommit diffCommit, string fileName)
        {
            DiffCommit = diffCommit;
            FileName = fileName;
        }

        public DiffInfoCommit DiffCommit
        { get; }

        public string FileName
        { get; }

        public List<DiffInfoLine> NewLines
        { get; } = new List<DiffInfoLine>();

        public int Count
        { get { return NewLines.Count; } }

        public void AddLine(DiffInfoLine diffInfoLine)
        {
            NewLines.Add(diffInfoLine);
        }
    }

    /// <summary>
    /// Data container for a commit in single file at a specific line
    /// </summary>
    public class DiffInfoLine
    {
        public DiffInfoLine(DiffInfoFile diffFile, int lineNumber, string line)
        {
            DiffFile = diffFile;
            LineNumber = lineNumber;
            Line = line;
        }

        public DiffInfoFile DiffFile
        { get; }

        public int LineNumber
        { get; }

        public string Line
        { get; }
    }
}