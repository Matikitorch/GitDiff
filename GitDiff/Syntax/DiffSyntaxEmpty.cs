using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDiff.Syntax
{
    public class DiffSyntaxEmpty : DiffSyntax
    {
        public DiffSyntaxEmpty()
            : base(" ", false)
        { }

        public override void ParseSyntax(DiffInfo diffInfo, string commitLine)
        {
            return;
        }
    }
}
