using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Lizpy.Syntax {
    [Serializable]
    public class MapExpression : SExpression {
        public IReadOnlyDictionary<AtomExpression, SExpression> Data { get; }

        public override ExpressionType Type => ExpressionType.Map;

        public MapExpression(
            Int32 startIndex,
            Int32 nextIndex,
            IEnumerable<ListExpression> keyValuePairs) :
            base(startIndex, nextIndex) {

            if (keyValuePairs == null) {
                throw new ArgumentNullException(nameof(keyValuePairs));
            }

            this.Data = new ReadOnlyDictionary<AtomExpression, SExpression>(
                keyValuePairs.ToDictionary(
                    kvp => ValidateAndGetKey(kvp.Items),
                    kvp => kvp.Items[1])
            );
        }

        public MapExpression(IReadOnlyDictionary<AtomExpression, SExpression> data) : base(0, 0) {
            this.Data = data;
        }

        public override string ToString() {
            return $"{{ {String.Join(", ", this.Data.Select(kvp => $"{kvp.Key} {kvp.Value}"))} }}";
        }

        private static AtomExpression ValidateAndGetKey(IReadOnlyList<SExpression> keyValuePair) {
            if (keyValuePair == null) {
                throw new ArgumentNullException(nameof(keyValuePair));
            }

            if (keyValuePair.Count != 2) {
                throw new ArgumentException(
                    "Invalid Key Value Pair list. Key Value Pairs must have a length of 2.",
                    nameof(keyValuePair)
                );
            }

            if (keyValuePair[0] is AtomExpression key) {
                return key;
            } else {
                throw new ArgumentException(
                    "They first item of a Key Value Pair list must be an AtomExpression.",
                    nameof(keyValuePair)
                );
            }
        }
    }
}
