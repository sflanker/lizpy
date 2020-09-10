using System;
using System.Linq;
using ModApi.Craft.Program;
using ModApi.Craft.Program.Expressions;
using ModApi.Craft.Program.Instructions;

namespace Lizpy.Internal {
    public class FlightProgramVisitor<TState> {
        public TState VisitProgram(FlightProgram program, TState initialState = default(TState)) {
            var state = initialState;

            foreach (var variable in program.GlobalVariables.Variables.ToList()) {
                var (nextState, resultVariable) = this.VisitGlobalVariable(variable, state);
                state = nextState;

                if (resultVariable != variable) {
                    program.GlobalVariables.DeleteVariable(variable.Name);

                    if (resultVariable != null) {
                        program.GlobalVariables.AddVariable(resultVariable);
                    }
                }
            }

            foreach (var expression in program.CustomExpressions.ToList()) {
                var (nextState, resultExpression) = this.VisitCustomExpression(expression, state);
                state = nextState;

                if (resultExpression != expression) {
                    program.RemoveCustomExpression(expression);

                    if (resultExpression != null) {
                        program.AddCustomExpression(resultExpression);
                    }
                }
            }

            foreach (var instruction in program.CustomInstructions.ToList()) {
                var (nextState, resultInstruction) = this.VisitCustomInstruction(instruction, state);
                state = nextState;

                if (resultInstruction != instruction) {
                    program.RemoveCustomInstruction(instruction);

                    if (resultInstruction != null) {
                        program.AddCustomInstruction(resultInstruction);
                    }
                }
            }

            foreach (var (instruction, ix) in program.RootInstructions.Select((inst, ix) => (inst, ix)).ToList()) {
                var (nextState, resultInstruction) = this.VisitInstruction(instruction, state);
                state = nextState;

                if (resultInstruction != instruction) {
                    program.RootInstructions.RemoveAt(ix);

                    if (resultInstruction != null) {
                        program.RootInstructions.Insert(ix, resultInstruction);
                    }
                }
            }

            return state;
        }

        protected virtual (TState, Variable) VisitGlobalVariable(
            Variable variable,
            TState state) {

            return (state, variable);
        }

        protected virtual (TState, CustomExpression) VisitCustomExpression(
            CustomExpression expression,
            TState state) {

            return (VisitNodeExpressionsImpl(expression, state), expression);
        }

        protected virtual (TState, CustomInstruction) VisitCustomInstruction(
            CustomInstruction instruction,
            TState state) {

            return (VisitInstructionImpl(instruction, state), instruction);
        }

        protected virtual (TState, ProgramInstruction) VisitInstruction(
            ProgramInstruction instruction,
            TState state) {

            return (VisitInstructionImpl(instruction, state), instruction);
        }

        protected virtual (TState, ProgramExpression) VisitExpression(
            ProgramExpression expression,
            TState state) {

            return (VisitNodeExpressionsImpl(expression, state), expression);
        }

        private TState VisitInstructionImpl(ProgramInstruction instruction, TState state) {
            if (instruction.SupportsChildren && instruction.FirstChild != null) {
                var (nextState, resultInstruction) =
                    this.VisitInstruction(instruction.FirstChild, state);

                state = nextState;
                if (resultInstruction != instruction.FirstChild) {
                    instruction.FirstChild = resultInstruction;
                }
            }

            return VisitInstructionChain(instruction, VisitNodeExpressionsImpl(instruction, state));
        }

        private TState VisitInstructionChain(ProgramInstruction rootInstruction, TState state) {
            var previousInstruction = rootInstruction;
            var currentInstruction = rootInstruction.Next;

            while (currentInstruction != null) {
                var previousNext = currentInstruction.Next;
                var (nextState, resultInstruction) = this.VisitInstruction(currentInstruction, state);
                state = nextState;

                if (resultInstruction != currentInstruction) {
                    // operations:
                    //  * delete all subsequent instructions (return null)
                    //  * replace current instruction (return a chain of instructions, with the final .Next equal to the replaced instruction's .Next)
                    //  * return an new instruction with a .Next pointing to the current or a subsequent instruction.
                    //  * delete instructions (return a subsequent instruction)

                    if (resultInstruction == null) {
                        previousInstruction.Next = null;
                        currentInstruction = null;
                    } else {
                        previousInstruction.Next = resultInstruction;

                        // Do not re-visit any of the returned instructions until we get back to
                        // one of instructions in the original chain.
                        var nextInstruction = resultInstruction;
                        while (nextInstruction != null &&
                            !InChain(nextInstruction, previousNext) &&
                            nextInstruction != currentInstruction) {

                            nextInstruction = nextInstruction.Next;
                        }

                        previousInstruction = currentInstruction;
                        currentInstruction = nextInstruction;
                    }
                } else {
                    previousInstruction = currentInstruction;
                    currentInstruction = currentInstruction.Next;
                }
            }

            return state;
        }

        private TState VisitNodeExpressionsImpl(ProgramNode expression, TState state) {
            if (expression.Expressions != null) {
                for (var i = 0; i < expression.Expressions.Count; i++) {
                    var child = expression.Expressions[i];
                    var (nextState, resultExpression) = this.VisitExpression(child, state);
                    if (resultExpression != child) {
                        expression.SetExpression(i, resultExpression);
                    }

                    state = nextState;
                }
            }

            return state;
        }

        private static Boolean InChain(ProgramInstruction instruction, ProgramInstruction root) {
            while (root != null && instruction != root) {
                root = root.Next;
            }

            return root != null;
        }
    }
}
