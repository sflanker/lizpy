using System;
using System.Collections.Generic;
using Lizpy.Syntax;
using ModApi.Craft.Program;

namespace Lizpy.Compiler {
    public interface IExpressionFunctionHandler {
        String Namespace { get; }
        IEnumerable<String> SupportedSymbols { get; }

        ProgramExpression CompileFunction(
            CompilerState state,
            ListExpression expression);
    }
}
