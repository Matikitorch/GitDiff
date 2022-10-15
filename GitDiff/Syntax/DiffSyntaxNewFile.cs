using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDiff.Syntax
{
    public class DiffSyntaxNewFile : DiffSyntax
    {
        public DiffSyntaxNewFile()
            : base("+++", true)
        { }

        public override void ParseSyntax(DiffInfo diffInfo, string commitLine)
        {
            int index;

            index = commitLine.IndexOf("b/");
            if (index < 0)
            {
                diffInfo.NewFile = "NULL";
            }
            else
            {
                diffInfo.NewFile = commitLine.Substring(index + 2);
            }
        }
    }
}
