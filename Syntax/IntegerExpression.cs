using System;

namespace Lizpy.Syntax {
    [Serializable]
    public class IntegerExpression : NumberExpression<Int32> {
        public override ExpressionType Type => ExpressionType.Integer;

        public IntegerExpression(Int32 startIndex, Int32 nextIndex, Int32 value) :
            base(startIndex, nextIndex, value) {
        }

        public override Double AsDouble() {
            return this.Value;
        }

        public override Int32 AsInteger() {
            return this.Value;
        }
    }
}
