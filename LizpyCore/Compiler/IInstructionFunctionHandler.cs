using System;
using System.Collections.Generic;
using Lizpy.Syntax;
using ModApi.Craft.Program.Instructions;

namespace Lizpy.Compiler {
    public interface IInstructionFunctionHandler {
        String Namespace { get; }
        IEnumerable<String> SupportedSymbols { get; }

        ProgramInstruction[] CompileFunction(
            CompilerState state,
            ListExpression expression);
    }
}
