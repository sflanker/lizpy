using System;
using System.Collections.Generic;
using Lizpy.Compiler;
using Lizpy.Syntax;
using ModApi.Craft.Program;

namespace Lizpy.Builtin {
    public class VariableDeclarationHandler : IDeclarationHandler {
        private static readonly String[] VariableDeclarationSymbols =
            {
                "declare",
                "declarel"
            };

        public IEnumerable<String> SupportedSymbols => VariableDeclarationSymbols;

        public CompilerResult ApplyDeclaration(
            CompilerState state,
            ListExpression declaration) {
            if (declaration.Items.Count != 2) {
                return new CompilerResult(
                    declaration,
                    $"Invalid variable declaration. Expected: ({declaration.Items[0]} expression)"
                );
            }

            var localNameExpression = declaration.Items[1];

            if (!(localNameExpression is SymbolExpression symbol)) {
                return new CompilerResult(
                    localNameExpression,
                    $"Invalid variable declaration. Expected a symbol, encountered: {localNameExpression.Type}."
                );
            }

            if (!LizpyCompiler.IsValidVariableName(symbol.Value)) {
                return new CompilerResult(
                    localNameExpression,
                    $"Invalid variable declaration. The specified symbol ({localNameExpression}) has illegal characters. Variable names may contain only letters numbers, the tilde character, and underscores.");
            }

            var variableName = LizpyCompiler.ApplyNamespace(state.Namespace, symbol.Value);

            if (state.Program.GlobalVariables.GetVariable(variableName) != null) {
                return new CompilerResult(
                    localNameExpression,
                    $"Invalid variable declaration. A variable with this name has already been declared: {variableName}"
                );
            }

            state.Program.GlobalVariables.AddVariable(
                LizpyCompiler.GetLocalName((SymbolExpression)declaration.Items[0]) == "declarel" ?
                    new Variable(variableName, new ExpressionResult(new List<String>())) :
                    new Variable(variableName));

            return CompilerResult.Success;
        }
    }
}
