using System;
using System.Runtime.Serialization;
using Lizpy.Syntax;

namespace Lizpy.Internal {
    [Serializable]
    public class LizpyInternalCompilationException : Exception {
        public SExpression Expression { get; }

        public LizpyInternalCompilationException(
            String message,
            SExpression expression) : base(message) {
            this.Expression = expression;
        }

        public LizpyInternalCompilationException(
            String message,
            SExpression expression,
            Exception innerException) : base(message, innerException) {
            this.Expression = expression;
        }

        protected LizpyInternalCompilationException(SerializationInfo info, StreamingContext context) :
            base(info, context) {

            switch ((ExpressionType)info.GetValue("expressionType", typeof(ExpressionType))) {
                case ExpressionType.Symbol:
                    this.Expression = (SymbolExpression)info.GetValue("expression", typeof(SymbolExpression));
                    break;
                case ExpressionType.String:
                    this.Expression = (StringExpression)info.GetValue("expression", typeof(StringExpression));
                    break;
                case ExpressionType.Character:
                    this.Expression = (CharacterExpression)info.GetValue("expression", typeof(CharacterExpression));
                    break;
                case ExpressionType.Integer:
                    this.Expression = (IntegerExpression)info.GetValue("expression", typeof(IntegerExpression));
                    break;
                case ExpressionType.Decimal:
                    this.Expression = (DecimalExpression)info.GetValue("expression", typeof(DecimalExpression));
                    break;
                case ExpressionType.Boolean:
                    this.Expression = (BooleanExpression)info.GetValue("expression", typeof(BooleanExpression));
                    break;
                case ExpressionType.List:
                    this.Expression = (ListExpression)info.GetValue("expression", typeof(ListExpression));
                    break;
                case ExpressionType.Array:
                    this.Expression = (ArrayExpression)info.GetValue("expression", typeof(ArrayExpression));
                    break;
                case ExpressionType.Map:
                    this.Expression = (MapExpression)info.GetValue("expression", typeof(MapExpression));
                    break;
                case ExpressionType.Number:
                case ExpressionType.Atom:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            base.GetObjectData(info, context);

            info.AddValue("expressionType", this.Expression.Type);
            info.AddValue("expression", this.Expression);
        }
    }
}
