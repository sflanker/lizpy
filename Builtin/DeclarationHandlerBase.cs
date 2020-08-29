using System;
using System.Collections.Generic;
using Lizpy.Compiler;
using Lizpy.Syntax;
using ModApi.Craft.Program;

namespace Lizpy.Builtin {
    public abstract class DeclarationHandlerBase : IDeclarationHandler {
        protected static readonly SymbolExpression NameKey = new SymbolExpression(":name");
        protected static readonly SymbolExpression FormatKey = new SymbolExpression(":format");
        protected static readonly SymbolExpression PositionKey = new SymbolExpression(":position");

        public abstract IEnumerable<String> SupportedSymbols { get; }

        public abstract CompilerResult ApplyDeclaration(
            CompilerState state,
            ListExpression declaration);
    }
}
