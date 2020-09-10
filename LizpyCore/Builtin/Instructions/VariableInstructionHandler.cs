using System;
using System.Collections.Generic;
using Lizpy.Compiler;
using Lizpy.Internal;
using Lizpy.Syntax;
using ModApi.Craft.Program.Instructions;

namespace Lizpy.Builtin.Instructions {
    public class VariableInstructionHandler : IInstructionFunctionHandler {
        private static readonly String[] VariableInstructionSymbols = { "set!", "change!" };

        public string Namespace => String.Empty;
        public IEnumerable<string> SupportedSymbols => VariableInstructionSymbols;

        public ProgramInstruction[] CompileFunction(
            CompilerState state,
            ListExpression expression) {

            if (expression.Items.Count != 3) {
                throw new LizpyInternalCompilationException(
                    $"Invalid function call, argument count mismatch. Expected: 2, Actual: {expression.Items.Count - 1}",
                    expression
                );
            }
            if (!(expression.Items[1] is SymbolExpression variableSymbol)) {
                throw new LizpyInternalCompilationException(
                    $"Invalid function call, expression type mismatch. Expected: Symbol, Actual: {expression.Items[1].Type}",
                    expression
                );
            }

            var function = ((SymbolExpression)expression.Items[0]).GetLocalName();

            ProgramInstruction instruction;
            switch (function) {
                case "set!":
                    instruction = new SetVariableInstruction {
                        Style = "set-variable"
                    };
                    break;
                case "change!":
                    instruction = new ChangeVariableInstruction {
                        Style = "change-variable"
                    };
                    break;
                default:
                    throw new NotSupportedException($"Unsupported Symbol {expression.Items[0]}");
            }

            instruction.InitializeExpressions(
                new LateBoundVariableExpression {
                    Reference = variableSymbol,
                    SourceNamespace = state.Namespace
                },
                state.CompileExpression(expression.Items[2])
            );

            return new[] { instruction };
        }
    }
}
