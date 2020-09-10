using System;
using System.Collections.Generic;
using Lizpy.Compiler;
using Lizpy.Syntax;
using ModApi.Craft.Program;
using ModApi.Craft.Program.Expressions;
using UnityEngine;

namespace Lizpy.Builtin.Declarations {
    public class ExpressionDeclarationHandler : CustomNodeDeclarationHandler<CustomExpression> {
        private static readonly String[] ExpressionDeclarationSymbols = { "defexpr" };

        public override IEnumerable<String> SupportedSymbols => ExpressionDeclarationSymbols;

        protected override string NodeType => "Expression";

        protected override CustomExpression CreateNode(string qualifiedName, string callFormat, string format, ArrayExpression position) {
            return new CustomExpression {
                Name = qualifiedName,
                CallFormat = callFormat,
                Format = format,
                EditorPosition =
                    position != null &&
                    position.Items[0] is INumberExpression x &&
                    position.Items[1] is INumberExpression y ?
                        new Vector2((Single)x.AsDouble(), (Single)y.AsDouble()) :
                        (Vector2?)null,
                Style = "custom-expression"
            };
        }

        protected override ProgramNode GetCustomProgramNode(CompilerState state, String nodeName) {
            return state.Program.GetCustomExpression(nodeName);
        }

        protected override CompilerResult ApplyDeclarationImpl(
            CompilerState state,
            ListExpression declaration,
            String qualifiedName,
            SymbolExpression qualifiedIdentifierSymbol,
            CustomExpression node) {

            if (state.ExpressionNameLookup.ContainsKey(qualifiedIdentifierSymbol)) {
                return new CompilerResult(
                    declaration.Items[0],
                    $"Invalid expression definition. An expression with this symbol has already been declared: {qualifiedIdentifierSymbol}"
                );
            }

            node.InitializeExpressions(state.CompileExpression(declaration.Items[3]));
            state.Program.AddCustomExpression(node);
            state.Program.RootExpressions.Add(node);
            state.AddExpression(
                qualifiedIdentifierSymbol,
                qualifiedName
            );

            return CompilerResult.Success;
        }

        protected override String GenerateFormat(String callFormat, ArrayExpression argumentList) {
            return $"{base.GenerateFormat(callFormat, argumentList)} return (0)";
        }
    }
}
