using System;

namespace Lizpy.Syntax {
    [Serializable]
    public abstract class NumberExpression<TNumber> :
        AtomExpression<TNumber>, INumberExpression where TNumber : struct {

        protected NumberExpression(Int32 startIndex, Int32 nextIndex, TNumber value) :
            base(startIndex, nextIndex, value) {
        }

        public abstract double AsDouble();

        public abstract int AsInteger();
    }
}
