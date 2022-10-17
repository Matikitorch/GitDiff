using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDiff.Syntax
{
    public static class DiffSyntaxNewFile
    {
        public static string Prefix => "+++";

        public static string Parse(string commitLine)
        {
            int idx;

            idx = commitLine.IndexOf("b/");
            if (idx < 0) return "NULL";

            return commitLine.Substring(idx + 2).Trim();
        }
    }
}
