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

        private static readonly Char[] Whitespace = { ' ', '\t', '\n', '\r' };

        private static string MakeString(IEnumerable<CharacterExpression> characters) {
            var builder = new StringBuilder();

            var skipWhitespace = false;
            foreach (var character in characters) {
                if (skipWhitespace && Whitespace.Contains(character.Value)) {
                    continue;
                } else if (character.Value == '\n' && !character.Escaped) {
                    // In a multiline string declaration anytime a unescaped newline is encountered,
                    // it and any following whitespace is ignored. This allows for strings to be
                    // broken over multiple lines without disrupting indentation. For example:
                    // (comment
                    //     "This is an example
                    //     of a multi-line string constant")
                    //
                    // Would generate a comment with the text "This is an example of a multi-line string constant"
                    // If an actual newline is desired it must be escaped:
                    // (comment "This is an example\n of a string with an actual newline")
                    skipWhitespace = true;
                    continue;
                }

                skipWhitespace = false;
                builder.Append(character.Value);
            }

            return builder.ToString();
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
