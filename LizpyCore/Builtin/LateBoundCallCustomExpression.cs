using System;
using Lizpy.Syntax;
using ModApi.Craft.Program.Expressions;

namespace Lizpy.Builtin {
    public class LateBoundCallCustomExpression : CallCustomExpression {
        /// <summary>
        /// The raw symbol in an custom expression reference.
        /// </summary>
        public SymbolExpression Reference { get; set; }

        /// <summary>
        /// The namespace where the custom expression reference was defined.
        /// </summary>
        /// <remarks>
        /// This is necessary for symbol resolution which defaults to the current namespace followed by the default namespace with no namespace is explicitly specified.
        /// </remarks>
        public String SourceNamespace { get; set; }
    }
}
