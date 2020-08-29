using System;
using System.Collections.Generic;
using Lizpy.Compiler;
using Lizpy.Syntax;
using ModApi.Craft.Program;

namespace Lizpy.Builtin {
    public class InstructionDeclarationHandler : DeclarationHandlerBase {
        private static readonly String[] ExpressionDeclarationSymbols = { "definst" };

        public override IEnumerable<String> SupportedSymbols => ExpressionDeclarationSymbols;

        public override CompilerResult ApplyDeclaration(
            CompilerState state,
            ListExpression declaration) {

            // expected form (definst symbol [args] instruction1 instruction2 ... instructionN)
            throw new System.NotImplementedException();
        }
    }
}
