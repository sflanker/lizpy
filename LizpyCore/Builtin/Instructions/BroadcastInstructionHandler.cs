using System;
using System.Collections.Generic;
using System.Linq;
using Lizpy.Compiler;
using Lizpy.Internal;
using Lizpy.Syntax;
using ModApi.Craft.Program.Instructions;

namespace Lizpy.Builtin.Instructions {
    public class BroadcastInstructionHandler : IInstructionFunctionHandler {
        private static readonly String[] BroadcastInstructionSymbols = {
            "broadcast!",
            "broadcast-to-craft!"
        };

        public String Namespace => String.Empty;
        public IEnumerable<String> SupportedSymbols => BroadcastInstructionSymbols;

        public ProgramInstruction[] CompileFunction(
            CompilerState state,
            ListExpression expression) {

            if (expression.Items.Count != 3) {
                throw new LizpyInternalCompilationException(
                    $"Invalid function call, argument count mismatch. Expected: 2, Actual: {expression.Items.Count - 1}",
                    expression
                );
            }

            var function = ((SymbolExpression)expression.Items[0]).GetLocalName();

            String style;

            switch (function) {
                case "broadcast!":
                    style = "broadcast-msg";
                    break;
                case "broadcast-to-craft!":
                    style = "broadcast-msg-craft";
                    break;
                default:
                    throw new NotSupportedException($"Unsupported Symbol {expression.Items[0]}");
            }

            var instruction = new BroadcastMessageInstruction {
                Style = style
            };

            instruction.InitializeExpressions(
                expression.Items.Skip(1).Select(state.CompileExpression).ToArray()
            );

            return new ProgramInstruction[] { instruction };
        }
    }
}
