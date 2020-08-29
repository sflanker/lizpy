using System.Collections.Generic;
using Lizpy.Syntax;
using ModApi.Craft.Program;
using ExpressionType = Lizpy.Syntax.ExpressionType;

namespace Lizpy.Compiler {
    public interface IExpressionHandler {
        IEnumerable<ExpressionType> SupportedTypes { get; }

        ProgramExpression CompileExpression(CompilerState state, SExpression expression);
    }
}
