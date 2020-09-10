using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Lizpy.Syntax {
    [Serializable]
    public class ListExpression : SExpression {
        public IReadOnlyList<SExpression> Items { get; }

        public override ExpressionType Type => ExpressionType.List;

        public ListExpression(Int32 startIndex, Int32 nextIndex, IEnumerable<SExpression> items) :
            base(startIndex, nextIndex) {

            this.Items = new ReadOnlyCollection<SExpression>(items.ToList());
        }

        public override string ToString() {
            return $"({String.Join(" ", this.Items)})";
        }
    }
}
