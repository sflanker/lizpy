using System;
using System.Collections.Generic;
using System.Linq;
using Lizpy.Compiler;
using Lizpy.Internal;
using Lizpy.Syntax;
using ModApi.Craft.Program.Instructions;

namespace Lizpy.Builtin.Instructions.Craft {
    public class SetCameraPropertyInstructionHandler : IInstructionFunctionHandler {
        private static readonly String[] SetCameraPropertySymbols = { "set-camera-property!" };

        public string Namespace => "craft";
        public IEnumerable<string> SupportedSymbols => SetCameraPropertySymbols;

        public ProgramInstruction[] CompileFunction(CompilerState state, ListExpression expression) {
            if (expression.Items.Count != 3) {
                throw new LizpyInternalCompilationException(
                    $"Invalid function call, argument count mismatch. Expected: 2, Actual: {expression.Items.Count - 1}",
                    expression
                );
            }

            if (!(expression.Items[1] is SymbolExpression cameraPropertySymbol)) {
                throw new LizpyInternalCompilationException(
                    $"Invalid craft/set-camera-property! call. Expected: {ExpressionType.Symbol}, Actual: {expression.Items[1].Type}",
                    expression.Items[1]
                );
            }

            if (!cameraPropertySymbol.Value.StartsWith(":")) {
                throw new LizpyInternalCompilationException(
                    $"Invalid craft/set-camera-property! call. The camera property symbol must be a keyword (start with a colon), Actual: {cameraPropertySymbol}",
                    cameraPropertySymbol
                );
            }

            var inst = new SetCameraPropertyInstruction {
                Style = "set-camera"
            };

            var cameraProperty =
                // Support things like Mode as well as mode for consistency with
                // craft info expressions.
                cameraPropertySymbol.Value.Substring(1, 1).ToLower() +
                cameraPropertySymbol.Value.Substring(2);

            if (inst.GetListItems("x").All(li => li.Id != cameraProperty)) {
                throw new LizpyInternalCompilationException(
                    $"Invalid craft/set-camera-property! call. Unrecognized camera property: {cameraProperty}",
                    cameraPropertySymbol
                );
            }

            inst.SetListValue("x", cameraProperty);

            inst.InitializeExpressions(
                state.CompileExpression(expression.Items[2])
            );

            return new ProgramInstruction[] { inst };
        }
    }
}
