using System;
using System.Collections.Generic;
using Lizpy.Compiler;
using Lizpy.Internal;
using Lizpy.Syntax;
using ModApi.Craft.Program;
using ModApi.Craft.Program.Expressions;

namespace Lizpy.Builtin.Expressions {
    public class BinaryMathFunctionHandler : IExpressionFunctionHandler {
        private static readonly String[] MathFunctionSymbols = {
            "+",
            "-",
            "/",
            "*",
            "^",
            "%",
            "rand",
            "min",
            "max",
            "atan2"
        };

        private static readonly IReadOnlyDictionary<String, String> MathFunctionStyleMapping =
            new Dictionary<String, String> {
                { "+", "op-add" },
                { "-", "op-sub" },
                { "/", "op-div" },
                { "*", "op-mul" },
                { "^", "op-exp" },
                { "%", "op-mod" },
                { "rand", "op-rand" },
                { "min", "op-min" },
                { "max", "op-max" },
                { "atan2", "op-atan-2" }
            };

        public String Namespace => String.Empty;

        public IEnumerable<String> SupportedSymbols => MathFunctionSymbols;

        public ProgramExpression CompileFunction(
            CompilerState state,
            ListExpression expression) {

            if (expression.Items.Count != 3) {
                throw new LizpyInternalCompilationException(
                    $"Invalid function call, argument count mismatch. Expected: 2, Actual: {expression.Items.Count - 1}",
                    expression
                );
            }

            var op = ((SymbolExpression)expression.Items[0]).GetLocalName();
            var operation = new BinaryOperatorExpression {
                Operator = op,
                Style = MathFunctionStyleMapping[op]
            };

            operation.InitializeExpressions(
                state.CompileExpression(expression.Items[1]),
                state.CompileExpression(expression.Items[2])
            );

            return operation;
        }
    }
}
