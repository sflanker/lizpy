using System;
using System.Collections.Generic;
using Lizpy.Syntax;
using ModApi.Craft.Program;

namespace Lizpy.Compiler {
    public interface IFunctionHandler {
        String Namespace { get; }
        IEnumerable<String> SupportedSymbols { get; }

        ProgramNode CompileFunction(CompilerState state, ListExpression expression);
    }
}
