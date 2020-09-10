using System;
using System.Collections.Generic;
using Lizpy.Compiler;
using Lizpy.Internal;
using Lizpy.Syntax;
using ModApi.Craft.Program;

namespace Lizpy.Builtin.Expressions {
    public class NewVectorFunctionHandler : IExpressionFunctionHandler {
        private static readonly String[] VectorFunctionSymbols = { "vector" };

        public String Namespace => String.Empty;

        public IEnumerable<String> SupportedSymbols => VectorFunctionSymbols;

        public ProgramExpression CompileFunction(
            CompilerState state,
            ListExpression expression) {

            var function = ((SymbolExpression)expression.Items[0]).GetLocalName();

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
            } else {
                throw new NotSupportedException($"Unsupported Symbol {expression.Items[0]}");
            }
        }
    }
}
