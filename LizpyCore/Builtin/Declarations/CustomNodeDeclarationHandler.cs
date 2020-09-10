using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Lizpy.Compiler;
using Lizpy.Internal;
using Lizpy.Syntax;
using ModApi.Craft.Program;

namespace Lizpy.Builtin.Declarations {
    public abstract class CustomNodeDeclarationHandler<TProgramNode> : DeclarationHandlerBase {
        protected abstract String NodeType { get; }
        protected String NodeTypeLowercase => this.NodeType.ToLower();

        public override CompilerResult ApplyDeclaration(
            CompilerState state,
            ListExpression declaration) {

            if (!(declaration.Items[1] is SymbolExpression identifierSymbol)) {
                return new CompilerResult(
                    declaration.Items[1],
                    $"Invalid {this.NodeTypeLowercase} definition. Expected Symbol, encountered: {declaration.Items[1].Type}"
                );
            }

            if (!(declaration.Items[2] is ArrayExpression argumentList)) {
                return new CompilerResult(
                    declaration.Items[2],
                    $"Invalid {this.NodeTypeLowercase} definition. Expected Array, encountered: {declaration.Items[2].Type}"
                );
            }

            if (!identifierSymbol.IsValidUnNamespacedVariable()) {
                return new CompilerResult(
                    identifierSymbol,
                    $"Invalid {this.NodeTypeLowercase} definition. The specified symbol ({identifierSymbol}) has illegal characters. {this.NodeType} identifiers may contain only letters numbers, the tilde character, and underscores."
                );
            }

            var localName =
                    identifierSymbol.TryGetMetadata<StringExpression>(NameKey, out var nameString) ?
                        nameString.Value :
                        identifierSymbol.Value;

            var qualifiedName = LizpyCompiler.ApplyNamespace(state.Namespace, localName);

            if (this.GetCustomProgramNode(state, qualifiedName) != null) {
                return new CompilerResult(
                    identifierSymbol,
                    $"Invalid {this.NodeTypeLowercase} definition. An expression with this name has already been declared: {qualifiedName}"
                );
            }

            var localVariables = new List<SymbolExpression>();
            foreach (var argument in argumentList.Items) {
                if (!(argument is SymbolExpression argumentSymbol)) {
                    return new CompilerResult(
                        argument,
                        $"Invalid {this.NodeTypeLowercase} definition. Invalid argument list. Expected Symbol, encountered: {argument.Type}"
                    );
                }

                if (!argumentSymbol.IsValidUnNamespacedVariable()) {
                    return new CompilerResult(
                        argumentSymbol,
                        $"Invalid {this.NodeTypeLowercase} definition. The specified symbol ({argumentSymbol}) has illegal characters. {this.NodeType} arguments may contain only letters numbers, the tilde character, and underscores."
                    );
                }

                if (localVariables.Contains(argumentSymbol)) {
                    return new CompilerResult(
                        argumentSymbol,
                        $"Invalid {this.NodeTypeLowercase} definition. Duplicate argument name: {argumentSymbol}"
                    );
                }

                localVariables.Add(argumentSymbol);
            }

            var callFormat =
                identifierSymbol.TryGetMetadata<StringExpression>(FormatKey, out var formatString) ?
                    PopulateArgumentListFromCount(formatString.Value, argumentList.Items.Count) :
                    $"{qualifiedName}{ArgumentListFromCount(argumentList.Items.Count)}";

            var format = GenerateFormat(callFormat, argumentList);

            var position =
                identifierSymbol.TryGetMetadata<ArrayExpression>(PositionKey, out var p) ? p : null;

            var customNode = CreateNode(qualifiedName, callFormat, format, position);

            var qualifiedIdentifier =
                LizpyCompiler.ApplyNamespace(state.Namespace, identifierSymbol);

            state.PushScope(argumentList.Items.Cast<SymbolExpression>().ToList());
            try {
                return this.ApplyDeclarationImpl(
                    state,
                    declaration,
                    qualifiedName,
                    qualifiedIdentifier,
                    customNode);
            } finally {
                state.PopScope();
            }
        }

        protected abstract TProgramNode CreateNode(
            String qualifiedName,
            String callFormat,
            String format,
            ArrayExpression position);

        protected abstract ProgramNode GetCustomProgramNode(CompilerState state, String nodeName);

        protected abstract CompilerResult ApplyDeclarationImpl(
            CompilerState state,
            ListExpression declaration,
            String qualifiedName,
            SymbolExpression qualifiedIdentifierSymbol,
            TProgramNode node);

        private static readonly Regex ArgumentPlaceholderRegex = new Regex(@"\([0-9]+\)", RegexOptions.Compiled);

        protected virtual String GenerateFormat(
            String callFormat,
            ArrayExpression argumentList) {

            var builder = new StringBuilder();

            var start = 0;
            for (var current = 0; current < callFormat.Length; current++) {
                if (callFormat[current] == '(') {
                    var match = ArgumentPlaceholderRegex.Match(callFormat, current);
                    if (match.Success) {
                        if (start < current) {
                            builder.Append(callFormat.Substring(start, current - start));
                        }

                        var argumentIndex = Int32.Parse(match.Value.TrimStart('(').TrimEnd(')'));
                        builder.Append($"|{argumentList.Items[argumentIndex]}|");

                        current += match.Length - 1;
                        start = current + 1;
                    }
                }
            }

            if (start < callFormat.Length) {
                builder.Append(callFormat.Substring(start));
            }

            return builder.ToString();
        }

        protected static String ArgumentListFromCount(Int32 count) {
            return count == 0 ?
                "" :
                $" ({String.Join(") (", Enumerable.Range(0, count))})";
        }

        protected static String PopulateArgumentListFromCount(
            String callFormat,
            Int32 count) {

            var builder = new StringBuilder();

            var start = 0;
            var i = 0;
            for (; i < count; i++) {
                var nextArgIx = callFormat.IndexOf("%", start, StringComparison.OrdinalIgnoreCase);

                if (nextArgIx >= 0) {
                    builder.Append(callFormat.Substring(start, nextArgIx - start));
                    builder.Append($"({i})");
                    start = nextArgIx + 1;
                } else {
                    break;
                }
            }
            if (start < callFormat.Length) {
                builder.Append(callFormat.Substring(start));
            }

            if (i < count) {
                builder.Append($" ({String.Join(") (", Enumerable.Range(i, count - i))})");
            }

            return builder.ToString();
        }
    }
}
