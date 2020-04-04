using System.Collections.Generic;

namespace VComputer.Assembler.Binding
{
    internal abstract class BoundNode
    {
        public abstract BoundNodeKind Kind { get; }

        #region Methods

        public abstract IEnumerable<BoundNode> GetChildren();

        #endregion Methods
    }
}