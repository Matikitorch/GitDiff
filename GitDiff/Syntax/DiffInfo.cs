using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace GitDiff.Syntax
{
    public class DiffInfoCommit
    {
        public DiffInfoCommit(string name, string id)
        {
            Name = name;
            ID = id;
        }

        public string Name
        { get; }

        public string ID
        { get; }

        public List<DiffInfoFile> DiffFile
        { get; } = new();

        public int FileCount
        { get { return DiffFile.Count; } }

        public void AddFile(DiffInfoFile diffInfoFile)
        {
            if (diffInfoFile.NewLinesCount == 0) return;

            DiffFile.Add(diffInfoFile);
        }
    }

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
        { get; } = new();

        public int NewLinesCount
        { get { return NewLines.Count; } }

        public void AddLine(DiffInfoLine diffInfoLine)
        {
            NewLines.Add(diffInfoLine);
        }
    }

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