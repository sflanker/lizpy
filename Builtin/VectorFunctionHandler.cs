using System;
using System.Collections.Generic;
using System.Linq;
using Lizpy.Compiler;
using Lizpy.Internal;
using Lizpy.Syntax;
using ModApi.Craft.Program;
using ModApi.Craft.Program.Expressions;

namespace Lizpy.Builtin {
    public class VectorFunctionHandler : IFunctionHandler {
        private static readonly String[] VectorFunctionSymbols = {
            "vector",
            "vector-length",
            "vector-x",
            "vector-y",
            "vector-z",
            "dot",
            "cross",
            "angle",
            "dist",
            "clamp",
            "normalize",
            "project",
            "vector-max",
            "vector-min"
        };

        private static readonly IReadOnlyDictionary<String, (String op, Int32 argCount)> FunctionOpMapping =
            new Dictionary<String, (String, Int32)> {
                { "vector-length", ("length", 1) },
                { "x", ("length", 1) },
                { "y", ("length", 1) },
                { "z", ("length", 1) },
                { "dot", ("dot", 2) },
                { "cross", ("cross", 2) },
                { "angle", ("angle", 2) },
                { "dist", ("dist", 2) },
                { "clamp", ("clamp", 2) },
                { "normalize", ("normalize", 2) },
                { "project", ("project", 2) },
                { "vector-max", ("vector-max", 2) },
                { "vector-min", ("vector-min", 2) },
            };

        public string Namespace => String.Empty;

        public IEnumerable<String> SupportedSymbols => VectorFunctionSymbols;

        public ProgramNode CompileFunction(
            CompilerState state,
            ListExpression expression) {

            var function = LizpyCompiler.GetLocalName((SymbolExpression)expression.Items[0]);

            if (function == "vector") {
                if (expression.Items.Count != 4) {
                    throw new LizpyInternalCompilationException(
                        $"Invalid vector expression, argument count mismatch. Expected: 3, Actual: {expression.Items.Count - 1}",
                        expression
                    );
                }

                return Vizzy.Vector(
                    state.CompileExpression(expression.Items[1]),
                    state.CompileExpression(expression.Items[2]),
                    state.CompileExpression(expression.Items[3])
                );
            } else if (FunctionOpMapping.TryGetValue(function, out var opInfo)) {
                if (expression.Items.Count != opInfo.argCount + 1) {
                    throw new LizpyInternalCompilationException(
                        $"Invalid function call, argument count mismatch. Expected: {opInfo.argCount}, Actual: {expression.Items.Count - 1}",
                        expression
                    );
                }

                var op = new VectorOperatorExpression {
                    Operator = opInfo.op,
                    Style = $"vec-op-{opInfo.argCount}"
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
