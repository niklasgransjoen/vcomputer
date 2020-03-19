using VComputer.Assembler.Text;

namespace VComputer.Assembler
{
    public sealed class Diagnostic
    {
        public Diagnostic(TextSpan span, string message)
        {
            Span = span;
            Message = message;
        }

        #region Properties

        public TextSpan Span { get; }
        public string Message { get; }

        #endregion Properties

        #region Methods

        public override string ToString() => Message;

        #endregion Methods
    }
}