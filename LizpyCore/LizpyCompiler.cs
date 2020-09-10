using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Lizpy.Builtin;
using Lizpy.Builtin.Declarations;
using Lizpy.Builtin.Expressions;
using Lizpy.Builtin.Expressions.Craft;
using Lizpy.Builtin.Instructions;
using Lizpy.Builtin.Instructions.Craft;
using Lizpy.Compiler;
using Lizpy.Internal;
using Lizpy.Syntax;
using ModApi.Craft.Program;
using ModApi.Craft.Program.Instructions;
using UnityEngine;
using ExpressionType = Lizpy.Syntax.ExpressionType;

namespace Lizpy {
    public class LizpyCompiler {
        [NotNull] private readonly IReadOnlyDictionary<SymbolExpression, IDeclarationHandler> declarationHandlers;
        [NotNull] private readonly IReadOnlyDictionary<SymbolExpression, IExpressionFunctionHandler> expressionFunctionHandlers;
        [NotNull] private readonly IReadOnlyDictionary<SymbolExpression, IInstructionFunctionHandler> instructionFunctionHandlers;
        [NotNull] private readonly IReadOnlyDictionary<ExpressionType, IExpressionHandler> expressionHandlers;
        [NotNull] private readonly IEnumerable<ICompilerPass> compilerPasses;

        private static readonly SymbolExpression PositionKey = new SymbolExpression(":position");

        public static LizpyCompiler DefaultInstance { get; }

        public LizpyCompiler(
            [NotNull] IEnumerable<IDeclarationHandler> declarationHandlers,
            [NotNull] IEnumerable<IExpressionFunctionHandler> expressionFunctionHandlers,
            [NotNull] IEnumerable<IInstructionFunctionHandler> instructionFunctionHandlers,
            [NotNull] IEnumerable<IExpressionHandler> expressionHandlers,
            [NotNull] IEnumerable<ICompilerPass> compilerPasses) {

            if (declarationHandlers == null) {
                throw new ArgumentNullException(nameof(declarationHandlers));
            }

            if (expressionFunctionHandlers == null) {
                throw new ArgumentNullException(nameof(expressionFunctionHandlers));
            }

            if (instructionFunctionHandlers == null) {
                throw new ArgumentNullException(nameof(instructionFunctionHandlers));
            }

            if (expressionHandlers == null) {
                throw new ArgumentNullException(nameof(expressionHandlers));
            }

            if (compilerPasses == null) {
                throw new ArgumentNullException(nameof(compilerPasses));
            }

            this.declarationHandlers =
                ToHandlerMap(declarationHandlers, _ => String.Empty, handler => handler.SupportedSymbols);
            this.expressionFunctionHandlers =
                ToHandlerMap(expressionFunctionHandlers, handler => handler.Namespace, handler => handler.SupportedSymbols);
            this.instructionFunctionHandlers =
                ToHandlerMap(instructionFunctionHandlers, handler => handler.Namespace, handler => handler.SupportedSymbols);
            this.expressionHandlers =
                expressionHandlers
                    .SelectMany(h => h.SupportedTypes.Select(t => (t, h)))
                    .ToDictionary(th => th.Item1, th => th.Item2);
            this.compilerPasses = compilerPasses;
        }

        private static IReadOnlyDictionary<SymbolExpression, THandler> ToHandlerMap<THandler>(
            IEnumerable<THandler> handlers,
            Func<THandler, String> getHandlerNamespace,
            Func<THandler, IEnumerable<String>> getHandlerKeys) {

            return handlers
                .SelectMany(h => {
                    var ns = getHandlerNamespace(h);
                    return getHandlerKeys(h).Select(k => (ApplyNamespace(ns, new SymbolExpression(k)), h));
                })
                .ToDictionary(kh => kh.Item1, kh => kh.Item2);
        }

        static LizpyCompiler() {
            DefaultInstance = new LizpyCompiler(
                new IDeclarationHandler[] {
                    new VariableDeclarationHandler(),
                    new EventDeclarationHandler(),
                    new InstructionDeclarationHandler(),
                    new ExpressionDeclarationHandler(),
                },
                new IExpressionFunctionHandler[] {
                    new AdvancedMathFunctionHandler(),
                    new BinaryMathFunctionHandler(),
                    new BooleanFunctionHandler(),
                    new ComparisonFunctionHandler(),
                    new ConditionalFunctionHandler(),
                    new ListExpressionHandler(),
                    new NewVectorFunctionHandler(),
                    new StringFunctionHandler(),
                    new UnaryMathFunctionHandler(),
                    new VectorFunctionHandler(),
                    // Craft
                    new CoordinateConversionHandler(),
                    new CraftPropertyFunctionHandler(),
                    new PlanetInfoFunctionHandler(),
                },
                new IInstructionFunctionHandler[] {
                    new BroadcastInstructionHandler(),
                    new CommentHandler(),
                    new DisplayInstructionHandler(),
                    new ListInstructionHandler(),
                    new LogMessageInstructionHandler(),
                    new ProgramFlowInstructionHandler(),
                    new VariableInstructionHandler(),
                    // Craft
                    new MiscCraftInstructionHandler(),
                    new SetCameraPropertyInstructionHandler(),
                    new SetPartPropertyInstructionHandler(),
                },
                new IExpressionHandler[] {
                    new ArrayExpressionHandler(),
                    new ConstantExpressionHandler(),
                },
                new ICompilerPass[] {
                    new LateBindingPass(),
                }
            );
        }

        public FlightProgram Compile(String filePath) {
            using (var reader = File.OpenText(filePath)) {
                return Compile(filePath, reader);
            }
        }

        public FlightProgram Compile(String fileName, Stream stream) {
            using (var reader = new StreamReader(stream)) {
                return Compile(fileName, reader);
            }
        }

        public FlightProgram Compile(String fileName, TextReader reader) {
            var state = new CompilerState(this, new FlightProgram());

            var input = reader.ReadToEnd();
            CompileImpl(fileName, input, state);

            try {
                foreach (var compilerPass in this.compilerPasses) {
                    compilerPass.ApplyPass(state);
                }
            } catch (LizpyInternalCompilationException ex) {
                if (ex is LizpyCompilationException) {
                    throw;
                } else {
                    var (line, column) = GetLocation(input, ex.Expression.StartIndex);
                    throw new LizpyCompilationException(
                        ex.Message,
                        ex.Expression,
                        ex,
                        fileName,
                        line,
                        column
                    );
                }
            }

            if (state.Errors.Any()) {
                var firstError = state.Errors.First();
                throw new LizpyCompilationException(
                    firstError.Error,
                    firstError.Expression,
                    // TODO attach filename and location info to every expression?
                    null,
                    0,
                    0
                );
            }

            return state.Program;
        }

        private void CompileImpl(String fileName, String input, CompilerState state) {
            var parser = new LizpyParser();
            var result = parser.GetMatch(input, parser.TopLevelExpression);

            void RaiseException(
                SExpression expression,
                String message) {
                throw CreateException(fileName, input, expression, message);
            }

            if (result.Success) {
                if (!(result.Result is ListExpression programExpression)) {
                    RaiseException(
                        result.Result,
                        "A Lizpy Flight Program file must have a list as its root expression."
                    );
                }

                if (programExpression.Items.Count == 0) {
                    RaiseException(
                        programExpression,
                        "A Lizpy Flight Program file must have a list containing the 'program' symbol as its first item.");
                }

                String @namespace;

                if (programExpression.Items.Count > 1 && programExpression.Items[1] is StringExpression namespaceExpression) {
                    @namespace = namespaceExpression.Value;
                    if (!namespaceExpression.IsValidNamespaceString()) {
                        RaiseException(
                            namespaceExpression,
                            $"The specified string is not a valid namespace name ({@namespace}). Namespaces can only contain letters and numbers.");
                    }
                } else {
                    @namespace = String.Empty;
                }

                state.PushNamespace(@namespace);
                try {
                    foreach (var declarationExpression in programExpression.Items.Skip(2)) {
                        if (!(declarationExpression is ListExpression declarationList)) {
                            RaiseException(
                                declarationExpression,
                                $"Invalid program declaration. Expected: List. Encountered: {declarationExpression.Type}."
                            );
                        }

                        if (declarationList.Items.Count == 0) {
                            RaiseException(
                                declarationList,
                                $"Invalid program declaration. An empty list is not a valid to level declaration."
                            );
                        }

                        if (!(declarationList.Items[0] is SymbolExpression declarationSymbol)) {
                            RaiseException(
                                declarationList.Items[0],
                                $"Invalid program declaration. Expected: Symbol, Encountered: {declarationList.Items[0].Type}."
                            );
                        }

                        if (declarationSymbol.Value == "import") {
                            // Todo: support introducing a position offset for all of the imported elements.
                            if (declarationList.Items.Count != 2) {
                                RaiseException(
                                    declarationList,
                                    $"Invalid import statement, argument count mismatch. Expected: 1, Actual: {declarationList.Items.Count - 1}."
                                );
                            }

                            if (!(declarationList.Items[1] is StringExpression importPath)) {
                                RaiseException(
                                    declarationList.Items[1],
                                    $"Invalid import statement. Expected: String. Encountered: {declarationList.Items[1].Type}."
                                );
                            }

                            if (File.Exists(importPath.Value)) {
                                // Compile another file as a part of this flight program
                                using (var importReader = File.OpenText(importPath.Value)) {
                                    var importInput = importReader.ReadToEnd();
                                    CompileImpl(importPath.Value, importInput, state);
                                }
                            } else {
                                RaiseException(
                                    importPath,
                                    $"The specified file was not found: {importPath.Value}"
                                );
                            }
                        } else if (this.declarationHandlers.TryGetValue(declarationSymbol, out var handler)) {
                            CompilerResult declarationResult;

                            try {
                                declarationResult = handler.ApplyDeclaration(state, declarationList);
                            } catch (LizpyInternalCompilationException ex) {
                                if (ex is LizpyCompilationException) {
                                    throw;
                                } else {
                                    var (line, column) = GetLocation(input, ex.Expression.StartIndex);
                                    throw new LizpyCompilationException(
                                        ex.Message,
                                        ex.Expression,
                                        ex,
                                        fileName,
                                        line,
                                        column
                                    );
                                }
                            }

                            if (!declarationResult.IsSuccess) {
                                RaiseException(
                                    declarationResult.Expression,
                                    declarationResult.Error
                                );
                            }
                        } else if (declarationSymbol.Value == "comment" || declarationSymbol.Value == "do") {
                            // Allow top level comments and do blocks
                            var instruction =
                                this.CompileInstruction(state, declarationList).First();

                            if (declarationSymbol.TryGetMetadata<ArrayExpression>(PositionKey, out var position) &&
                                position.Items.Count == 2 &&
                                position.Items[0] is INumberExpression x &&
                                position.Items[1] is INumberExpression y) {
                                instruction.EditorPosition =
                                    new Vector2((Single)x.AsDouble(), (Single)y.AsDouble());
                            }

                            state.Program.RootInstructions.Add(instruction);
                        } else {
                            RaiseException(
                                declarationSymbol,
                                $"Unrecognized top level declaration: {declarationSymbol}"
                            );
                        }
                    }
                } finally {
                    state.PopNamespace();
                }
            } else {
                var (line, column) = GetLocation(input, result.ErrorIndex);
                throw new LizpySyntaxException(result.Error, fileName, line, column);
            }
        }

        private static LizpyCompilationException CreateException(
            String fileName,
            String input,
            SExpression expression,
            String message) {

            var (line, column) = GetLocation(input, expression.StartIndex);
            return new LizpyCompilationException(
                message,
                expression,
                fileName,
                line,
                column
            );
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
                    if (this.expressionFunctionHandlers.TryGetValue(symbol, out var handler)) {
                        return handler.CompileFunction(state, list);
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
                } else if (symbol.IsValidVariable(out var match)) {
                    if (match.Groups["namespace"].Success && !String.IsNullOrEmpty(match.Groups["namespace"].Value)) {
                        return new VariableExpression {
                            VariableName = symbol.ToVizzyNamespacedName()
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

        public ProgramInstruction[] CompileInstruction(
            CompilerState state,
            SExpression sexpression) {

            if (sexpression is ListExpression list) {
                if (list.Items.Count == 0) {
                    throw new LizpyInternalCompilationException(
                        "Unexpected end of list encountered. An instruction list must contain a function name.",
                        list
                    );
                }

                if (list.Items[0] is SymbolExpression symbol) {
                    if (this.instructionFunctionHandlers.TryGetValue(symbol, out var handler)) {
                        return handler.CompileFunction(state, list);
                    } else {
                        // Assume this is a custom instruction reference
                        var inst = new LateBoundCallCustomInstruction {
                            Style = "call-custom-instruction",
                            // Custom expression calls will need to be resolved in a subsequent pass once all declarations and imports are resolved. Even if they are explicitly namespaced we still need to check for explicit naming.
                            Reference = symbol,
                            SourceNamespace = state.Namespace
                        };

                        inst.InitializeExpressions(
                            list.Items.Skip(1).Select(state.CompileExpression).ToArray()
                        );

                        return new ProgramInstruction[] { inst };
                    }
                } else {
                    throw new LizpyInternalCompilationException(
                        $"Unexpected expression type: {list.Items[0].Type}. The first form in a list must be a symbol.",
                        list.Items[0]);
                }
            } else {
                throw new LizpyInternalCompilationException(
                    $"Unsupported expression type: {sexpression.Type}. Expected a instruction function invocation.",
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
                new SymbolExpression($"{@namespace}/{symbol.Value}") :
                symbol;
        }

        private static (Int32 line, Int32 column) GetLocation(
            String input,
            Int32 index) {

            var line = 0;
            var column = 0;

            for (var i = 0; i < index && i < input.Length; i++) {
                if (input[i] == '\n') {
                    line++;
                    column = 0;
                } else {
                    column++;
                }
            }

            return (line, column);
        }
    }
}
