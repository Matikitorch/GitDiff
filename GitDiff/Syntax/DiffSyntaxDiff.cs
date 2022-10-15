using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDiff.Syntax
{
    public class DiffSyntaxDiff : DiffSyntax
    {
        public DiffSyntaxDiff()
            : base("diff", true, true)
        { }

        public override void ParseSyntax(DiffInfo diffInfo, string commitLine)
        {
            string oldFile, newFile;
            int oldFileIndex, newFileIndex;

            oldFileIndex = commitLine.IndexOf("a/");
            if (oldFileIndex < 0) throw new ArgumentException("Failed to find \"a/\" in \"" + commitLine + "\"");

            newFileIndex = commitLine.IndexOf("b/");
            if (newFileIndex < 0) throw new ArgumentException("Failed to find \"b/\" in \"" + commitLine + "\"");

            oldFile = commitLine.Substring(oldFileIndex + 2, newFileIndex - oldFileIndex - 2);
            newFile = commitLine.Substring(newFileIndex + 2);

            diffInfo.OldFile = oldFile;
            diffInfo.NewFile = newFile;
        }
    }
}
