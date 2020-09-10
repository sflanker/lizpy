using System;
using System.Text.RegularExpressions;
using Lizpy.Syntax;

namespace Lizpy.Internal {
    public static class SExpressionExtensions {
        private static readonly Regex VariableNameRegex = new Regex("^[A-Za-z_~][0-9A-Za-z_~]*$");
        private static readonly Regex NamespaceRegex = new Regex("^[A-Za-z~][0-9A-Za-z~]*$");

        private static readonly Regex NamespacedVariableNameRegex =
            new Regex("^(?'namespace'[A-Za-z~][0-9A-Za-z~]*/)?((?'name'[A-Za-z_~][0-9A-Za-z_~]*))$");

        public static Boolean TryGetMetadata<TExpression>(
            this SymbolExpression symbol,
            SymbolExpression key,
            out TExpression data)
            where TExpression : SExpression {

            data = null;
            SExpression value = null;
            if (symbol.Metadata?.TryGetValue(key, out value) == true &&
                value is TExpression typedValue) {
                data = typedValue;
                return true;
            } else {
                data = null;
                return false;
            }
        }

        public static String ToVizzyNamespacedName(this SymbolExpression symbol) {
            var parts = symbol.Value.Split(new[] { '/' }, 2);
            return String.Join("__", parts);
        }

        public static Boolean IsValidUnNamespacedVariable(this SymbolExpression symbol) {
            return VariableNameRegex.IsMatch(symbol.Value);
        }

        public static Boolean IsValidVariable(this SymbolExpression symbol, out Match match) {
            match = NamespacedVariableNameRegex.Match(symbol.Value);
            return match.Success;
        }

        public static Boolean IsValidNamespacedVariable(this SymbolExpression symbol) {
            return symbol.IsValidVariable(out var match) && match.Groups["namespace"].Success;
        }

        public static Boolean IsValidNamespaceString(this StringExpression @namespace) {
            return NamespaceRegex.IsMatch(@namespace.Value);
        }

        public static String GetLocalName(this SymbolExpression symbol) {
            if (symbol.Value.StartsWith("/")) {
                return symbol.Value;
            }

            var ix = symbol.Value.IndexOf('/');
            return ix < 0 ? symbol.Value : symbol.Value.Substring(ix + 1);
        }
    }
}
