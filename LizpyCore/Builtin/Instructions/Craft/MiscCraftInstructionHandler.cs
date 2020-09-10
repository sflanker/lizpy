using System;
using System.Collections.Generic;
using System.Linq;
using Lizpy.Compiler;
using Lizpy.Internal;
using Lizpy.Syntax;
using ModApi.Craft.Program.Instructions;

namespace Lizpy.Builtin.Instructions.Craft {
    public class MiscCraftInstructionHandler : IInstructionFunctionHandler {
        public static readonly String[] CraftInstructionSymbols = {
            "activate-stage!",
            "set-input!",
            "set-target!",
            "set-heading!",
            "set-pitch!",
            "set-autopilot-mode!",
            "set-activation-group!",
            "set-time-mode!",
            "switch-craft!",
        };

        public string Namespace => "craft";
        public IEnumerable<string> SupportedSymbols => CraftInstructionSymbols;

        public ProgramInstruction[] CompileFunction(
            CompilerState state,
            ListExpression expression) {

            var function = ((SymbolExpression)expression.Items[0]).GetLocalName();

            switch (function) {
                case "activate-stage!" when expression.Items.Count != 1:
                    throw new LizpyInternalCompilationException(
                        $"Invalid function call, argument count mismatch. Expected: 0, Actual: {expression.Items.Count - 1}",
                        expression
                    );
                case "activate-stage!": {
                    var inst = new ActivateStageInstruction {
                        Style = "activate=stage"
                    };

                    return new ProgramInstruction[] { inst };
                }
                case "set-input!" when expression.Items.Count != 3:
                    throw new LizpyInternalCompilationException(
                        $"Invalid function call, argument count mismatch. Expected: 2, Actual: {expression.Items.Count - 1}",
                        expression
                    );
                case "set-input!": {
                    if (!(expression.Items[1] is SymbolExpression inputTypeSymbol)) {
                        throw new LizpyInternalCompilationException(
                            $"Invalid craft/set-input! call. Expected: {ExpressionType.Symbol}, Actual: {expression.Items[1].Type}",
                            expression.Items[1]
                        );
                    }

                    if (!inputTypeSymbol.Value.StartsWith(":")) {
                        throw new LizpyInternalCompilationException(
                            $"Invalid craft/set-input! call. The input type symbol must be a keyword (start with a colon), Actual: {expression.Items[1]}",
                            expression.Items[1]
                        );
                    }

                    var inputType =
                        // Support things like :Throttle as well as :throttle for consistency with
                        // craft info expressions.
                        inputTypeSymbol.Value.Substring(1, 1).ToLower() +
                        inputTypeSymbol.Value.Substring(2);

                    var inst = new SetCraftInputInstruction {
                        Style = "set-input"
                    };

                    if (inst.GetListItems("x").All(li => li.Id != inputType)) {
                        throw new LizpyInternalCompilationException(
                            $"Invalid craft/set-input! call. Unrecognized input type: {inputType}",
                            inputTypeSymbol
                        );
                    }

                    inst.SetListValue("x", inputType);

                    inst.InitializeExpressions(
                        state.CompileExpression(expression.Items[2])
                    );

                    return new ProgramInstruction[] { inst };
                }
                case "set-target!" when expression.Items.Count != 2:
                    throw new LizpyInternalCompilationException(
                        $"Invalid function call, argument count mismatch. Expected: 1, Actual: {expression.Items.Count - 1}",
                        expression
                    );
                case "set-target!": {
                    var inst = new SetTargetInstruction {
                        Style = "set-target"
                    };

                    inst.InitializeExpressions(
                        state.CompileExpression(expression.Items[1])
                    );

                    return new ProgramInstruction[] { inst };
                }
                case "set-heading!" when expression.Items.Count != 2:
                    throw new LizpyInternalCompilationException(
                        $"Invalid function call, argument count mismatch. Expected: 1, Actual: {expression.Items.Count - 1}",
                        expression
                    );
                case "set-heading!": {
                    var inst = new SetTargetHeadingInstruction {
                        Style = "set-target"
                    };

                    inst.SetListValue("x", "heading");

                    inst.InitializeExpressions(
                        state.CompileExpression(expression.Items[1])
                    );

                    return new ProgramInstruction[] { inst };
                }
                case "set-pitch!" when expression.Items.Count != 2:
                    throw new LizpyInternalCompilationException(
                        $"Invalid function call, argument count mismatch. Expected: 1, Actual: {expression.Items.Count - 1}",
                        expression
                    );
                case "set-pitch!": {
                    var inst = new SetTargetHeadingInstruction {
                        Style = "set-target"
                    };

                    inst.SetListValue("x", "pitch");

                    inst.InitializeExpressions(
                        state.CompileExpression(expression.Items[1])
                    );

                    return new ProgramInstruction[] { inst };
                }
                case "set-autopilot-mode!" when expression.Items.Count != 2:
                    throw new LizpyInternalCompilationException(
                        $"Invalid function call, argument count mismatch. Expected: 1, Actual: {expression.Items.Count - 1}",
                        expression
                    );
                case "set-autopilot-mode!": {
                    if (!(expression.Items[1] is SymbolExpression autopilotModeSymbol)) {
                        throw new LizpyInternalCompilationException(
                            $"Invalid craft/set-autopilot-mode! call. Expected: {ExpressionType.Symbol}, Actual: {expression.Items[1].Type}",
                            expression.Items[1]
                        );
                    }

                    if (!autopilotModeSymbol.Value.StartsWith(":")) {
                        throw new LizpyInternalCompilationException(
                            $"Invalid craft/set-autopilot-mode! call. The autopilot mode symbol must be a keyword (start with a colon), Actual: {autopilotModeSymbol}",
                            autopilotModeSymbol
                        );
                    }

                    var autopilotMode = autopilotModeSymbol.Value.Substring(1);

                    var inst = new LockNavSphereInstruction {
                        Style = "lock-nav-sphere"
                    };

                    if (inst.GetListItems("x").All(li => li.Id != autopilotMode)) {
                        throw new LizpyInternalCompilationException(
                            $"Invalid craft/set-autopilot-mode! call. Unrecognized autopilot mode: {autopilotMode}",
                            autopilotModeSymbol
                        );
                    }

                    inst.SetListValue("x", autopilotMode);

                    return new ProgramInstruction[] { inst };
                }
                case "set-activation-group!" when expression.Items.Count != 3:
                    throw new LizpyInternalCompilationException(
                        $"Invalid function call, argument count mismatch. Expected: 1, Actual: {expression.Items.Count - 1}",
                        expression
                    );
                case "set-activation-group!": {
                    var inst = new SetActivationGroupInstruction {
                        Style = "set-ag"
                    };

                    inst.InitializeExpressions(
                        expression.Items.Skip(1).Select(state.CompileExpression).ToArray()
                    );

                    return new ProgramInstruction[] { inst };
                }
                case "set-time-mode!" when expression.Items.Count != 2:
                    throw new LizpyInternalCompilationException(
                        $"Invalid function call, argument count mismatch. Expected: 1, Actual: {expression.Items.Count - 1}",
                        expression
                    );
                case "set-time-mode!": {
                    if (!(expression.Items[1] is SymbolExpression timeModeSymbol)) {
                        throw new LizpyInternalCompilationException(
                            $"Invalid craft/set-time-mode! call. Expected: {ExpressionType.Symbol}, Actual: {expression.Items[1].Type}",
                            expression.Items[1]
                        );
                    }

                    if (!timeModeSymbol.Value.StartsWith(":")) {
                        throw new LizpyInternalCompilationException(
                            $"Invalid craft/set-time-mode! call. The time mode symbol must be a keyword (start with a colon), Actual: {timeModeSymbol}",
                            timeModeSymbol
                        );
                    }

                    var timeMode = timeModeSymbol.Value.Substring(1);

                    var inst = new SetTimeModeInstruction {
                        Style = "set-time-mode"
                    };

                    if (inst.GetListItems("x").All(li => li.Id != timeMode)) {
                        throw new LizpyInternalCompilationException(
                            $"Invalid craft/set-time-mode! call. Unrecognized time mode: {timeMode}",
                            timeModeSymbol
                        );
                    }

                    inst.SetListValue("x", timeMode);

                    return new ProgramInstruction[] { inst };
                }
                case "switch-craft!" when expression.Items.Count != 2:
                    throw new LizpyInternalCompilationException(
                        $"Invalid function call, argument count mismatch. Expected: 1, Actual: {expression.Items.Count - 1}",
                        expression
                    );
                case "switch-craft!": {
                    var inst = new SwitchCraftInstruction {
                        Style = "switch-craft"
                    };

                    inst.InitializeExpressions(
                        state.CompileExpression(expression.Items[1])
                    );

                    return new ProgramInstruction[] { inst };
                }
                default:
                    throw new NotSupportedException($"Unsupported Symbol {expression.Items[0]}");
            }
        }
    }
}
