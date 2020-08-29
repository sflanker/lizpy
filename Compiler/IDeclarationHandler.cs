using System;
using System.Collections.Generic;
using Lizpy.Syntax;

namespace Lizpy.Compiler {
    public interface IDeclarationHandler {
        CompilerResult ApplyDeclaration(
            CompilerState state,
            ListExpression declaration);

        IEnumerable<String> SupportedSymbols { get; }
    }
}
