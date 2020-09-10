using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Lizpy.Compiler;
using Lizpy.Internal;
using Lizpy.Syntax;
using ModApi.Craft.Program;
using ModApi.Craft.Program.Expressions;
using ModApi.Craft.Program.Instructions;
using UnityEngine;

namespace Lizpy.Builtin.Declarations {
    public class EventDeclarationHandler : DeclarationHandlerBase {
        private static readonly String[] EventDeclarationSymbols = {
            "on-start",
            "on-part-collided",
            "on-part-exploded",
            "on-craft-docked",
            "on-entered-soi",
            "on-message-received"
        };

        private static readonly IReadOnlyDictionary<String, (String style, String @event, String[] variables)> EventInfo =
            new Dictionary<String, (String, String, String[])> {
                { "on-start", ("flight-start", "FlightStart", new String[0]) },
                { "on-part-collided", ("part-collision", "PartCollision", new[] { "part", "other", "velocity", "impulse" }) },
                { "on-part-exploded", ("part-explode", "PartExplode", new[] { "part" }) },
                { "on-craft-docked", ("craft-docked", "Docked", new[] { "craftA", "craftB" }) },
                { "on-entered-soi", ("change-soi", "ChangeSoi", new[] { "planet" }) },
            };

        public override IEnumerable<String> SupportedSymbols => EventDeclarationSymbols;

        public override CompilerResult ApplyDeclaration(
            CompilerState state,
            ListExpression declaration) {

            var identifierSymbol = (SymbolExpression)declaration.Items[0];
            var function = identifierSymbol.GetLocalName();

            EventInstruction eventDeclaration;
            IEnumerable<SExpression> instructionExpressions;
            String[] eventVariables;

            if (EventInfo.TryGetValue(function, out var info)) {
                eventDeclaration = (EventInstruction)
                    ProgramSerializer.DeserializeProgramNode(
                        new XElement("Event",
                            new XAttribute("style", info.style),
                            new XAttribute("event", info.@event)));

                instructionExpressions = declaration.Items.Skip(1);
                eventVariables = info.variables;
            } else if (function == "on-message-received") {
                if (declaration.Items.Count < 2) {
                    return new CompilerResult(
                        declaration,
                        $"Invalid event declaration, argument count mismatch. Expected: 1+, Actual: {declaration.Items.Count - 1}"
                    );
                }

                if (!(declaration.Items[1] is StringExpression message)) {
                    return new CompilerResult(
                        declaration.Items[1],
                        $"Invalid on-message-received declaration. The message expression must be a string constant, Actual: {declaration.Items[1]}"
                    );
                }

                eventDeclaration = (EventInstruction)
                    ProgramSerializer.DeserializeProgramNode(
                        new XElement("Event",
                            new XAttribute("style", "receive-msg"),
                            new XAttribute("event", "ReceiveMessage")));

                eventDeclaration.InitializeExpressions(
                    new ConstantExpression(message.Value) {
                        CanReplaceInUI = false
                    }
                );

                instructionExpressions = declaration.Items.Skip(2);
                eventVariables = new[] { "data" };
            } else {
                throw new NotSupportedException($"Unsupported Symbol {declaration.Items[0]}");
            }

            ProgramInstruction previous = eventDeclaration;

            var localScope = eventVariables.Select(v => new SymbolExpression(v)).ToList();
            state.PushScope(localScope);
            try {
                foreach (var instructionExpression in instructionExpressions) {
                    var inst = state.CompileInstruction(instructionExpression);
                    previous.Next = inst[0];
                    previous = inst.Last();
                }
            } finally {
                state.PopScope();
            }

            if (identifierSymbol.TryGetMetadata<ArrayExpression>(PositionKey, out var position) &&
                position.Items.Count == 2 &&
                position.Items[0] is INumberExpression x &&
                position.Items[1] is INumberExpression y) {
                eventDeclaration.EditorPosition =
                    new Vector2((Single)x.AsDouble(), (Single)y.AsDouble());
            }

            state.Program.RootInstructions.Add(eventDeclaration);
            return CompilerResult.Success;
        }
    }
}
