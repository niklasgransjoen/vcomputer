﻿using System.Collections.Immutable;
using VComputer.Assembler.Text;

namespace VComputer.Assembler.Syntax
{
    internal sealed class SyntaxTree
    {
        #region Constructor

        private SyntaxTree(SourceText text)
        {
            Parser parser = new Parser(text);

            Text = text;
            Root = parser.ParseCompilationUnit();
            Diagnostics = ImmutableArray.CreateRange(parser.Diagnostics);
        }

        public static SyntaxTree Parse(string program)
        {
            var text = SourceText.From(program);
            return Parse(text);
        }

        public static SyntaxTree Parse(SourceText text) => new SyntaxTree(text);

        #endregion Constructor

        public SourceText Text { get; }
        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public CompilationUnitSyntax Root { get; }
    }
}