using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDiff.Syntax
{
    public class DiffSyntaxOldLine : DiffSyntax
    {
        public DiffSyntaxOldLine()
            : base("-", false)
        { }

        public override void ParseSyntax(DiffInfo diffInfo, string commitLine)
        {
            if (string.IsNullOrEmpty(commitLine)) return;
            diffInfo.OldLines.Add(commitLine);
        }
    }
}
