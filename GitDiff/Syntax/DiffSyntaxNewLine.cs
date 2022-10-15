using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDiff.Syntax
{
    public class DiffSyntaxNewLine : DiffSyntax
    {
        public DiffSyntaxNewLine()
            : base("+", false)
        { }

        public override void ParseSyntax(DiffInfo diffInfo, string commitLine)
        {
            if (string.IsNullOrEmpty(commitLine)) return;
            diffInfo.NewLines.Add(commitLine);
        }
    }
}
