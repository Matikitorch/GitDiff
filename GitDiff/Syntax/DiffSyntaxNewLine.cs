using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDiff.Syntax
{
    public static class DiffSyntaxNewLine
    {
        public static string Prefix => "+";

        public static string Parse(string commitLine)
        {
            return commitLine.Substring(Prefix.Length).Trim();
        }
    }
}
