using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDiff.Syntax
{
    public class DiffSyntaxDeletedFileMode : DiffSyntax
    {
        public DiffSyntaxDeletedFileMode()
            : base("deleted file", true)
        { }

        public override void ParseSyntax(DiffInfo diffInfo, string commitLine)
        {
            return;
        }
    }
}