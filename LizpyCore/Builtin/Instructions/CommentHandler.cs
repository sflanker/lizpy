using System;
using System.Collections.Generic;
using Lizpy.Compiler;
using Lizpy.Internal;
using Lizpy.Syntax;
using ModApi.Craft.Program.Expressions;
using ModApi.Craft.Program.Instructions;

namespace Lizpy.Builtin.Instructions {
    public class CommentHandler  : IInstructionFunctionHandler {
        private static readonly String[] CommentInstructionSymbols = { "comment" };
        public String Namespace => String.Empty;

        public IEnumerable<String> SupportedSymbols => CommentInstructionSymbols;

        public ProgramInstruction[] CompileFunction(
            CompilerState state,
            ListExpression expression) {

            if (expression.Items.Count != 2) {
                throw new LizpyInternalCompilationException(
                    "Invalid Comment.",
                    expression
                );
            }
            if (!(expression.Items[1] is StringExpression message)) {
                throw new LizpyInternalCompilationException(
                    "Invalid Comment, value must be a string constant.",
                    expression
                );
            }

            var commentInstruction = new LogMessageInstruction {
                Style = "comment"
            };

            commentInstruction.InitializeExpressions(
                new ConstantExpression(message.Value) {
                    Style = "comment-text",
                    CanReplaceInUI = false
                }
            );

            return new ProgramInstruction[] { commentInstruction };
        }
    }
}
