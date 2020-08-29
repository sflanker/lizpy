using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lizpy.Syntax {
    [Serializable]
    public class StringExpression : AtomExpression<String> {
        public override ExpressionType Type => ExpressionType.String;

        public StringExpression(Int32 startIndex, Int32 nextIndex, String value) :
            base(startIndex, nextIndex, value) {
        }

        public StringExpression(
            Int32 startIndex,
            Int32 nextIndex,
            IEnumerable<CharacterExpression> characters) :
            base(startIndex, nextIndex, MakeString(characters)) {
        }

        private static string MakeString(IEnumerable<CharacterExpression> characters) {
            return new String(characters.Select(c => c.Value).ToArray());
        }

        public override string ToString() {
            return $"\"{Escape(this.Value)}\"";
        }

        private static readonly IReadOnlyDictionary<Char, String> EscapeSequences =
            new Dictionary<char, string> {
                { '\n', "\\n" },
                { '\r', "\\r" },
                { '\t', "\\t" },
                { '\0', "\\0" },
                { '\\', "\\\\" },
                { '"', "\\\"" },
            };

        public static String Escape(String value) {
            var result = new StringBuilder(value.Length);

            var start = 0;
            for (var current = 0; current < value.Length; current++) {
                if (EscapeSequences.TryGetValue(value[current], out var escape)) {
                    if (start < current) {
                        result.Append(value.Substring(start, current - start));
                    }

                    result.Append(escape);
                    start = current + 1;
                }
            }

            if (start < value.Length) {
                result.Append(value.Substring(start));
            }

            return result.ToString();
        }
    }
}
