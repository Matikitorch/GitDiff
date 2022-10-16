using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDiff;
using GitDiff.Syntax;

namespace GitDiff.Smells
{
    public abstract class CodeSmell
    {
        public CodeSmell(CodeSmellSeverity severity)
        {
            Severity = severity;
        }

        public abstract string Title
        { get; }

        public abstract string Description
        { get; }

        public abstract string Suggestion
        { get; }

        public CodeSmellSeverity Severity
        { get; }

        public abstract CodeSmellResult Analyze(DiffInfoCommit diffInfoCommit);
    }

    public enum CodeSmellSeverity
    {
        Review,
        Warning,
        Severe,
    }
}