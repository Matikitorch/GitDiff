using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace GitDiff
{
    public class GitInfo
    {
        //public GitInfo(int oldIndex, string oldFile, int newIndex, string newFile)
        //{
        //    OldIndex = oldIndex;
        //    OldFile = oldFile;
        //    NewIndex = newIndex;
        //    NewFile = newFile;
        //}

        public string OldIndex
        { get; internal set; }

        public string OldFile
        { get; internal set; }

        public string NewIndex
        { get; internal set; }

        public string NewFile
        { get; internal set; }

        public List<string> NewLines
        { get; } = new List<string>();

        public List<string> OldLines
        { get; } = new List<string>();

        public List<Chunk> Chunks
        { get; } = new List<Chunk>();

        public class Chunk
        {
            public Chunk(int startLine, int numOfLines)
            {
                StartLine = startLine;
                NumOfLines = numOfLines;
            }

            public int StartLine
            { get; }

            public int NumOfLines
            { get; }
        }
    }
}