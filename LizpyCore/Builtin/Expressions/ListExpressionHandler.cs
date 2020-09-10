using System;
using System.Collections.Generic;
using System.Linq;
using Lizpy.Compiler;
using Lizpy.Internal;
using Lizpy.Syntax;
using ModApi.Craft.Program;
using ModApi.Craft.Program.Expressions;

namespace Lizpy.Builtin.Expressions {
    public class ListExpressionHandler : IExpressionFunctionHandler {
        private static readonly String[] ListExpressionSymbols = {
            "create",
            "get-item",
            "index-of",
            "length"
        };

        private static readonly Dictionary<String, (String op, Int32 argCount)> FunctionInfoMapping =
            new Dictionary<String, (String, Int32)> {
                { "create", ("create", 1) },
                { "get-item", ("get", 2) },
                { "index-of", ("index", 2) },
                { "length", ("length", 1) },
            };

        public string Namespace => "list";
        public IEnumerable<string> SupportedSymbols => ListExpressionSymbols;

        public ProgramExpression CompileFunction(CompilerState state, ListExpression expression) {
            var function = ((SymbolExpression)expression.Items[0]).GetLocalName();
            if (FunctionInfoMapping.TryGetValue(function, out var opInfo)) {
                if (expression.Items.Count - 1 != opInfo.argCount) {
                    throw new LizpyInternalCompilationException(
                        $"Invalid function call, argument count mismatch. Expected: {opInfo.argCount}, Actual: {expression.Items.Count - 1}",
                        expression
                    );
                }

                var op = new ListOperatorExpression {
                    Operator = opInfo.op,
                    Style = $"list-{opInfo.op}"
                };

                op.InitializeExpressions(
                    expression.Items.Skip(1).Select(state.CompileExpression).ToArray()
                );

                return op;
            } else {
                throw new NotSupportedException($"Unsupported Symbol {expression.Items[0]}");
            }
        }
    }
}
