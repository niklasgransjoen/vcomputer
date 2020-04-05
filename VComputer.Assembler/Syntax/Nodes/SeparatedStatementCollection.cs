using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace VComputer.Assembler.Syntax
{
    internal sealed class SeparatedStatementCollection<T> : IReadOnlyList<T>
        where T : StatementSyntax
    {
        /// <summary>
        /// An empty collection with no separators.
        /// </summary>
        public static SeparatedStatementCollection<T> Empty = new SeparatedStatementCollection<T>(ImmutableArray<SyntaxNode>.Empty);

        private readonly ImmutableArray<SyntaxNode> _statementsAndSeparators;
        private readonly SeparatorOptions _options;
        private readonly int _separatorCount;
        private readonly int _period;

        public SeparatedStatementCollection(
            ImmutableArray<SyntaxNode> statementsAndSeparators,
            SeparatorOptions options = SeparatorOptions.None,
            int separatorCount = 1)
        {
            _statementsAndSeparators = statementsAndSeparators;
            _options = options;
            _separatorCount = separatorCount;
            _period = separatorCount + 1;
        }

        public T this[int index]
        {
            get
            {
                int offset = _options.HasFlag(SeparatorOptions.HasLeadingSeparator) ? _separatorCount : 0;
                return (T)_statementsAndSeparators[index * _period + offset];
            }
        }

        public int Count
        {
            get
            {
                int leadingAndTrailingSeparatorCount =
                    (_options.HasFlag(SeparatorOptions.HasLeadingSeparator) ? _separatorCount : 0) +
                    (_options.HasFlag(SeparatorOptions.HasTrailingSeparator) ? _separatorCount : 0);

                return (_statementsAndSeparators.Length - leadingAndTrailingSeparatorCount + _separatorCount) / _period;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}