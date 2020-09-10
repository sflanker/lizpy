using System;
using System.Collections.Generic;
using System.Linq;
using Lizpy.Compiler;
using Lizpy.Internal;
using Lizpy.Syntax;
using ModApi.Craft.Program;
using ExpressionType = Lizpy.Syntax.ExpressionType;

namespace Lizpy.Builtin {
    public class ArrayExpressionHandler : IExpressionHandler {
        private static readonly ExpressionType[] supportedExpressionTypes = {
            ExpressionType.Array
        };

        public IEnumerable<ExpressionType> SupportedTypes => supportedExpressionTypes;

        public ProgramExpression CompileExpression(CompilerState state, SExpression expression) {
            if (expression is ArrayExpression array) {
                // This sucks, but until we get proper list construction, our hands are tied
                return Vizzy.List.Create(
                    String.Join(
                        ",",
                        array.Items.Select(i => i.GetValue().ToString().Replace(",", "□")))
                );
            } else {
                throw new NotSupportedException($"Unsupported expression type: {expression.Type}");
            }
        }
    }
}
