using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDiff.Syntax
{
    public class DiffSyntaxChunk : DiffSyntax
    {
        public DiffSyntaxChunk()
            : base("@@", false, true)
        { }

        public override void ParseSyntax(DiffInfo diffInfo, string commitLine)
        {
            string startLine, numOfLines;
            int index;

            while (commitLine.Length > 0)
            {
                if (commitLine.StartsWith(Prefix)) break;
                if (!commitLine.StartsWith("-") && !commitLine.StartsWith("+")) throw new ArgumentException("Failed to find \"-\" or \"+\" in \"" + commitLine + "\"");

                index = commitLine.IndexOf(",");
                startLine = commitLine.Substring(1, index - 1);

                commitLine = commitLine.Substring(index + 1);

                index = commitLine.IndexOf(" ");
                numOfLines = commitLine.Substring(0, index);

                diffInfo.Chunks.Add(new DiffInfo.Chunk(int.Parse(startLine), int.Parse(numOfLines)));

                commitLine = commitLine.Substring(index + 1);
            }
        }
    }
}
