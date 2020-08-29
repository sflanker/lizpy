using System;
using System.Collections.Generic;

namespace Lizpy.Syntax {
    [Serializable]
    public class CharacterExpression : AtomExpression<Char> {
        public override ExpressionType Type => ExpressionType.Character;

        public CharacterExpression(Char value) : base(0, 0, value) {
        }

        public override string ToString() {
            return $"'{Escape(this.Value)}'";
        }

        private static readonly IReadOnlyDictionary<Char, String> EscapeSequences =
            new Dictionary<Char, String> {
                { '\n', "\\n" },
                { '\r', "\\r" },
                { '\t', "\\t" },
                { '\0', "\\0" },
                { '\\', "\\\\" },
                { '\'', "\\'" },
            };

        public static String Escape(Char character) {
            return EscapeSequences.TryGetValue(character, out var escape) ?
                escape :
                character.ToString();
        }
    }
}
