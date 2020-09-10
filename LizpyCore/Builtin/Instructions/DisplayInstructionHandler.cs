using System;
using System.Collections.Generic;
using Lizpy.Compiler;
using Lizpy.Internal;
using Lizpy.Syntax;
using ModApi.Craft.Program.Instructions;

namespace Lizpy.Builtin.Instructions {
    public class DisplayInstructionHandler : IInstructionFunctionHandler {
        private static readonly String[] DisplayInstructionSymbols = { "display" };

        public String Namespace => String.Empty;
        public IEnumerable<String> SupportedSymbols => DisplayInstructionSymbols;

        public ProgramInstruction[] CompileFunction(
            CompilerState state,
            ListExpression expression) {

            if (expression.Items.Count < 2) {
                throw new LizpyInternalCompilationException(
                    $"Invalid function call, argument count mismatch. Expected: 1 - 2, Actual: {expression.Items.Count - 1}",
                    expression
                );
            } else if (expression.Items.Count > 3) {
                throw new LizpyInternalCompilationException(
                    $"Invalid function call, argument count mismatch. Expected: 1 - 2, Actual: {expression.Items.Count - 1}",
                    expression
                );
            }

            var displayInstruction = new DisplayMessageInstruction {
                Style = "display"
            };

            if (expression.Items.Count == 2) {
                displayInstruction.InitializeExpressions(
                    state.CompileExpression(expression.Items[1])
                );
            } else {
                displayInstruction.InitializeExpressions(
                    state.CompileExpression(expression.Items[1]),
                    state.CompileExpression(expression.Items[2])
                );
            }

            return new ProgramInstruction[] { displayInstruction };
        }
    }
}
