using System;
using System.Linq;
using Lizpy.Compiler;
using Lizpy.Internal;
using ModApi.Craft.Program;
using ModApi.Craft.Program.Expressions;
using ModApi.Craft.Program.Instructions;

namespace Lizpy.Builtin {
    public class LateBindingPass : ICompilerPass {
        public void ApplyPass(CompilerState state) {
            new LateBindingVisitor().VisitProgram(state.Program, state);
        }

        private class LateBindingVisitor : FlightProgramVisitor<CompilerState> {
            protected override (CompilerState, ProgramInstruction) VisitInstruction(
                ProgramInstruction instruction,
                CompilerState state) {

                if (instruction is LateBoundCallCustomInstruction lateBoundCall) {
                    String vizzyName = null;
                    var found = false;
                    if (lateBoundCall.Reference.IsValidUnNamespacedVariable()) {
                        // Try searching the namespace where this reference was found first
                        var qualifiedReference =
                            LizpyCompiler.ApplyNamespace(lateBoundCall.SourceNamespace, lateBoundCall.Reference);

                        found = state.InstructionNameLookup.TryGetValue(qualifiedReference, out vizzyName);
                    }

                    // Look up as is.
                    if (found ||
                        state.InstructionNameLookup.TryGetValue(lateBoundCall.Reference, out vizzyName)) {

                        var boundInstruction = new CallCustomInstruction {
                            Style = lateBoundCall.Style,
                            Call = vizzyName,
                            Next = instruction.Next
                        };

                        if (lateBoundCall.Expressions != null) {
                            boundInstruction.InitializeExpressions(lateBoundCall.Expressions.ToArray());
                        }

                        // Replace the current instruction with the bound instruction call.
                        return base.VisitInstruction(boundInstruction, state);
                    } else {
                        state.AddError(new CompilerResult(
                            lateBoundCall.Reference,
                            $"The specified custom instruction was not found: {lateBoundCall.Reference}"
                        ));

                        return base.VisitInstruction(instruction, state);
                    }
                } else {
                    return base.VisitInstruction(instruction, state);
                }
            }

            protected override (CompilerState, ProgramExpression) VisitExpression(
                ProgramExpression expression,
                CompilerState state) {

                if (expression is LateBoundCallCustomExpression lateBoundCall) {
                    String vizzyName = null;
                    var found = false;
                    if (lateBoundCall.Reference.IsValidUnNamespacedVariable()) {
                        // Try searching the namespace where this reference was found first
                        var qualifiedReference =
                            LizpyCompiler.ApplyNamespace(lateBoundCall.SourceNamespace, lateBoundCall.Reference);

                        found = state.ExpressionNameLookup.TryGetValue(qualifiedReference, out vizzyName);
                    }

                    // Look up as is.
                    if (found ||
                        state.ExpressionNameLookup.TryGetValue(lateBoundCall.Reference, out vizzyName)) {

                        var boundExpression = new CallCustomExpression {
                            Style = lateBoundCall.Style,
                            Call = vizzyName,
                        };

                        if (lateBoundCall.Expressions != null) {
                            boundExpression.InitializeExpressions(lateBoundCall.Expressions.ToArray());
                        }

                        return base.VisitExpression(boundExpression, state);
                    } else {
                        state.AddError(new CompilerResult(
                            lateBoundCall.Reference,
                            $"The specified custom expression was not found: {lateBoundCall.Reference}"
                        ));

                        return base.VisitExpression(expression, state);
                    }
                } else if (expression is LateBoundVariableExpression lateBoundVariable) {
                    Variable variable = null;
                    if (lateBoundVariable.Reference.IsValidUnNamespacedVariable()) {
                        // Try searching the namespace where this reference was found first
                        var qualifiedReference =
                            LizpyCompiler.ApplyNamespace(
                                    lateBoundVariable.SourceNamespace,
                                    lateBoundVariable.Reference)
                                .ToVizzyNamespacedName();

                        variable = state.Program.GlobalVariables.GetVariable(qualifiedReference);
                    }

                    // Look up as is.
                    if (variable == null) {
                        variable = state.Program.GlobalVariables.GetVariable(
                            lateBoundVariable.Reference.ToVizzyNamespacedName()
                        );
                    }

                    if (variable != null) {
                        var boundExpression = new VariableExpression {
                            Style = lateBoundVariable.Style,
                            VariableName = variable.Name
                        };

                        return base.VisitExpression(boundExpression, state);
                    } else {
                        state.AddError(new CompilerResult(
                            lateBoundVariable.Reference,
                            $"The specified global variable was not found: {lateBoundVariable.Reference}"
                        ));

                        return base.VisitExpression(expression, state);
                    }
                } else {
                    return base.VisitExpression(expression, state);
                }
            }
        }
    }
}
