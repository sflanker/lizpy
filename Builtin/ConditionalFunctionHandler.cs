using System;
using System.Collections.Generic;
using System.Linq;
using Lizpy.Compiler;
using Lizpy.Internal;
using Lizpy.Syntax;
using ModApi.Craft.Program;
using ModApi.Craft.Program.Expressions;

namespace Lizpy.Builtin {
    public class ConditionalFunctionHandler : IFunctionHandler {
        private static readonly String[] ConditionalFunctionSymbols = {
            "if",
            "case",
            "scase",
            "cond",
        };

        public string Namespace => String.Empty;

        public IEnumerable<String> SupportedSymbols => ConditionalFunctionSymbols;

        public ProgramNode CompileFunction(CompilerState state, ListExpression expression) {
            var function = LizpyCompiler.GetLocalName((SymbolExpression)expression.Items[0]);

            switch (function) {
                case "if" when expression.Items.Count != 3:
                    throw new LizpyInternalCompilationException(
                        $"Invalid conditional expression, argument count mismatch. Expected: 3, Actual: {expression.Items.Count - 1}",
                        expression
                    );
                case "if": {
                    var conditional = new ConditionalExpression {
                        Style = "conditional"
                    };

                    conditional.InitializeExpressions(
                        state.CompileExpression(expression.Items[1]),
                        state.CompileExpression(expression.Items[2]),
                        state.CompileExpression(expression.Items[3])
                    );

                    return conditional;
                }
                // (case value
                //     match1 result1
                //     match2 result2
                //     defaultResult)
                case "case" when expression.Items.Count < 4:
                    throw new LizpyInternalCompilationException(
                        $"Invalid case expression, argument count mismatch. Expected at least: 4, Actual: {expression.Items.Count - 1}",
                        expression
                    );
                case "case": {
                    var compiledExpressions =
                        expression.Items.Skip(1).Select(state.CompileExpression).ToList();

                    return BuildCaseExpression(compiledExpressions[0], compiledExpressions.Skip(1).GetEnumerator());
                }
                // (scase stringValue
                //     match1 result1
                //     match2 result2
                //     defaultResult)
                case "scase" when expression.Items.Count < 4:
                    throw new LizpyInternalCompilationException(
                        $"Invalid case expression, argument count mismatch. Expected at least: 4, Actual: {expression.Items.Count - 1}",
                        expression
                    );
                case "scase": {
                    var compiledExpressions =
                        expression.Items.Skip(1).Select(state.CompileExpression).ToList();

                    return BuildStringCaseExpression(compiledExpressions[0], compiledExpressions.Skip(1).GetEnumerator());
                }
                /*
                 * (cond
                 *     condition1 result1
                 *     condition2 result2
                 *     defaultResult)
                 */
                case "cond" when expression.Items.Count < 3:
                    throw new LizpyInternalCompilationException(
                        $"Invalid case expression, argument count mismatch. Expected at least: 4, Actual: {expression.Items.Count - 1}",
                        expression
                    );
                case "cond": {
                    var compiledExpressions =
                        expression.Items.Skip(1).Select(state.CompileExpression).ToList();

                    return BuildCondExpression(compiledExpressions.GetEnumerator());
                }
                default:
                    throw new NotSupportedException($"Unsupported Symbol {expression.Items[0]}");
            }
        }

        private static ProgramExpression BuildCaseExpression(
            ProgramExpression value,
            IEnumerator<ProgramExpression> caseEnumerator) {

            if (!caseEnumerator.MoveNext()) {
                return new ConstantExpression(0);
            }

            var value1 = caseEnumerator.Current;

            if (!caseEnumerator.MoveNext()) {
                return value1;
            }

            var value2 = caseEnumerator.Current;

            var conditional = new ConditionalExpression {
                Style = "conditional"
            };

            conditional.InitializeExpressions(
                Vizzy.Equal(value, value1),
                value2,
                BuildCaseExpression(value, caseEnumerator)
            );

            return conditional;
        }

        private static ProgramExpression BuildStringCaseExpression(
            ProgramExpression value,
            IEnumerator<ProgramExpression> caseEnumerator) {

            if (!caseEnumerator.MoveNext()) {
                return new ConstantExpression(0);
            }

            var value1 = caseEnumerator.Current;

            if (!caseEnumerator.MoveNext()) {
                return value1;
            }

            var value2 = caseEnumerator.Current;

            var conditional = new ConditionalExpression {
                Style = "conditional"
            };

            conditional.InitializeExpressions(
                VizzyEx.StringEqual(value, value1),
                value2,
                BuildCaseExpression(value, caseEnumerator)
            );

            return conditional;
        }

        private static ProgramExpression BuildCondExpression(IEnumerator<ProgramExpression> conditionEnumerator) {
            if (!conditionEnumerator.MoveNext()) {
                return new ConstantExpression(0);
            }

            var value1 = conditionEnumerator.Current;

            if (!conditionEnumerator.MoveNext()) {
                return value1;
            }

            var value2 = conditionEnumerator.Current;

            var conditional = new ConditionalExpression {
                Style = "conditional"
            };

            conditional.InitializeExpressions(
                value1,
                value2,
                BuildCondExpression(conditionEnumerator)
            );

            return conditional;
        }
    }
}
