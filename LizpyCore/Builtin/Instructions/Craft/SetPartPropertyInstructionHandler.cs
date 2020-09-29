using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Lizpy.Compiler;
using Lizpy.Internal;
using Lizpy.Syntax;
using ModApi.Craft.Program;
using ModApi.Craft.Program.Instructions;
using ExpressionType = Lizpy.Syntax.ExpressionType;

namespace Lizpy.Builtin.Instructions.Craft {
    public class SetPartPropertyInstructionHandler : IInstructionFunctionHandler {
        private static readonly String[] SetPartPropertySymbols = { "set-part-property!" };

        public string Namespace => "craft";
        public IEnumerable<string> SupportedSymbols => SetPartPropertySymbols;

        public ProgramInstruction[] CompileFunction(
            CompilerState state,
            ListExpression expression) {

            if (expression.Items.Count != 4) {
                throw new LizpyInternalCompilationException(
                    $"Invalid function call, argument count mismatch. Expected: 3, Actual: {expression.Items.Count - 1}",
                    expression
                );
            }

            if (!(expression.Items[1] is SymbolExpression partPropertySymbol)) {
                throw new LizpyInternalCompilationException(
                    $"Invalid craft/set-part-property! call. Expected: {ExpressionType.Symbol}, Actual: {expression.Items[1].Type}",
                    expression.Items[1]
                );
            }

            if (!partPropertySymbol.Value.StartsWith(":")) {
                throw new LizpyInternalCompilationException(
                    $"Invalid craft/set-part-property! call. The part property symbol must be a keyword (start with a colon), Actual: {partPropertySymbol}",
                    partPropertySymbol
                );
            }

            var propertyName = partPropertySymbol.Value.Substring(1);
            var property = CraftProperties.GetProperty(
                propertyName.StartsWith("Part.Set") ? propertyName : $"Part.Set{propertyName}"
            );

            if (property == null) {
                throw new LizpyInternalCompilationException(
                    $"Invalid craft/set-part-property! call. The specified craft property is not recognized: {partPropertySymbol}",
                    expression
                );
            } else if (property.Category != "SetPart") {
                throw new LizpyInternalCompilationException(
                    $"Invalid craft/set-part-property! call. The specified craft property is not supported: {partPropertySymbol}",
                    expression
                );
            }

            var inst = (SetCraftPropertyInstruction)
                ProgramSerializer.DeserializeProgramNode(
                    new XElement(
                        "SetCraftProperty",
                        new XAttribute("style", "set-part"),
                        new XAttribute("property", property.XmlName)));

            inst.InitializeExpressions(
                expression.Items.Skip(2).Select(state.CompileExpression).ToArray()
            );

            return new ProgramInstruction[] { inst };
        }
    }
}
