using System;
using System.Collections.Generic;
using System.Linq;
using IronMeta.Matcher;
using Lizpy.Syntax;

namespace Lizpy.Internal {
    public static class MatchItemExtensions {
        public static String InputString(this MatchItem<Char, SExpression> item) {
            return new String(item.Inputs.ToArray());
        }

        public static IEnumerable<TExpression> ResultsAs<TExpression>(
            this MatchItem<Char, SExpression> item)
            where TExpression : SExpression {

            return item.Results.Cast<TExpression>();
        }

        public static TExpression ResultAs<TExpression>(
            this MatchItem<Char, SExpression> item)
            where TExpression : SExpression {

            return (TExpression)item.Results.Single();
        }
    }
}
