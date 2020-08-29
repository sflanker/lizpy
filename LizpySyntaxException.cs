using System;
using System.Runtime.Serialization;

namespace Lizpy {
    [Serializable]
    public class LizpySyntaxException : Exception {
        public Int32 Line { get; }
        public Int32 Column { get; }

        public LizpySyntaxException(String message, Int32 line, Int32 column) : base(message) {
            Line = line;
            Column = column;
        }

        protected LizpySyntaxException(SerializationInfo info, StreamingContext context) :
            base(info, context) {

            this.Line = info.GetInt32("line");
            this.Column = info.GetInt32("column");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            base.GetObjectData(info, context);

            info.AddValue("line", this.Line);
            info.AddValue("column", this.Column);
        }
    }
}
