using System;
using System.Collections.Generic;
using Lizpy.Compiler;
using Lizpy.Syntax;
using ModApi.Craft.Program;
using ModApi.Craft.Program.Expressions;
using ExpressionType = Lizpy.Syntax.ExpressionType;

namespace Lizpy.Builtin {
    public class ConstantExpressionHandler : IExpressionHandler {
        private static readonly ExpressionType[] supportedExpressionTypes = {
            ExpressionType.Boolean,
            ExpressionType.Decimal,
            ExpressionType.Integer,
            ExpressionType.String
        };

        public IEnumerable<ExpressionType> SupportedTypes => supportedExpressionTypes;

        public ProgramExpression CompileExpression(CompilerState state, SExpression expression) {

            switch (expression) {
                case INumberExpression number:
                    return new ConstantExpression(number.AsDouble());
                case BooleanExpression boolExpression:
                    return new ConstantExpression(boolExpression.Value);
                case StringExpression stringExpression:
                    return new ConstantExpression(stringExpression.Value);
                default:
                    throw new NotSupportedException($"Unsupported expression type: {expression.Type}");
            }
        }
    }
}
