using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Lizpy.Compiler;
using Lizpy.Internal;
using Lizpy.Syntax;
using ModApi.Craft.Program;
using ModApi.Craft.Program.Expressions;

namespace Lizpy.Builtin.Expressions.Craft {
    public class CoordinateConversionHandler : IExpressionFunctionHandler {
        public static readonly String[] CoordinateConversionSymbols = {
            "pci->lat-lon-agl",
            "lat-lon-agl->pci",
            "local->pci",
            "pci->local"
        };

        public string Namespace => String.Empty;
        public IEnumerable<string> SupportedSymbols => CoordinateConversionSymbols;

        public ProgramExpression CompileFunction(CompilerState state, ListExpression expression) {
            var function = ((SymbolExpression)expression.Items[0]).GetLocalName();

            switch (function) {
                case "pci->lat-lon-agl" when expression.Items.Count != 2:
                    throw new LizpyInternalCompilationException(
                        $"Invalid function call, argument count mismatch. Expected: 1, Actual: {expression.Items.Count - 1}",
                        expression
                    );
                case "pci->lat-lon-agl": {
                    var exp = new PlanetExpression {
                        Style = "planet-to-lat-long-agl"
                    };

                    exp.SetListValue("x", "toLatLongAgl");

                    exp.InitializeExpressions(
                        state.CompileExpression(expression.Items[1])
                    );

                    return exp;
                }
                case "lat-lon-agl->pci" when expression.Items.Count != 2:
                    throw new LizpyInternalCompilationException(
                        $"Invalid function call, argument count mismatch. Expected: 1, Actual: {expression.Items.Count - 1}",
                        expression
                    );
                case "lat-lon-agl->pci": {
                    var exp = new PlanetExpression {
                        Style = "planet-to-position"
                    };

                    exp.SetListValue("x", "toPosition");

                    exp.InitializeExpressions(
                        state.CompileExpression(expression.Items[1])
                    );

                    return exp;
                }
                case "local->pci" when expression.Items.Count != 3:
                    throw new LizpyInternalCompilationException(
                        $"Invalid function call, argument count mismatch. Expected: 2, Actual: {expression.Items.Count - 1}",
                        expression
                    );
                case "local->pci": {
                    var exp = (CraftPropertyExpression)
                        ProgramSerializer.DeserializeProgramNode(
                            new XElement(
                                "CraftProperty",
                                new XAttribute("style", "part-transform"),
                                new XAttribute("property", "Part.LocalToPci")));

                    exp.InitializeExpressions(
                        expression.Items.Skip(1).Select(state.CompileExpression).ToArray()
                    );

                    return exp;
                }
                case "pci->local" when expression.Items.Count != 3:
                    throw new LizpyInternalCompilationException(
                        $"Invalid function call, argument count mismatch. Expected: 2, Actual: {expression.Items.Count - 1}",
                        expression
                    );
                case "pci->local": {
                    var exp = (CraftPropertyExpression)
                        ProgramSerializer.DeserializeProgramNode(
                            new XElement(
                                "CraftProperty",
                                new XAttribute("style", "part-transform"),
                                new XAttribute("property", "Part.PciToLocal")));

                    exp.InitializeExpressions(
                        expression.Items.Skip(1).Select(state.CompileExpression).ToArray()
                    );

                    return exp;
                }
                default:
                    throw new NotSupportedException($"Unsupported Symbol {expression.Items[0]}");
            }
        }
    }
}
