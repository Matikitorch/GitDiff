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

        public void AddFile(DiffInfoFile diffInfoFile)
        {
            if (diffInfoFile.NewLinesCount == 0) return;

            DiffFile.Add(diffInfoFile);
        }
    }

    public class DiffInfoFile
    {
        public DiffInfoFile(string fileName)
        {
            FileName = fileName;
        }

        public string FileName
        { get;  }
        
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
        public DiffInfoLine(int lineNumber, string line)
        {
            LineNumber = lineNumber;
            Line = line;
        }   

        public int LineNumber
        { get; }

        public string Line
        { get; }
    }
}