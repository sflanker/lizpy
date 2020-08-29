using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Lizpy.Builtin;
using Lizpy.Compiler;
using Lizpy.Internal;
using Lizpy.Syntax;
using ModApi.Craft.Program;
using ExpressionType = Lizpy.Syntax.ExpressionType;

namespace Lizpy {
    public class LizpyCompiler {
        [NotNull] private readonly IReadOnlyDictionary<String, IDeclarationHandler> declarationHandlers;
        [NotNull] private readonly IReadOnlyDictionary<String, IFunctionHandler> functionHandlers;
        [NotNull] private readonly IReadOnlyDictionary<ExpressionType, IExpressionHandler> expressionHandlers;
        [NotNull] private readonly IEnumerable<ICompilerPass> compilerPasses;
        private static readonly Regex VariableNameRegex = new Regex("^[0-9A-Za-z_~]+$");
        private static readonly Regex NamespacedRegex = new Regex("^[0-9A-Za-z~]+$");
        private static readonly Regex NamespacedVariableNameRegex =
            new Regex("^(?'namespace'[0-9A-Za-z~]+)(/(?'name'[0-9A-Za-z_~]+))?$");

        public LizpyCompiler(
            [NotNull] IReadOnlyDictionary<String, IDeclarationHandler> declarationHandlers,
            [NotNull] IReadOnlyDictionary<String, IFunctionHandler> functionHandlers,
            [NotNull] IReadOnlyDictionary<ExpressionType, IExpressionHandler> expressionHandlers,
            [NotNull] IEnumerable<ICompilerPass> compilerPasses) {

            if (declarationHandlers == null) {
                throw new ArgumentNullException(nameof(declarationHandlers));
            }
            if (functionHandlers == null) {
                throw new ArgumentNullException(nameof(functionHandlers));
            }
            if (expressionHandlers == null) {
                throw new ArgumentNullException(nameof(expressionHandlers));
            }
            if (compilerPasses == null) {
                throw new ArgumentNullException(nameof(compilerPasses));
            }

            this.declarationHandlers = declarationHandlers;
            this.functionHandlers = functionHandlers;
            this.expressionHandlers = expressionHandlers;
            this.compilerPasses = compilerPasses;
        }

        public FlightProgram Compile(String filePath) {
            using (var reader = File.OpenText(filePath)) {
                return Compile(reader);
            }
        }

        public FlightProgram Compile(Stream stream) {
            using (var reader = new StreamReader(stream)) {
                return Compile(reader);
            }
        }

        public FlightProgram Compile(TextReader reader) {
            var parser = new LizpyParser();
            var input = reader.ReadToEnd();
            var result = parser.GetMatch(input, parser.TopLevelExpression);

            if (result.Success) {
                throw new NotImplementedException();
            } else {
                // TODO Find line and column numbers
                throw new LizpySyntaxException(result.Error, 0, 0);
            }
        }

        public ProgramExpression CompileExpression(
            CompilerState state,
            SExpression sexpression) {

            if (sexpression is ListExpression list) {
                if (list.Items.Count == 0) {
                    throw new LizpyInternalCompilationException(
                        "Unexpected end of list encountered. A list must contain a function name. To initialize an empty list value use [].",
                        list
                    );
                }

                if (list.Items[0] is SymbolExpression symbol) {
                    if (this.functionHandlers.TryGetValue(symbol.Value, out var handler)) {
                        var result = handler.CompileFunction(state, list);
                        if (result is ProgramExpression resultExpression) {
                            return resultExpression;
                        } else {
                            throw new LizpyInternalCompilationException(
                                $"Illegal use of instruction. Expected an expression valued function, but encountered: {symbol}",
                                list
                            );
                        }
                    } else {
                        // Assume this is a custom expression reference
                        var op = new LateBoundCallCustomExpression {
                            Style = "call-custom-expression",
                            // Custom expression calls will need to be resolved in a subsequent pass once all declarations and imports are resolved. Even if they are explicitly namespaced we still need to check for explicit naming.
                            Reference = symbol,
                            SourceNamespace = state.Namespace
                        };

                        op.InitializeExpressions(
                            list.Items.Skip(1).Select(state.CompileExpression).ToArray()
                        );

                        return op;
                    }
                } else {
                    throw new LizpyInternalCompilationException(
                        $"Unexpected expression type: {list.Items[0].Type}. The first form in a list must be a symbol.",
                        list.Items[0]);
                }
            } else if (sexpression is SymbolExpression symbol) {
                // Variable reference.
                if (state.IsLocalVariable(symbol.Value)) {
                    return new VariableExpression {
                        IsLocal = true,
                        VariableName = symbol.Value
                    };
                } else if (IsValidNamespacedVariableName(symbol.Value, out var match)) {
                    if (match.Groups["namespace"].Success && !String.IsNullOrEmpty(match.Groups["namespace"].Value)) {
                        return new VariableExpression {
                            VariableName = VizzyNamespacedNameFromSymbol(symbol)
                        };
                    } else {
                        // This is an un-namespaced global variable, so we may need to do late binding
                        return new LateBoundVariableExpression {
                            Reference = symbol,
                            SourceNamespace = state.Namespace
                        };
                    }
                } else {
                    throw new LizpyInternalCompilationException(
                        $"Invalid symbol. Expected a valid variable symbol, encountered: {symbol}",
                        symbol
                    );
                }
            } else if (this.expressionHandlers.TryGetValue(sexpression.Type, out var handler)) {
                return handler.CompileExpression(state, sexpression);
            } else {
                throw new LizpyInternalCompilationException(
                    $"Unsupported expression type: {sexpression.Type}",
                    sexpression
                );
            }
        }

        public static String ApplyNamespace(String @namespace, String localName) {
            return !String.IsNullOrEmpty(@namespace) ?
                $"{@namespace}__{localName}" :
                localName;
        }

        public static SymbolExpression ApplyNamespace(String @namespace, SymbolExpression symbol) {
            return !String.IsNullOrEmpty(@namespace) && !symbol.Value.Contains("/") ?
                new SymbolExpression($"{@namespace}/{symbol}") :
                symbol;
        }

        public static String VizzyNamespacedNameFromSymbol(SymbolExpression symbol) {
            var parts = symbol.Value.Split(new[] { '/' }, 2);
            return String.Join("__", parts);
        }

        public static Boolean IsValidVariableName(String variableName) {
            return VariableNameRegex.IsMatch(variableName);
        }

        public static Boolean IsValidNamespacedVariableName(String variableName, out Match match) {
            match = NamespacedVariableNameRegex.Match(variableName);
            return match.Success;
        }

        public static String GetLocalName(SymbolExpression symbol) {
            if (symbol.Value.StartsWith("/")) {
                return symbol.Value;
            }

            var ix = symbol.Value.IndexOf('/');
            return ix < 0 ? symbol.Value : symbol.Value.Substring(ix + 1);
        }
    }
}
