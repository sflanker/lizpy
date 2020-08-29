using System;
using System.Collections.Generic;
using System.Linq;
using Lizpy.Compiler;
using Lizpy.Internal;
using Lizpy.Syntax;
using ModApi.Craft.Program;

namespace Lizpy.Builtin {
    public class StringFunctionHandler : IFunctionHandler {
        private static readonly String[] StringFunctionSymbols = {
            "string=",
            "string-contains",
            "string-length",
            "letter",
            "substring",
            "concat",
            "format"
        };

        private static readonly IReadOnlyDictionary<String, Int32> FunctionArgumentCounts =
            new Dictionary<String, Int32> {
                { "string=", 2 },
                { "string-contains", 2 },
                { "string-length", 1 },
                { "letter", 2 },
            };

        public string Namespace => String.Empty;

        public IEnumerable<String> SupportedSymbols => StringFunctionSymbols;

        public ProgramNode CompileFunction(
            CompilerState state,
            ListExpression expression) {
            var function = LizpyCompiler.GetLocalName((SymbolExpression)expression.Items[0]);

            if (FunctionArgumentCounts.TryGetValue(function, out var argCount)) {
                if (expression.Items.Count != argCount + 1) {
                    throw new LizpyInternalCompilationException(
                        $"Invalid function call, argument count mismatch. Expected: {argCount}, Actual: {expression.Items.Count - 1}",
                        expression
                    );
                }
            }

            switch (function) {
                case "string=":
                    return VizzyEx.StringEqual(
                        state.CompileExpression(expression.Items[1]),
                        state.CompileExpression(expression.Items[2])
                    );
                case "string-contains":
                    return Vizzy.Contains(
                        state.CompileExpression(expression.Items[1]),
                        state.CompileExpression(expression.Items[2])
                    );
                case "string-length":
                    return Vizzy.StringLength(
                        state.CompileExpression(expression.Items[1])
                    );
                case "letter":
                    return Vizzy.GetLetter(
                        state.CompileExpression(expression.Items[1]),
                        state.CompileExpression(expression.Items[2])
                    );
                case "substring":
                    var str = state.CompileExpression(expression.Items[1]);
                    var start = state.CompileExpression(expression.Items[2]);
                    switch (expression.Items.Count) {
                        case 4:
                            return Vizzy.SubString(
                                str,
                                start,
                                state.CompileExpression(expression.Items[3])
                            );
                        case 3:
                            return Vizzy.SubString(
                                str,
                                start,
                                Vizzy.StringLength(str)
                            );
                        default:
                            throw new LizpyInternalCompilationException(
                                $"Invalid function call, argument count mismatch. Expected: 2-3, Actual: {expression.Items.Count - 1}",
                                expression
                            );
                    }
                case "concat":
                    return Vizzy.Join(
                        expression.Items.Skip(1).Select(state.CompileExpression).ToArray()
                    );
                case "format":
                    return Vizzy.Format(
                        expression.Items.Skip(1).Select(state.CompileExpression).ToArray()
                    );
                default:
                    throw new NotSupportedException($"Unsupported Symbol {expression.Items[0]}");
            }
        }
    }
}
