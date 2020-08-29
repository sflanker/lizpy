using System;
using ModApi.Craft.Program;

namespace Lizpy.Syntax {
    [Serializable]
    public abstract class SExpression {
        public Int32 StartIndex { get; }
        public Int32 NextIndex { get; }

        public abstract ExpressionType Type { get; }

        protected SExpression(Int32 startIndex, Int32 nextIndex) {
            this.StartIndex = startIndex;
            this.NextIndex = nextIndex;
        }
    }
}
