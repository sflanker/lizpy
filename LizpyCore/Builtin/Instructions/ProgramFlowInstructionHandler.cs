using System;
using System.Collections.Generic;
using System.Linq;
using Lizpy.Compiler;
using Lizpy.Internal;
using Lizpy.Syntax;
using ModApi.Craft.Program.Expressions;
using ModApi.Craft.Program.Instructions;

namespace Lizpy.Builtin.Instructions {
    public class ProgramFlowInstructionHandler : IInstructionFunctionHandler {
        public readonly String[] ProgramFlowSymbols = {
            "wait",
            "wait-until",
            "repeat",
            "while",
            "for",
            "break",
            "if",
            "cond",
            "do"
        };

        private static readonly IReadOnlyDictionary<String, (Func<ProgramInstruction> create, String style, Int32 expressions, Boolean supportsChildren, String
                [] variables)>
            FunctionInfoMapping =
                new Dictionary<String, (Func<ProgramInstruction>, String, Int32, Boolean, String[])> {
                    { "wait", (() => new WaitSecondsInstruction(), "wait-seconds", 1, false, null) },
                    { "wait-until", (() => new WaitUntilInstruction(), "wait-until", 1, false, null) },
                    { "repeat", (() => new RepeatInstruction(), "repeat", 1, true, null) },
                    { "while", (() => new WhileInstruction(), "while", 1, true, null) },
                    { "for", (() => new ForInstruction(), "for", 3, true, new[] { "i" }) },
                    { "break", (() => new BreakInstruction(), "break", 0, false, null) }
                };

        public String Namespace => String.Empty;
        public IEnumerable<String> SupportedSymbols => ProgramFlowSymbols;

        public ProgramInstruction[] CompileFunction(
            CompilerState state,
            ListExpression expression) {

            var function = ((SymbolExpression)expression.Items[0]).GetLocalName();

            if (FunctionInfoMapping.TryGetValue(function, out var info)) {
                var instruction = info.create();

                instruction.Style = info.style;
                if (info.expressions > 0) {
                    if (expression.Items.Count < info.expressions + 1) {
                        throw new LizpyInternalCompilationException(
                            $"Invalid function call, argument count mismatch. Expected: {info.expressions}, Actual: {expression.Items.Count - 1}",
                            expression
                        );
                    }

                    instruction.InitializeExpressions(
                        expression.Items.Skip(1).Take(info.expressions)
                            .Select(state.CompileExpression)
                            .ToArray()
                    );
                }

                if (info.supportsChildren) {
                    var createsScope = info.variables?.Length > 0;
                    if (createsScope) {
                        state.PushScope(info.variables.Select(v => new SymbolExpression(v)).ToList());
                    }

                    try {
                        ProgramInstruction currentInstruction = null;
                        foreach (var child in expression.Items.Skip(1 + info.expressions)) {
                            var nextInstructions = state.CompileInstruction(child);
                            if (currentInstruction != null) {
                                currentInstruction.Next = nextInstructions.First();
                                currentInstruction = nextInstructions.Last();
                            } else {
                                instruction.FirstChild = nextInstructions.First();
                                currentInstruction = nextInstructions.Last();
                            }
                        }
                    } finally {
                        if (createsScope) {
                            state.PopScope();
                        }
                    }
                }

                return new[] { instruction };
            } else if (function == "do") {
                // allows multiple instructions to be grouped in a single Lizpy expression
                var instructions = new Queue<ProgramInstruction>();
                ProgramInstruction currentInstruction = null;
                foreach (var child in expression.Items.Skip(1)) {
                    var childInstructions = state.CompileInstruction(child);
                    if (currentInstruction != null) {
                        currentInstruction.Next = childInstructions.First();
                        currentInstruction = childInstructions.Last();
                    } else {
                        currentInstruction = childInstructions.Last();
                    }

                    foreach (var inst in childInstructions) {
                        instructions.Enqueue(inst);
                    }
                }

                return instructions.ToArray();
            } else if (function == "if") {
                var ifInstruction = new IfInstruction {
                    Style = "if"
                };

                InitializeIfInstruction(state, expression, ifInstruction);

                var results = new List<ProgramInstruction>(new[] { ifInstruction });

                if (expression.Items.Count == 4) {
                    ProgramInstruction previousBlock = ifInstruction;
                    var nextExpression = expression.Items[3];

                    while (true) {
                        if (!(nextExpression is ListExpression elseExpression) ||
                            elseExpression.Items.Count == 0 ||
                            !(elseExpression.Items[0] is SymbolExpression elseBlockSymbol)) {

                            throw new LizpyInternalCompilationException(
                                $"Invalid if block instruction, else clause must be a valid instruction.",
                                expression
                            );
                        }

                        if (elseBlockSymbol.Value == "if") {
                            // Attach as else-if
                            var elseIfBlock = new ElseIfInstruction {
                                Style = "else-if"
                            };

                            InitializeIfInstruction(state, elseExpression, elseIfBlock);

                            previousBlock.Next = elseIfBlock;
                            previousBlock = elseIfBlock;
                            results.Add(elseIfBlock);

                            if (elseExpression.Items.Count == 4) {
                                nextExpression = elseExpression.Items[3];
                            } else {
                                break;
                            }
                        } else {
                            // Attach as else
                            var elseBlock = new ElseIfInstruction {
                                Style = "else"
                            };

                            // Else blocks are actually ElseIf blocks with a hidden true condition
                            elseBlock.InitializeExpressions(new ConstantExpression(true));

                            var instructions = state.CompileInstruction(elseExpression);

                            elseBlock.FirstChild = instructions.First();
                            previousBlock.Next = elseBlock;
                            results.Add(elseBlock);

                            break;
                        }
                    }

                }

                return results.ToArray();
            } else if (function == "cond") {
                /* (cond
                 *     condition1 instruction1
                 *     condition2 instruction2
                 *     defaultInstruction)
                 */

                if (expression.Items.Count < 3) {
                    throw new LizpyInternalCompilationException(
                        $"Invalid function call, argument count mismatch. Expected: 3+, Actual: {expression.Items.Count - 1}",
                        expression
                    );
                }

                var ifInstruction = new IfInstruction {
                    Style = "if"
                };

                ifInstruction.InitializeExpressions(state.CompileExpression(expression.Items[1]));
                ifInstruction.FirstChild = state.CompileInstruction(expression.Items[2]).First();

                var results = new List<ProgramInstruction>(new[] { ifInstruction });

                var previousInstruction = ifInstruction;
                for (var i = 3; i < expression.Items.Count; i++) {
                    if (i + 1 < expression.Items.Count) {
                        // Else If block
                        var elseIfInstruction = new ElseIfInstruction {
                            Style = "else-if"
                        };

                        elseIfInstruction.InitializeExpressions(
                            state.CompileExpression(expression.Items[i]));
                        elseIfInstruction.FirstChild =
                            state.CompileInstruction(expression.Items[i + 1]).First();

                        previousInstruction.Next = elseIfInstruction;
                        previousInstruction = elseIfInstruction;
                        results.Add(elseIfInstruction);
                    } else {
                        // Else block
                        var elseInstruction = new ElseIfInstruction {
                            Style = "else"
                        };

                        elseInstruction.InitializeExpressions(
                            state.CompileExpression(expression.Items[i]));
                        elseInstruction.FirstChild =
                            state.CompileInstruction(expression.Items[i + 1]).First();

                        previousInstruction.Next = elseInstruction;
                        previousInstruction = elseInstruction;
                        results.Add(elseInstruction);
                    }
                }

                return results.ToArray();
            } else {
                throw new NotSupportedException($"Unsupported Symbol {expression.Items[0]}");
            }
        }

        private static void InitializeIfInstruction(
            CompilerState state,
            ListExpression expression,
            ProgramInstruction ifInstruction) {

            if (expression.Items.Count < 2) {
                throw new LizpyInternalCompilationException(
                    $"Invalid function call, argument count mismatch. Expected: 1 - 3, Actual: {expression.Items.Count - 1}",
                    expression
                );
            } else if (expression.Items.Count > 4) {
                throw new LizpyInternalCompilationException(
                    $"Invalid function call, argument count mismatch. Expected: 1 - 3, Actual: {expression.Items.Count - 1}",
                    expression
                );
            }

            ifInstruction.InitializeExpressions(state.CompileExpression(expression.Items[1]));
            var childInstructions = state.CompileInstruction(expression.Items[2]);
            ifInstruction.FirstChild = childInstructions.First();
        }
    }
}
