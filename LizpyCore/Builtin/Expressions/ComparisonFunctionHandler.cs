using System;
using System.Collections.Generic;
using Lizpy.Compiler;
using Lizpy.Internal;
using Lizpy.Syntax;
using ModApi.Craft.Program;
using ModApi.Craft.Program.Expressions;

namespace Lizpy.Builtin.Expressions {
    public class ComparisonFunctionHandler : IExpressionFunctionHandler {
        private static readonly String[] ComparisonFunctionSymbols = {
            "=",
            "!=",
            "<",
            "<=",
            ">",
            ">="
        };

        private static readonly IReadOnlyDictionary<String, String> ComparisonOpStyleMapping =
            new Dictionary<String, String> {
                { "=", "op-eq" },
                { "l", "op-lt" },
                { "le", "op-lte" },
                { "g", "op-gt" },
                { "ge", "op-gte" },
            };

        public String Namespace => String.Empty;

        public IEnumerable<String> SupportedSymbols => ComparisonFunctionSymbols;

        public ProgramExpression CompileFunction(
            CompilerState state,
            ListExpression expression) {
            if (expression.Items.Count != 3) {
                throw new LizpyInternalCompilationException(
                    $"Invalid function call, argument count mismatch. Expected: 2, Actual: {expression.Items.Count - 1}",
                    expression
                );
            }

            String op;
            var negate = false;
            switch (((SymbolExpression)expression.Items[0]).GetLocalName()) {
                case "=":
                    op = "\"";
                    break;
                case "!=":
                    negate = true;
                    op = "\"";
                    break;
                case "<":
                    op = "l";
                    break;
                case "<=":
                    op = "le";
                    break;
                case ">":
                    op = "g";
                    break;
                case ">=":
                    op = "ge";
                    break;
                default:
                    throw new NotSupportedException($"Unsupported Symbol {expression.Items[0]}");
            }

            var comparer = new ComparisonExpression {
                Operator = op,
                Style = ComparisonOpStyleMapping[op]
            };

            comparer.InitializeExpressions(
                state.CompileExpression(expression.Items[1]),
                state.CompileExpression(expression.Items[2])
            );

            if (negate) {
                var negation = new NotExpression {
                    Style = "op-not"
                };

                negation.InitializeExpressions(comparer);
                return negation;
            } else {
                return comparer;
            }
        }
    }
}
