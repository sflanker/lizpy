using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Lizpy.Compiler;
using Lizpy.Internal;
using Lizpy.Syntax;
using ModApi.Craft.Program;
using ModApi.Craft.Program.Expressions;
using ExpressionType = Lizpy.Syntax.ExpressionType;

namespace Lizpy.Builtin.Expressions.Craft {
    public class CraftPropertyFunctionHandler : IExpressionFunctionHandler {
        private static readonly String[] CraftPropertySymbols = {
            "info",
            "activation-group",
            "craft-info",
            "craft-id",
            "part-info",
            "part-id",
        };

        private static readonly ISet<String> SupportedInfoCategories = new HashSet<String> {
            "Altitude",
            "Orbit",
            "Atmosphere",
            "Performance",
            "Fuel",
            "Nav",
            "Velocity",
            "Input",
            "Misc",
            "Time",
            "Name"
        };

        public String Namespace => "craft";
        public IEnumerable<String> SupportedSymbols => CraftPropertySymbols;

        public ProgramExpression CompileFunction(CompilerState state, ListExpression expression) {
            var function = ((SymbolExpression)expression.Items[0]).GetLocalName();

            switch (function) {
                case "info" when expression.Items.Count != 2:
                    throw new LizpyInternalCompilationException(
                        $"Invalid function call, argument count mismatch. Expected: 1, Actual: {expression.Items.Count - 1}",
                        expression
                    );
                case "info": {
                    if (!(expression.Items[1] is SymbolExpression craftPropertySymbol)) {
                        throw new LizpyInternalCompilationException(
                            $"Invalid craft/info call. Expected: {ExpressionType.Symbol}, Actual: {expression.Items[1].Type}",
                            expression
                        );
                    }

                    if (!craftPropertySymbol.Value.StartsWith(":")) {
                        throw new LizpyInternalCompilationException(
                            $"Invalid craft/info call. The craft property symbol must be a keyword (start with a colon), Actual: {expression.Items[1]}",
                            expression.Items[1]
                        );
                    }

                    var property = CraftProperties.GetProperty(
                        craftPropertySymbol.Value.Substring(1)
                    );

                    if (property == null) {
                        throw new LizpyInternalCompilationException(
                            $"Invalid craft/info call. The specified craft property is not recognized: {craftPropertySymbol}",
                            expression
                        );
                    } else if (!SupportedInfoCategories.Contains(property.Category)) {
                        throw new LizpyInternalCompilationException(
                            $"Invalid craft/info call. The specified craft property is not supported: {craftPropertySymbol}",
                            expression
                        );
                    }

                    var exp = (CraftPropertyExpression)
                        ProgramSerializer.DeserializeProgramNode(
                            new XElement(
                                "CraftProperty",
                                new XAttribute("style", $"prop-{property.Category.ToLower()}"),
                                new XAttribute("property", property.XmlName)));

                    return exp;
                }
                case "activation-group" when expression.Items.Count != 2:
                    throw new LizpyInternalCompilationException(
                        $"Invalid function call, argument count mismatch. Expected: 1, Actual: {expression.Items.Count - 1}",
                        expression
                    );
                case "activation-group": {
                    var exp = new ActivationGroupExpression {
                        Style = "activation-group"
                    };

                    exp.InitializeExpressions(
                        state.CompileExpression(expression.Items[1])
                    );

                    return exp;
                }
                case "craft-info" when expression.Items.Count != 3:
                    throw new LizpyInternalCompilationException(
                        $"Invalid function call, argument count mismatch. Expected: 2, Actual: {expression.Items.Count - 1}",
                        expression
                    );
                case "craft-info": {
                    if (!(expression.Items[1] is SymbolExpression craftPropertySymbol)) {
                        throw new LizpyInternalCompilationException(
                            $"Invalid craft/craft-info call. Expected: {ExpressionType.Symbol}, Actual: {expression.Items[1].Type}",
                            expression
                        );
                    }

                    if (!craftPropertySymbol.Value.StartsWith(":")) {
                        throw new LizpyInternalCompilationException(
                            $"Invalid craft/craft-info call. The craft property symbol must be a keyword (start with a colon), Actual: {expression.Items[1]}",
                            expression.Items[1]
                        );
                    }

                    var propertyName = craftPropertySymbol.Value.Substring(1);
                    var property = CraftProperties.GetProperty(
                        propertyName.StartsWith("Craft.") ? propertyName : $"Craft.{propertyName}"
                    );

                    if (property == null) {
                        throw new LizpyInternalCompilationException(
                            $"Invalid craft/craft-info call. The specified craft property is not recognized: {craftPropertySymbol}",
                            expression
                        );
                    } else if (property.Category != "Craft") {
                        throw new LizpyInternalCompilationException(
                            $"Invalid craft/craft-info call. The specified craft property is not supported: {craftPropertySymbol}",
                            expression
                        );
                    }

                    var exp = (CraftPropertyExpression)
                        ProgramSerializer.DeserializeProgramNode(
                            new XElement(
                                "CraftProperty",
                                new XAttribute("style", "craft"),
                                new XAttribute("property", property.XmlName)));

                    exp.InitializeExpressions(
                        state.CompileExpression(expression.Items[2])
                    );

                    return exp;
                }
                case "craft-id" when expression.Items.Count != 2:
                    throw new LizpyInternalCompilationException(
                        $"Invalid function call, argument count mismatch. Expected: 1, Actual: {expression.Items.Count - 1}",
                        expression
                    );
                case "craft-id": {
                    var exp = (CraftPropertyExpression)
                        ProgramSerializer.DeserializeProgramNode(
                            new XElement(
                                "CraftProperty",
                                new XAttribute("style", "craft-id"),
                                new XAttribute("property", "Craft.NameToID")));

                    exp.InitializeExpressions(
                        state.CompileExpression(expression.Items[1])
                    );

                    return exp;
                }
                case "part-info" when expression.Items.Count < 2:
                    throw new LizpyInternalCompilationException(
                        $"Invalid function call, argument count mismatch. Expected: 1-2, Actual: {expression.Items.Count - 1}",
                        expression
                    );
                case "part-info": {
                    if (!(expression.Items[1] is SymbolExpression craftPropertySymbol)) {
                        throw new LizpyInternalCompilationException(
                            $"Invalid craft/part-info call. Expected: {ExpressionType.Symbol}, Actual: {expression.Items[1].Type}",
                            expression
                        );
                    }

                    if (!craftPropertySymbol.Value.StartsWith(":")) {
                        throw new LizpyInternalCompilationException(
                            $"Invalid craft/part-info call. The part property symbol must be a keyword (start with a colon), Actual: {expression.Items[1]}",
                            expression.Items[1]
                        );
                    }

                    var propertyName = craftPropertySymbol.Value.Substring(1);
                    var property = CraftProperties.GetProperty(
                        propertyName.StartsWith("Part.") ? propertyName : $"Part.{propertyName}"
                    );

                    if (property == null) {
                        throw new LizpyInternalCompilationException(
                            $"Invalid craft/part-info call. The specified craft property is not recognized: {craftPropertySymbol}",
                            expression
                        );
                    } else if (property.Category != "Part") {
                        throw new LizpyInternalCompilationException(
                            $"Invalid craft/part-info call. The specified craft property is not supported: {craftPropertySymbol}",
                            expression
                        );
                    }

                    var exp = (CraftPropertyExpression)
                        ProgramSerializer.DeserializeProgramNode(
                            new XElement(
                                "CraftProperty",
                                new XAttribute("style", "part"),
                                new XAttribute("property", property.XmlName)));

                    if (property.XmlName.EndsWith("ID")) {
                        if (expression.Items.Count != 2) {
                            throw new LizpyInternalCompilationException(
                                $"Invalid function call, argument count mismatch. Expected: 2, Actual: {expression.Items.Count - 1}",
                                expression
                            );
                        }
                    } else {
                        if (expression.Items.Count != 3) {
                            throw new LizpyInternalCompilationException(
                                $"Invalid function call, argument count mismatch. Expected: 3, Actual: {expression.Items.Count - 1}",
                                expression
                            );
                        }

                        exp.InitializeExpressions(
                            state.CompileExpression(expression.Items[2])
                        );
                    }

                    return exp;
                }
                case "part-id": {
                    var exp = (CraftPropertyExpression)
                        ProgramSerializer.DeserializeProgramNode(
                            new XElement(
                                "CraftProperty",
                                new XAttribute("style", "part-id"),
                                new XAttribute("property", "Part.NameToID")));

                    exp.InitializeExpressions(
                        state.CompileExpression(expression.Items[1])
                    );

                    return exp;
                }
                default:
                    throw new NotSupportedException($"Unsupported Symbol {expression.Items[0]}");
            }
        }
    }
}
