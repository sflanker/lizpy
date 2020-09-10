using System;
using System.Collections.Generic;
using Lizpy.Compiler;
using Lizpy.Syntax;
using ModApi.Craft.Program.Instructions;

namespace Lizpy.Builtin.Instructions {
    public class BroadcastInstructionHandler : IInstructionFunctionHandler {
        private static readonly String[] BroadcastInstructionSymbols = {
            "broadcast!",
            "broadcast-to-craft!"
        };

        public String Namespace => String.Empty;
        public IEnumerable<String> SupportedSymbols => BroadcastInstructionSymbols;

        public ProgramInstruction[] CompileFunction(
            CompilerState state,
            ListExpression expression) {
            throw new System.NotImplementedException();
        }
    }
}
