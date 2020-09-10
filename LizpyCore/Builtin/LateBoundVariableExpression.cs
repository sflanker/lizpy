using System;
using Lizpy.Syntax;
using ModApi.Craft.Program;

namespace Lizpy.Builtin {
    public class LateBoundVariableExpression : VariableExpression {
        public SymbolExpression Reference { get; set; }
        public String SourceNamespace { get; set; }
    }
}
