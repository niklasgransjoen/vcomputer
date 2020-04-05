using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace VComputer.Assembler.Symbols
{
    internal sealed class DirectiveSymbol : Symbol
    {
        public static DirectiveSymbol ORG = new DirectiveSymbol(".org", true);
        public static DirectiveSymbol WORD = new DirectiveSymbol(".word", true);

        /// <summary>
        /// A no-operation directive, for internal use. Not a valid directive in user code.
        /// </summary>
        public static DirectiveSymbol NOOP = new DirectiveSymbol(".noop", false);

        /// <summary>
        /// Gets an array containing all directives.
        /// </summary>
        public static ImmutableArray<DirectiveSymbol> Directives { get; } = ImmutableArray.CreateRange(
            typeof(DirectiveSymbol)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(field => field.FieldType == typeof(DirectiveSymbol))
                .Select(field => (DirectiveSymbol)field.GetValue(null)!)
                .Where(directive => directive.Name != "noop")
            );

        private DirectiveSymbol(string name, bool hasOperand) : base(name)
        {
            HasOperand = hasOperand;
        }

        public bool HasOperand { get; }
    }
}