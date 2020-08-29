using System;
using System.Collections.Generic;
using Lizpy.Compiler;
using Lizpy.Internal;
using Lizpy.Syntax;
using ModApi.Craft.Program;
using ModApi.Craft.Program.Expressions;

namespace Lizpy.Builtin {
    public class UnaryMathFunctionHandler : IFunctionHandler {
        private static readonly String[] MathFunctionSymbols = {
            "abs",
            "floor",
            "ceiling",
            "round",
            "sqrt",
            "sin",
            "cos",
            "tan",
            "asin",
            "acos",
            "atan",
            "ln",
            "deg2rad",
            "rad2deg"
        };

        public string Namespace => String.Empty;

        public IEnumerable<String> SupportedSymbols => MathFunctionSymbols;

        public ProgramNode CompileFunction(
            CompilerState state,
            ListExpression expression) {

            if (expression.Items.Count != 2) {
                throw new LizpyInternalCompilationException(
                    $"Invalid function call, argument count mismatch. Expected: 1, Actual: {expression.Items.Count - 1}",
                    expression
                );
            }

            var function = LizpyCompiler.GetLocalName((SymbolExpression)expression.Items[0]);
            var operation = new MathFunctionExpression {
                FunctionName = function,
                Style = "op-math"
            };

            operation.InitializeExpressions(
                state.CompileExpression(expression.Items[1])
            );

            return operation;
        }
    }
}
