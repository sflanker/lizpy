using System;

namespace Lizpy.Syntax {
    [Serializable]
    public class BooleanExpression : AtomExpression<Boolean> {
        public override ExpressionType Type => ExpressionType.Boolean;

        public BooleanExpression(Int32 startIndex, Int32 nextIndex, Boolean value) :
            base(startIndex, nextIndex, value) {
        }
    }
}
