using System;
using System.Collections.Generic;
using System.Linq;
using Lizpy.Compiler;
using Lizpy.Internal;
using Lizpy.Syntax;
using ModApi.Craft.Program;
using ModApi.Craft.Program.Expressions;
using ExpressionType = Lizpy.Syntax.ExpressionType;

namespace Lizpy.Builtin.Expressions.Craft {
    public class PlanetInfoFunctionHandler : IExpressionFunctionHandler {
        public static readonly String[] PlanetInfoSymbols = { "info" };

        public string Namespace => "planet";
        public IEnumerable<string> SupportedSymbols => PlanetInfoSymbols;

        public ProgramExpression CompileFunction(
            CompilerState state,
            ListExpression expression) {

            if (expression.Items.Count != 3) {
                throw new LizpyInternalCompilationException(
                    $"Invalid function call, argument count mismatch. Expected: 2, Actual: {expression.Items.Count - 1}",
                    expression
                );
            }

            if (!(expression.Items[1] is SymbolExpression planetPropertySymbol)) {
                throw new LizpyInternalCompilationException(
                    $"Invalid planet/info call. Expected: {ExpressionType.Symbol}, Actual: {expression.Items[1].Type}",
                    expression.Items[1]
                );
            }
            if (!planetPropertySymbol.Value.StartsWith(":")) {
                throw new LizpyInternalCompilationException(
                    $"Invalid planet/info call. The planet property symbol must be a keyword (start with a colon), Actual: {expression.Items[1]}",
                    expression.Items[1]
                );
            }

            var exp = new PlanetExpression {
                Style = "planet"
            };

            var property =
                // Support things like SolarPosition as well as solarPosition for consistency with
                // craft info expressions.
                planetPropertySymbol.Value.Substring(1, 1).ToLower() +
                planetPropertySymbol.Value.Substring(2);

            if (exp.GetListItems("x").All(li => li.Id != property)) {
                throw new LizpyInternalCompilationException(
                    $"Invalid planet/info call. Unrecognized planet property: {planetPropertySymbol}",
                    planetPropertySymbol
                );
            }

            exp.SetListValue(
                "x",
                property
            );

            exp.InitializeExpressions(
                state.CompileExpression(expression.Items[2])
            );

            return exp;
        }
    }
}
