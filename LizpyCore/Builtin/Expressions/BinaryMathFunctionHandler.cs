using System;
using System.Collections.Generic;
using System.Linq;
using Lizpy.Compiler;
using Lizpy.Internal;
using Lizpy.Syntax;
using ModApi.Craft.Program;
using ModApi.Craft.Program.Expressions;

namespace Lizpy.Builtin.Expressions {
    public class BinaryMathFunctionHandler : IExpressionFunctionHandler {
        private static readonly String[] MathFunctionSymbols = {
            "+",
            "-",
            "/",
            "*",
            "^",
            "%",
            "rand",
            "min",
            "max",
            "atan2"
        };

        private static readonly IReadOnlyDictionary<String, (String style, Boolean varargs)> MathFunctionInfoMapping =
            new Dictionary<String, (String, Boolean)> {
                { "+", ("op-add", true) },
                { "-", ("op-sub", false) },
                { "/", ("op-div", false) },
                { "*", ("op-mul", true) },
                { "^", ("op-exp", false) },
                { "%", ("op-mod", false) },
                { "rand", ("op-rand", false) },
                { "min", ("op-min", true) },
                { "max", ("op-max", true) },
                { "atan2", ("op-atan-2", false) }
            };

        public String Namespace => String.Empty;

        public IEnumerable<String> SupportedSymbols => MathFunctionSymbols;

        public ProgramExpression CompileFunction(
            CompilerState state,
            ListExpression expression) {

            var op = ((SymbolExpression)expression.Items[0]).GetLocalName();

            if (!MathFunctionInfoMapping.TryGetValue(op, out var info)) {
                throw new NotSupportedException($"Unsupported Symbol {expression.Items[0]}");
            }

            if (!info.varargs && expression.Items.Count != 3) {
                throw new LizpyInternalCompilationException(
                    $"Invalid function call, argument count mismatch. Expected: 2, Actual: {expression.Items.Count - 1}",
                    expression
                );
            } else if (info.varargs && expression.Items.Count < 3) {
                throw new LizpyInternalCompilationException(
                    $"Invalid function call, argument count mismatch. Expected: 2+, Actual: {expression.Items.Count - 1}",
                    expression
                );
            }

            BinaryOperatorExpression operation;
            if (info.varargs && expression.Items.Count > 3) {
                operation = MakeVarArgsExpression(state, op, info.style, expression.Items.Skip(1).ToList());
            } else {
                operation = new BinaryOperatorExpression {
                    Operator = op,
                    Style = info.style
                };

                operation.InitializeExpressions(
                    state.CompileExpression(expression.Items[1]),
                    state.CompileExpression(expression.Items[2])
                );
            }

            return operation;
        }

        private static BinaryOperatorExpression MakeVarArgsExpression(
            CompilerState state,
            string op,
            string style,
            IReadOnlyList<SExpression> expressions) {

            var operation = new BinaryOperatorExpression {
                Operator = op,
                Style = style
            };

            operation.InitializeExpressions(
                state.CompileExpression(expressions[0]),
                expressions.Count > 2 ?
                    MakeVarArgsExpression(state, op, style, expressions.Skip(1).ToList()) :
                    state.CompileExpression(expressions[1])
            );

            return operation;
        }
    }
}
