using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDiff;

namespace GitDiff.Syntax
{
    public class DiffSyntaxFactory
    {
        public DiffSyntaxFactory(IEnumerable<DiffSyntax> diffSyntaxList)
        {
            DiffSyntaxList = new List<DiffSyntax>(diffSyntaxList);
        }

        private List<DiffSyntax> DiffSyntaxList
        { get; }

        public int Count
        { get { return DiffSyntaxList.Count; } }

        public void AddSyntax(DiffSyntax diffSyntax)
        {
            if (diffSyntax == null) return;
            DiffSyntaxList.Add(diffSyntax);
        }

        public bool MatchSyntax(ref bool isHeader, string commitLine, out DiffSyntax diffSyntax)
        {
            foreach (DiffSyntax syntax in DiffSyntaxList)
            {
                if (syntax.MatchesSyntax(ref isHeader, commitLine))
                {
                    diffSyntax = syntax;
                    return true;
                }
            }

            diffSyntax = null;
            return false;
        }

        public void Parse(DiffInfo diffInfo, string commitLine, DiffSyntax diffSyntax)
        {
            diffSyntax.ParseSyntax(diffInfo, commitLine);
        }
    }
}
