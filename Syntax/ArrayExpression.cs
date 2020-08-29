using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Lizpy.Syntax {
    [Serializable]
    public class ArrayExpression : SExpression {
        public IReadOnlyList<AtomExpression> Items { get; }

        public override ExpressionType Type => ExpressionType.Array;

        public ArrayExpression(
            Int32 startIndex,
            Int32 nextIndex,
            IEnumerable<AtomExpression> items) :
            base(startIndex, nextIndex) {

            this.Items = new ReadOnlyCollection<AtomExpression>(items.ToList());
        }

        public override string ToString() {
            return $"[{String.Join(", ", this.Items)}]";
        }
    }
}
