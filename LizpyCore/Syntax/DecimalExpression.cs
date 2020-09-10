using System;

namespace Lizpy.Syntax {
    [Serializable]
    public class DecimalExpression : NumberExpression<Double> {
        public override ExpressionType Type => ExpressionType.Decimal;

        public DecimalExpression(Int32 startIndex, Int32 nextIndex, Double value) :
            base(startIndex, nextIndex, value) {
        }

        public override string ToString() {
            return $"{this.Value:R}";
        }

        public override Double AsDouble() {
            return this.Value;
        }

        public override Int32 AsInteger() {
            return (Int32)Math.Round(this.Value);
        }
    }
}
