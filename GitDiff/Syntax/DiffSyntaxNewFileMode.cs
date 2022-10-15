using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDiff.Syntax
{
    public class DiffSyntaxNewFileMode : DiffSyntax
    {
        public DiffSyntaxNewFileMode()
            : base("new file", true)
        { }

        public override void ParseSyntax(DiffInfo diffInfo, string commitLine)
        {
            return;
        }
    }
}
