using System.Collections.Generic;
using System.Linq;
using VComputer.Assembler.Text;

namespace VComputer.Assembler.Syntax
{
    internal abstract class SyntaxNode
    {
        public abstract SyntaxKind Kind { get; }

        public virtual TextSpan Span
        {
            get
            {
                TextSpan first = GetChildren().First().Span;
                TextSpan last = GetChildren().Last().Span;

                return TextSpan.FromBounds(first.Start, last.End);
            }
        }

        #region Methods

        public abstract IEnumerable<SyntaxNode> GetChildren();

        #endregion Methods
    }
}