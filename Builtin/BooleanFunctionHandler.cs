using System;
using System.Collections.Generic;
using Lizpy.Compiler;
using Lizpy.Internal;
using Lizpy.Syntax;
using ModApi.Craft.Program;
using ModApi.Craft.Program.Expressions;

namespace Lizpy.Builtin {
    public class BooleanFunctionHandler : IFunctionHandler {
        private static readonly String[] BooleanFunctionSymbols = {
            "and",
            "or",
            "not"
        };

        private static readonly IReadOnlyDictionary<String, String> BooleanFunctionStyleMapping =
            new Dictionary<String, String> {
                { "and", "op-and" },
                { "or", "op-or" },
                { "not", "op-not" },
            };

        public string Namespace => String.Empty;

        public IEnumerable<String> SupportedSymbols => BooleanFunctionSymbols;

        public ProgramNode CompileFunction(
            CompilerState state,
            ListExpression expression) {

            var op = LizpyCompiler.GetLocalName((SymbolExpression)expression.Items[0]);
            if (op == "not") {
                if (expression.Items.Count != 2) {
                    throw new LizpyInternalCompilationException(
                        $"Invalid function call, argument count mismatch. Expected: 1, Actual: {expression.Items.Count - 1}",
                        expression
                    );
                }

                var operation = new NotExpression {
                    Style = "op-not"
                };

                operation.InitializeExpressions(
                    state.CompileExpression(expression.Items[1])
                );

                return operation;
            } else {
                if (expression.Items.Count != 3) {
                    throw new LizpyInternalCompilationException(
                        $"Invalid function call, argument count mismatch. Expected: 2, Actual: {expression.Items.Count - 1}",
                        expression
                    );
                }

                var operation = new BoolOperatorExpression {
                    Operator = op,
                    Style = BooleanFunctionStyleMapping[op]
                };

                operation.InitializeExpressions(
                    state.CompileExpression(expression.Items[1]),
                    state.CompileExpression(expression.Items[2])
                );

                return operation;
            }
        }
    }
}
