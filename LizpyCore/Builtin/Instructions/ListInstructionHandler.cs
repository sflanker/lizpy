using System;
using System.Collections.Generic;
using System.Linq;
using Lizpy.Compiler;
using Lizpy.Internal;
using Lizpy.Syntax;
using ModApi.Craft.Program.Expressions;
using ModApi.Craft.Program.Instructions;

namespace Lizpy.Builtin.Instructions {
    public class ListInstructionHandler : IInstructionFunctionHandler {
        private static readonly String[] ListInstructionSymbols = {
            "add!",
            "insert!",
            "remove!",
            "remove-at!",
            "set-at!",
            "clear!",
            "sort!",
            "reverse!"
        };

        public String Namespace => "list";
        public IEnumerable<String> SupportedSymbols => ListInstructionSymbols;

        private static readonly Dictionary<String, (String op, Int32[] args)> FunctionInfoMapping =
            new Dictionary<String, (String, Int32[])> {
                { "add!", ("add", new[] { 0, 1 }) },
                // Use clojure style (assoc list index item) order
                { "insert!", ("insert", new[] { 0, 2, 1 }) },
                { "remove-at!", ("remove", new[] { 0, 1 }) },
                { "set-at!", ("set", new[] { 0, 2, 1 }) },
                { "clear!", ("clear", new[] { 0 }) },
                { "sort!", ("sort", new[] { 0 }) },
                { "reverse!", ("reverse", new[] { 0 }) },
            };

        public ProgramInstruction[] CompileFunction(CompilerState state, ListExpression expression) {
            var function = ((SymbolExpression)expression.Items[0]).GetLocalName();
            if (FunctionInfoMapping.TryGetValue(function, out var opInfo)) {
                if (expression.Items.Count - 1 != opInfo.args.Length) {
                    throw new LizpyInternalCompilationException(
                        $"Invalid function call, argument count mismatch. Expected: {opInfo.args.Length}, Actual: {expression.Items.Count - 1}",
                        expression
                    );
                }

                var inst = new SetListInstruction {
                    Operator = opInfo.op,
                    Style = $"list-{opInfo.op}"
                };

                inst.InitializeExpressions(
                    opInfo.args.Select(i => state.CompileExpression(expression.Items[i + 1])).ToArray()
                );

                return new ProgramInstruction[] { inst };
            } else if (function == "remove!") {
                if (expression.Items.Count - 1 != 2) {
                    throw new LizpyInternalCompilationException(
                        $"Invalid function call, argument count mismatch. Expected: 2, Actual: {expression.Items.Count - 1}",
                        expression
                    );
                }

                var inst = new SetListInstruction {
                    Operator = "remove",
                    Style = "list-remove"
                };

                var indexExpression = new ListOperatorExpression {
                    Operator = "index",
                    Style = "list-index"
                };

                indexExpression.InitializeExpressions(
                    state.CompileExpression(expression.Items[2])
                );

                inst.InitializeExpressions(
                    state.CompileExpression(expression.Items[1]),
                    indexExpression
                );

                return new ProgramInstruction[] { inst };
            } else {
                throw new NotSupportedException($"Unsupported Symbol {expression.Items[0]}");
            }
        }
    }
}
