using System.Collections.Generic;
using System.Linq;
using VComputer.Assembler.Text;

namespace VComputer.Assembler.Syntax
{
    internal sealed class SyntaxToken : SyntaxNode
    {
        public SyntaxToken(SyntaxKind kind, int position, string text, object? value)
        {
            Kind = kind;
            Position = position;
            Text = text;
            Value = value;
        }

        #region Properties

        public override SyntaxKind Kind { get; }

        public int Position { get; }
        public string Text { get; }
        public object? Value { get; }
        public override TextSpan Span => new TextSpan(Position, Text.Length);

        #endregion Properties

        #region Methods

        public override IEnumerable<SyntaxNode> GetChildren() => Enumerable.Empty<SyntaxNode>();

        #endregion Methods
    }
}