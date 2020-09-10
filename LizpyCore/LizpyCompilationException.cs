using System;
using System.Runtime.Serialization;
using Lizpy.Internal;
using Lizpy.Syntax;

namespace Lizpy {
    [Serializable]
    public class LizpyCompilationException : LizpyInternalCompilationException {
        public String FileName { get; }
        public Int32 Line { get; }
        public Int32 Column { get; }

        public LizpyCompilationException(
            String message,
            SExpression expression,
            String fileName,
            Int32 line,
            Int32 column) : base(message, expression) {

            this.FileName = fileName;
            this.Line = line;
            this.Column = column;
        }

        public LizpyCompilationException(
            String message,
            SExpression expression,
            Exception innerException,
            String fileName,
            Int32 line,
            Int32 column) : base(message, expression, innerException) {

            this.FileName = fileName;
            this.Line = line;
            this.Column = column;
        }

        protected LizpyCompilationException(SerializationInfo info, StreamingContext context) :
            base(info, context) {

            this.FileName = info.GetString("fileName");
            this.Line = info.GetInt32("line");
            this.Column = info.GetInt32("column");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            base.GetObjectData(info, context);

            info.AddValue("fileName", this.FileName);
            info.AddValue("line", this.Line);
            info.AddValue("column", this.Column);
        }
    }
}
