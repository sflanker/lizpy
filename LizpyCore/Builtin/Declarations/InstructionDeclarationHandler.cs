using System;
using System.Collections.Generic;
using System.Linq;
using Lizpy.Compiler;
using Lizpy.Syntax;
using ModApi.Craft.Program;
using ModApi.Craft.Program.Instructions;
using UnityEngine;

namespace Lizpy.Builtin.Declarations {
    public class InstructionDeclarationHandler : CustomNodeDeclarationHandler<CustomInstruction> {
        private static readonly String[] ExpressionDeclarationSymbols = { "definst" };

        public override IEnumerable<String> SupportedSymbols => ExpressionDeclarationSymbols;

        protected override String NodeType => "Instruction";

        protected override CustomInstruction CreateNode(
            String qualifiedName,
            String callFormat,
            String format,
            ArrayExpression position) {

            return new CustomInstruction {
                Name = qualifiedName,
                CallFormat = callFormat,
                Format = format,
                EditorPosition =
                    position != null &&
                    position.Items[0] is INumberExpression x &&
                    position.Items[1] is INumberExpression y ?
                        new Vector2((Single)x.AsDouble(), (Single)y.AsDouble()) :
                        (Vector2?)null,
                Style = "custom-instruction"
            };
        }

        protected override ProgramNode GetCustomProgramNode(CompilerState state, String nodeName) {
            return state.Program.GetCustomInstruction(nodeName);
        }

        protected override CompilerResult ApplyDeclarationImpl(
            CompilerState state,
            ListExpression declaration,
            String qualifiedName,
            SymbolExpression qualifiedIdentifierSymbol,
            CustomInstruction node) {

            if (state.InstructionNameLookup.ContainsKey(qualifiedIdentifierSymbol)) {
                return new CompilerResult(
                    declaration.Items[0],
                    $"Invalid expression definition. An expression with this symbol has already been declared: {qualifiedIdentifierSymbol}"
                );
            }

            var currentNode = (ProgramInstruction)node;

            foreach (var expression in declaration.Items.Skip(3)) {
                var instructions = state.CompileInstruction(expression);
                currentNode.Next = instructions.First();
                currentNode = instructions.Last();
            }

            state.Program.AddCustomInstruction(node);
            state.Program.RootInstructions.Add(node);
            state.AddInstruction(
                qualifiedIdentifierSymbol,
                qualifiedName
            );

            return CompilerResult.Success;
        }
    }
}
