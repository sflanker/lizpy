using System;
using ModApi.Craft.Program;
using ModApi.Craft.Program.Expressions;

namespace Lizpy.Internal {
    public static class Vizzy {
        public static ConstantExpression Number(Int32 value) {
            return new ConstantExpression(value);
        }

        public static ConstantExpression Number(Double value) {
            return new ConstantExpression(value);
        }

        public static BinaryOperatorExpression Add(ProgramExpression value1, ProgramExpression value2) {
            var op = new BinaryOperatorExpression {
                Operator = "+",
                Style = "op-add"
            };

            op.InitializeExpressions(value1, value2);

            return op;
        }

        public static BinaryOperatorExpression Subtract(ProgramExpression value1, ProgramExpression value2) {
            var op = new BinaryOperatorExpression {
                Operator = "-",
                Style = "op-sub"
            };

            op.InitializeExpressions(value1, value2);

            return op;
        }

        public static BinaryOperatorExpression Multiply(ProgramExpression value1, ProgramExpression value2) {
            var op = new BinaryOperatorExpression {
                Operator = "*",
                Style = "op-mul"
            };

            op.InitializeExpressions(value1, value2);

            return op;
        }

        public static BinaryOperatorExpression Divide(ProgramExpression value1, ProgramExpression value2) {
            var op = new BinaryOperatorExpression {
                Operator = "/",
                Style = "op-div"
            };

            op.InitializeExpressions(value1, value2);

            return op;
        }

        public static BinaryOperatorExpression Pow(ProgramExpression value1, ProgramExpression value2) {
            var op = new BinaryOperatorExpression {
                Operator = "^",
                Style = "op-exp"
            };

            op.InitializeExpressions(value1, value2);

            return op;
        }

        public static MathFunctionExpression Ln(ProgramExpression value) {
            var op = new MathFunctionExpression {
                FunctionName = "ln",
                Style = "op-math"
            };

            op.InitializeExpressions(value);

            return op;
        }

        public static ProgramExpression Log(ProgramExpression value) {
            var op = new MathFunctionExpression {
                FunctionName = "log",
                Style = "op-math"
            };

            op.InitializeExpressions(value);

            return op;
        }

        public static ProgramExpression Sqrt(ProgramExpression value) {
            var op = new MathFunctionExpression {
                FunctionName = "sqrt",
                Style = "op-math"
            };

            op.InitializeExpressions(value);

            return op;
        }

        public static ProgramExpression Equal(ProgramExpression value1, ProgramExpression value2) {
            var comp = new ComparisonExpression {
                Operator = "=",
                Style = "op-eq"
            };

            comp.InitializeExpressions(value1, value2);

            return comp;
        }

        public static ProgramExpression And(ProgramExpression condition1, ProgramExpression condition2) {
            var op = new BoolOperatorExpression {
                Operator = "and",
                Style = "op-and"
            };

            op.InitializeExpressions(condition1, condition2);

            return op;
        }

        public static StringOperatorExpression Contains(ProgramExpression value1, ProgramExpression value2) {
            var op = new StringOperatorExpression {
                Operator = "contains",
                Style = "contains"
            };

            op.InitializeExpressions(value1, value2);

            return op;
        }

        public static StringOperatorExpression StringLength(ProgramExpression value) {
            var op = new StringOperatorExpression {
                Operator = "length",
                Style = "length"
            };

            op.InitializeExpressions(value);

            return op;
        }

        /// <summary>
        /// Get an individual letter from a string.
        /// </summary>
        /// <remarks>
        /// Note: the arguments to this function are in the reverse order from that of the corresponding Vizzy expression. This is to maintain consistency with the Common Lisp function char.
        /// </remarks>
        public static StringOperatorExpression GetLetter(
            ProgramExpression value,
            ProgramExpression index) {

            var op = new StringOperatorExpression {
                Operator = "letter",
                Style = "letter"
            };

            op.InitializeExpressions(
                index,
                value
            );

            return op;
        }

        /// <summary>
        /// Get an individual letter from a string.
        /// </summary>
        /// <remarks>
        /// Note: the arguments to this function are in a different order from that of the corresponding Vizzy expression. This is to maintain consistency with the Common Lisp function char.
        /// </remarks>
        public static StringOperatorExpression SubString(
            ProgramExpression str,
            ProgramExpression start,
            ProgramExpression end) {

            var op = new StringOperatorExpression {
                Operator = "substring",
                Style = "substring"
            };

            op.InitializeExpressions(
                start,
                end,
                str
            );

            return op;
        }

        public static StringOperatorExpression Join(params ProgramExpression[] parts) {
            var op = new StringOperatorExpression {
                Operator = "join",
                Style = "join"
            };

            op.InitializeExpressions(parts);

            return op;
        }

        public static StringOperatorExpression Format(params ProgramExpression[] parts) {
            var op = new StringOperatorExpression {
                Operator = "format",
                Style = "format"
            };

            op.InitializeExpressions(parts);

            return op;
        }

        public static VectorExpression Vector(
            ProgramExpression x,
            ProgramExpression y,
            ProgramExpression z) {

            var op = new VectorExpression {
                Style = "vec"
            };

            op.InitializeExpressions(x, y, z);

            return op;
        }

        public static class List {
            public static ListOperatorExpression Create(String str) {
                var op = new ListOperatorExpression {
                    Operator = "create",
                    Style = "list-create"
                };

                op.InitializeExpressions(new ConstantExpression(str));

                return op;
            }
        }
    }
}
