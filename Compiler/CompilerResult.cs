using System;
using Lizpy.Syntax;

namespace Lizpy.Compiler {
    public class CompilerResult {
        public static readonly CompilerResult Success = new CompilerResult();

        public Boolean IsSuccess { get; }

        public SExpression Expression { get; }

        public String Error { get; }

        protected CompilerResult() {
            this.IsSuccess = true;
        }

        public CompilerResult(SExpression expression, String error) {
            this.IsSuccess = false;
            this.Expression = expression;
            this.Error = error;
        }
    }
}
