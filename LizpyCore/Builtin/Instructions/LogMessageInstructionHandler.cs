using System;
using System.Collections.Generic;
using Lizpy.Compiler;
using Lizpy.Internal;
using Lizpy.Syntax;
using ModApi.Craft.Program.Instructions;

namespace Lizpy.Builtin.Instructions {
    public class LogMessageInstructionHandler : IInstructionFunctionHandler {
        private static readonly String[] LogMessageInstructionSymbols = { "log" };
        public String Namespace => "console";

        public IEnumerable<String> SupportedSymbols => LogMessageInstructionSymbols;

        public ProgramInstruction[] CompileFunction(
            CompilerState state,
            ListExpression expression) {

            if (expression.Items.Count != 2) {
                throw new LizpyInternalCompilationException(
                    $"Invalid function call, argument count mismatch. Expected: 1, Actual: {expression.Items.Count - 1}",
                    expression
                );
            }

            var logInstruction = new LogMessageInstruction {
                Style = "log"
            };

            logInstruction.InitializeExpressions(
                state.CompileExpression(expression.Items[1])
            );

            return new ProgramInstruction[] { logInstruction };
        }
    }
}
