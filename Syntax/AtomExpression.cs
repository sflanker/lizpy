using System;
using System.Collections.Generic;

namespace Lizpy.Syntax {
    [Serializable]
    public abstract class AtomExpression : SExpression {
        protected AtomExpression(Int32 startIndex, Int32 nextIndex) :
            base(startIndex, nextIndex) {
        }

        public abstract Object GetValue();
    }

    [Serializable]
    public abstract class AtomExpression<TValue> : AtomExpression {
        public TValue Value { get; }

        protected AtomExpression(Int32 startIndex, Int32 nextIndex, TValue value) :
            base(startIndex, nextIndex) {

            this.Value = value;
        }

        public override Object GetValue() {
            return this.Value;
        }

        public override String ToString() {
            return this.Value?.ToString() ?? "";
        }

        public override Boolean Equals(Object obj) {
            return this.Equals(obj as AtomExpression<TValue>);
        }

        public override int GetHashCode() {
            return EqualityComparer<TValue>.Default.GetHashCode(Value);
        }

        public static Boolean operator ==(AtomExpression<TValue> left, AtomExpression<TValue> right) {
            return Equals(left, right);
        }

        public static Boolean operator !=(AtomExpression<TValue> left, AtomExpression<TValue> right) {
            return !Equals(left, right);
        }

        public Boolean Equals(AtomExpression<TValue> otherAtom) {
            return !ReferenceEquals(otherAtom, null) && this.ValueEquals(this.Value, otherAtom.Value);
        }

        protected virtual Boolean ValueEquals(TValue value, TValue otherAtomValue) {
            return Object.Equals(value, otherAtomValue);
        }
    }
}
