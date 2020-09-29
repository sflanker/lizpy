using System;
using System.Collections.Generic;
using System.Linq;
using Lizpy.Compiler;
using Lizpy.Internal;
using Lizpy.Syntax;
using ModApi.Craft.Program;
using ModApi.Craft.Program.Expressions;

namespace Lizpy.Builtin.Expressions {
    public class BooleanFunctionHandler : IExpressionFunctionHandler {
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

        public String Namespace => String.Empty;

        public IEnumerable<String> SupportedSymbols => BooleanFunctionSymbols;

        public ProgramExpression CompileFunction(
            CompilerState state,
            ListExpression expression) {

            var op = ((SymbolExpression)expression.Items[0]).GetLocalName();
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
                if (expression.Items.Count < 3) {
                    throw new LizpyInternalCompilationException(
                        $"Invalid function call, argument count mismatch. Expected: 2+, Actual: {expression.Items.Count - 1}",
                        expression
                    );
                }

                var operation =
                    CreateBoolOperatorExpression(
                        state,
                        op,
                        BooleanFunctionStyleMapping[op],
                        expression.Items.Skip(1).ToList()
                    );

                return operation;
            }
        }

        private static BoolOperatorExpression CreateBoolOperatorExpression(
            CompilerState state, String op, String style, IReadOnlyList<SExpression> tmp) {

            var operation = new BoolOperatorExpression {
                Operator = op,
                Style = style
            };

            operation.InitializeExpressions(
                state.CompileExpression(tmp[0]),
                tmp.Count == 2 ?
                    state.CompileExpression(tmp[1]) :
                    CreateBoolOperatorExpression(state, op, style, tmp.Skip(1).ToList())
            );

            return operation;
        }
    }
}
