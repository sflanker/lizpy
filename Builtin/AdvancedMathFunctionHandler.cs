using System;
using System.Collections.Generic;
using Lizpy.Compiler;
using Lizpy.Internal;
using Lizpy.Syntax;
using ModApi.Craft.Program;
using ModApi.Craft.Program.Expressions;

namespace Lizpy.Builtin {
    public class AdvancedMathFunctionHandler  : IFunctionHandler {
        private static readonly String[] MathFunctionSymbols = {
            "exp",
            "sinh",
            "cosh",
            "tanh",
            "asinh",
            "acosh",
            "atanh",
            "log"
        };

        public string Namespace => String.Empty;

        public IEnumerable<String> SupportedSymbols => MathFunctionSymbols;

        public ProgramNode CompileFunction(
            CompilerState state,
            ListExpression expression) {

            var function = LizpyCompiler.GetLocalName((SymbolExpression)expression.Items[0]);

            if (function == "log") {
                var value = state.CompileExpression(expression.Items[1]);
                switch (expression.Items.Count) {
                    case 2:
                        // Default to log base 10
                        return Vizzy.Log(value);
                    case 3:
                        var @base = state.CompileExpression(expression.Items[2]);
                        return Vizzy.Divide(
                            Vizzy.Log(value),
                            Vizzy.Log(@base)
                        );
                }
            }

            if (expression.Items.Count != 2) {
                throw new LizpyInternalCompilationException(
                    $"Invalid function call, argument count mismatch. Expected: 1, Actual: {expression.Items.Count - 1}",
                    expression
                );
            }

            {
                var value = state.CompileExpression(expression.Items[1]);
                switch (function) {
                    case "exp":
                        return Exp(value);
                    case "sinh":
                        return SinH(value);
                    case "cosh":
                        return CosH(value);
                    case "tanh":
                        return TanH(value);
                    case "asinh":
                        return ArcSinH(value);
                    case "acosh":
                        return ArcCosH(value);
                    case "atanh":
                        return ArcTanH(value);
                    default:
                        throw new NotSupportedException($"Unsupported Symbol {expression.Items[0]}");
                }
            }
        }

        private static ProgramExpression Exp(ProgramExpression exponent) {
            var op = new BinaryOperatorExpression {
                Operator = "^",
                Style = "op-exp"
            };

            op.InitializeExpressions(
                new ConstantExpression(Math.E),
                exponent
            );

            return op;
        }

        private static ProgramExpression SinH(ProgramExpression value) {
            return Vizzy.Divide(
                Vizzy.Subtract(Vizzy.Number(1), Exp(Vizzy.Multiply(Vizzy.Number(-2), value))),
                Vizzy.Multiply(Vizzy.Number(2), Exp(Vizzy.Multiply(Vizzy.Number(-1), value)))
            );
        }

        private static ProgramExpression CosH(ProgramExpression value) {
            return Vizzy.Divide(
                Vizzy.Add(Vizzy.Number(1), Exp(Vizzy.Multiply(Vizzy.Number(-2), value))),
                Vizzy.Multiply(Vizzy.Number(2), Exp(Vizzy.Multiply(Vizzy.Number(-1), value)))
            );
        }

        private static ProgramExpression TanH(ProgramExpression value) {
            return Vizzy.Divide(
                Vizzy.Subtract(Exp(Vizzy.Multiply(Vizzy.Number(2), value)), Vizzy.Number(1)),
                Vizzy.Add(Exp(Vizzy.Multiply(Vizzy.Number(2), value)), Vizzy.Number(1))
            );
        }

        private static ProgramExpression ArcSinH(ProgramExpression value) {
            return Vizzy.Ln(
                Vizzy.Add(
                    value,
                    Vizzy.Sqrt(Vizzy.Add(Vizzy.Pow(value, Vizzy.Number(2)), Vizzy.Number(1)))
                )
            );
        }

        private static ProgramExpression ArcCosH(ProgramExpression value) {
            return Vizzy.Ln(
                Vizzy.Subtract(
                    value,
                    Vizzy.Sqrt(Vizzy.Add(Vizzy.Pow(value, Vizzy.Number(2)), Vizzy.Number(1)))
                )
            );
        }

        private static ProgramExpression ArcTanH(ProgramExpression value) {
            return Vizzy.Divide(
                Vizzy.Ln(Vizzy.Divide(
                    Vizzy.Add(Vizzy.Number(1), value),
                    Vizzy.Subtract(Vizzy.Number(1), value))),
                Vizzy.Number(2)
            );
        }

    }
}
