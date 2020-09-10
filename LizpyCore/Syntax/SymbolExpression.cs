using System;
using System.Collections.Generic;

namespace Lizpy.Syntax {
    [Serializable]
    public class SymbolExpression : AtomExpression<String> {
        public override ExpressionType Type => ExpressionType.Symbol;

        public IReadOnlyDictionary<AtomExpression, SExpression> Metadata { get; }

        public SymbolExpression(
            Int32 startIndex,
            Int32 nextIndex,
            String value,
            MapExpression metadata = null) :
            base(startIndex, nextIndex, value) {
            this.Metadata = metadata?.Data;
        }

        public SymbolExpression(
            String value,
            MapExpression metadata = null) :
            base(0, 0, value) {
            this.Metadata = metadata?.Data;
        }

        public override string ToString() {
            return this.Metadata != null ?
                $"{this.Value} ^{new MapExpression(this.Metadata)}" :
                this.Value;
        }
    }
}
