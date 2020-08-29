using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Lizpy.Internal;
using Lizpy.Syntax;
using ModApi.Craft.Program;

namespace Lizpy.Compiler {
    public class CompilerState {
        private readonly IDictionary<SymbolExpression, String> expressionNameLookup;
        private readonly Stack<SortedSet<String>> scopes;
        private readonly SortedSet<String> combinedScope;
        private readonly Stack<String> namespaceStack;

        public LizpyCompiler Compiler { get; }
        public IReadOnlyDictionary<SymbolExpression, String> ExpressionNameLookup { get; }
        public FlightProgram Program { get; }
        public String Namespace => this.namespaceStack.Peek();

        public CompilerState(
            LizpyCompiler compiler,
            FlightProgram program) {

            this.Compiler = compiler;
            this.Program = program;

            this.scopes = new Stack<SortedSet<String>>();
            this.combinedScope = new SortedSet<String>();
            this.namespaceStack = new Stack<String>();
            this.expressionNameLookup = new Dictionary<SymbolExpression, String>();
            this.ExpressionNameLookup =
                new ReadOnlyDictionary<SymbolExpression, String>(this.expressionNameLookup);
        }

        public void PushNamespace(String @namespace) {
            this.namespaceStack.Push(@namespace);
        }

        public void PopNamespace() {
            this.namespaceStack.Pop();
        }

        public void AddExpression(SymbolExpression qualifiedSymbol, String vizzyName) {
            this.expressionNameLookup.Add(qualifiedSymbol, vizzyName);
        }

        public void PushScope(ICollection<SymbolExpression> variables) {
            var variableSet =
                new SortedSet<String>(variables.Select(v => v.Value));
            if (this.combinedScope.Overlaps(variableSet)) {
                variableSet.IntersectWith(this.combinedScope);
                throw new LizpyInternalCompilationException(
                    $"The specified variable name conflicts with a variable in a parent scope.",
                    variables.First(v => variableSet.Contains(v.Value))
                );
            }

            this.scopes.Push(variableSet);
            this.combinedScope.UnionWith(variableSet);
        }

        public void PopScope() {
            var scope = this.scopes.Pop();
            this.combinedScope.ExceptWith(scope);
        }

        public Boolean IsLocalVariable(String variableName) {
            return this.combinedScope.Contains(variableName);
        }

        public ProgramExpression CompileExpression(SExpression sexpression) {
            return this.Compiler.CompileExpression(this, sexpression);
        }
    }
}
