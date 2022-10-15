using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDiff.Syntax
{
    public abstract class DiffSyntax
    {
        public DiffSyntax(string prefix, bool isHeader, bool allowHeaderChange = false)
        {
            Prefix = prefix;
            IsHeader = isHeader;
            AllowHeaderChange = allowHeaderChange;
        }

        public string Prefix
        { get; }

        public bool IsHeader
        { get; }

        public bool AllowHeaderChange
        { get; }

        public bool MatchesSyntax(ref bool isHeader, string commitLine)
        {
            bool tempHeader = isHeader;
                
            if (IsHeader != isHeader)
            {
                if (!AllowHeaderChange) return false;
                tempHeader = !isHeader;
            }

            if (commitLine.StartsWith(Prefix + (tempHeader ? " " : "")))
            {
                isHeader = tempHeader;
                return true;
            }

            return false;
        }

        public void ParseCommit(DiffInfo diffInfo, string commitLine)
        {
            commitLine = commitLine.Substring(Prefix.Length).Trim();

            ParseSyntax(diffInfo, commitLine);
        }

        public abstract void ParseSyntax(DiffInfo diffInfo, string commitLine);
    }
}