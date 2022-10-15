using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDiff.Syntax
{
    public class DiffSyntaxIndex : DiffSyntax
    {
        public DiffSyntaxIndex()
            : base("index", true)
        { }

        public override void ParseSyntax(DiffInfo diffInfo, string commitLine)
        {
            int index;

            index = commitLine.IndexOf("..");
            if (index < 0) throw new ArgumentException("Failed to find \"..\" in \"" + commitLine + "\"");
            diffInfo.OldIndex = commitLine.Substring(0, index);

            commitLine = commitLine.Substring(index + 2);

            index = commitLine.IndexOf(" ");
            if (index < 0)
            {
                diffInfo.NewIndex = commitLine;
            }
            else
            {
                diffInfo.NewIndex = commitLine.Substring(0, index);
            }
        }
    }
}