using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDiff.Syntax
{
    public class DiffSyntaxOldFile : DiffSyntax
    {
        public DiffSyntaxOldFile()
            : base("---", true)
        { }

        public override void ParseSyntax(DiffInfo diffInfo, string commitLine)
        {
            int index;

            index = commitLine.IndexOf("a/");
            if (index < 0)
            {
                diffInfo.OldFile = "NULL";
            }
            else
            {
                diffInfo.OldFile = commitLine.Substring(index + 2);
            }
        }
    }
}
