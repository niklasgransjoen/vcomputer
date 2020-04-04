using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace VComputer.Assembler
{
    internal sealed class Directive
    {
        public static Directive ORG = new Directive("org", true);
        public static Directive WORD = new Directive("word", true);

        /// <summary>
        /// A no-operation directive, for internal use. Not a valid directive in user code.
        /// </summary>
        public static Directive NOOP = new Directive("noop", false);

        /// <summary>
        /// Gets an array containing all directives.
        /// </summary>
        public static ImmutableArray<Directive> Directives { get; } = ImmutableArray.CreateRange(
            typeof(Directive)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(field => field.FieldType == typeof(Directive))
                .Select(field => (Directive)field.GetValue(null)!)
                .Where(directive => directive.Name != "noop")
            );

        private Directive(string name, bool hasOperand)
        {
            Name = name;
            HasOperand = hasOperand;
        }

        public string Name { get; }
        public bool HasOperand { get; }
    }
}