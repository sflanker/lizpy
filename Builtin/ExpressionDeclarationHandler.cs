using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Lizpy.Compiler;
using Lizpy.Syntax;
using ModApi.Craft.Program;
using ModApi.Craft.Program.Expressions;
using UnityEngine;

namespace Lizpy.Builtin {
    public class ExpressionDeclarationHandler : DeclarationHandlerBase {
        private static readonly String[] ExpressionDeclarationSymbols = { "defexpr" };

        public override IEnumerable<String> SupportedSymbols => ExpressionDeclarationSymbols;

        public override CompilerResult ApplyDeclaration(
            CompilerState state,
            ListExpression declaration) {
            // expected form (defexpr symbol [args] expression)
            if (declaration.Items.Count != 4) {
                return new CompilerResult(
                    declaration,
                    "Invalid expression definition. Expected: (defexpr name [arg1 arg2] expression)."
                );
            }

            if (!(declaration.Items[1] is SymbolExpression expressionSymbol)) {
                return new CompilerResult(
                    declaration.Items[1],
                    $"Invalid expression definition. Expected Symbol, encountered: {declaration.Items[1].Type}"
                );
            }

            if (!(declaration.Items[2] is ArrayExpression argumentList)) {
                return new CompilerResult(
                    declaration.Items[2],
                    $"Invalid expression definition. Expected Array, encountered: {declaration.Items[2].Type}"
                );
            }

            if (!LizpyCompiler.IsValidVariableName(expressionSymbol.Value)) {
                return new CompilerResult(
                    expressionSymbol,
                    $"Invalid expression definition. The specified symbol ({expressionSymbol}) has illegal characters. Expression names may contain only letters numbers, the tilde character, and underscores."
                );
            }

            Boolean tryGetMetadata<TExpression>(SymbolExpression key, out TExpression data)
                where TExpression : SExpression {
                data = null;
                SExpression value = null;
                if (expressionSymbol.Metadata?.TryGetValue(key, out value) == true &&
                    value is TExpression typedValue) {
                    data = typedValue;
                    return true;
                } else {
                    data = null;
                    return false;
                }
            }

            var localName =
                    tryGetMetadata<StringExpression>(NameKey, out var nameString) ?
                        nameString.Value :
                        expressionSymbol.Value;

            var qualifiedName = LizpyCompiler.ApplyNamespace(state.Namespace, localName);

            if (state.Program.GetCustomExpression(qualifiedName) != null) {
                return new CompilerResult(
                    expressionSymbol,
                    $"Invalid expression definition. An expression with this name has already been declared: {qualifiedName}"
                );
            }

            var callFormat =
                tryGetMetadata<StringExpression>(FormatKey, out var formatString) ?
                    formatString.Value :
                    $"{qualifiedName}{ArgumentListFromCount(argumentList.Items.Count)}";

            var customExpression = new CustomExpression {
                Name = qualifiedName,
                CallFormat = callFormat,
                Format = GenerateFormat(callFormat, argumentList),
                EditorPosition =
                    tryGetMetadata<ArrayExpression>(PositionKey, out var position) &&
                        position.Items[0] is INumberExpression x &&
                        position.Items[1] is INumberExpression y ?
                        new Vector2((Single)x.AsDouble(), (Single)y.AsDouble()) :
                        (Vector2?)null,
                Style = "custom-expression"
            };

            foreach (var argument in argumentList.Items) {
                if (!(argument is SymbolExpression argumentSymbol)) {
                    return new CompilerResult(
                        argument,
                        $"Invalid expression definition. Invalid argument list. Expected Symbol, encountered: {argument.Type}"
                    );
                }

                if (!LizpyCompiler.IsValidVariableName(argumentSymbol.Value)) {
                    return new CompilerResult(
                        argumentSymbol,
                        $"Invalid expression definition. The specified symbol ({argumentSymbol}) has illegal characters. Variable names may contain only letters numbers, the tilde character, and underscores."
                    );
                }

                if (customExpression.LocalVariables.Any(v => v.Name == argumentSymbol.Value)) {
                    return new CompilerResult(
                        argumentSymbol,
                        $"Invalid expression definition. Duplicate argument name: {argumentSymbol}"
                    );
                }

                customExpression.LocalVariables.Add(
                    new LocalVariableDefinition {
                        Name = argumentSymbol.Value
                    }
                );
            }

            var qualifiedExpressionSymbol =
                LizpyCompiler.ApplyNamespace(state.Namespace, expressionSymbol);
            if (state.ExpressionNameLookup.ContainsKey(qualifiedExpressionSymbol)) {
                return new CompilerResult(
                    expressionSymbol,
                    $"Invalid expression definition. An expression with this symbol has already been declared: {qualifiedExpressionSymbol}"
                );
            }

            state.PushScope(argumentList.Items.Cast<SymbolExpression>().ToList());
            try {
                customExpression.InitializeExpressions(state.CompileExpression(declaration.Items[3]));
                state.Program.AddCustomExpression(customExpression);
                state.AddExpression(
                    qualifiedExpressionSymbol,
                    qualifiedName
                );
            } finally {
                state.PopScope();
            }

            return CompilerResult.Success;
        }

        private static readonly Regex ArgumentPlaceholderRegex = new Regex(@"\(^[0-9]+\)", RegexOptions.Compiled);

        private static String GenerateFormat(String callFormat, ArrayExpression argumentList) {
            var builder = new StringBuilder();

            var start = 0;
            for (var current = 0; current < callFormat.Length; current++) {
                if ( callFormat[current] == '(') {
                    var match = ArgumentPlaceholderRegex.Match(callFormat, current);
                    if (match.Success) {
                        if (start < current) {
                            builder.Append(callFormat.Substring(start, current - start));
                        }

                        var argumentIndex = Int32.Parse(match.Value.TrimStart('(').TrimEnd(')'));
                        builder.Append($"|{argumentList.Items[argumentIndex]}|");

                        current += match.Length;
                        start = current + 1;
                    }
                }
            }

            if (start < callFormat.Length) {
                builder.Append(callFormat.Substring(start));
            }

            builder.Append(" return (0)");

            return builder.ToString();
        }

        private static String ArgumentListFromCount(Int32 count) {
            return count == 0 ?
                "" :
                $" ({String.Join(") (", Enumerable.Range(0, count))})";
        }
    }
}
