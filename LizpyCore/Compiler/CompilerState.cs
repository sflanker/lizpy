using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Lizpy.Internal;
using Lizpy.Syntax;
using ModApi.Craft.Program;
using ModApi.Craft.Program.Instructions;

namespace Lizpy.Compiler {
    public class CompilerState {
        private readonly IDictionary<SymbolExpression, String> expressionNameLookup;
        private readonly IDictionary<SymbolExpression, String> instructionNameLookup;
        private readonly Stack<SortedSet<String>> scopes;
        private readonly SortedSet<String> combinedScope;
        private readonly Stack<String> namespaceStack;
        private readonly List<CompilerResult> errors;

        public LizpyCompiler Compiler { get; }
        /// <summary>
        /// Mapping of custom expression identifiers used in Lizpy to expression names used in Vizzy.
        /// </summary>
        public IReadOnlyDictionary<SymbolExpression, String> ExpressionNameLookup { get; }

        /// <summary>
        /// Mapping of custom instruction identifiers used in Lizpy to instruction names used in Vizzy.
        /// </summary>
        public IReadOnlyDictionary<SymbolExpression, String> InstructionNameLookup { get; }
        public FlightProgram Program { get; }
        public String Namespace => this.namespaceStack.Peek();
        public IReadOnlyList<CompilerResult> Errors => this.errors;

        public CompilerState(
            LizpyCompiler compiler,
            FlightProgram program) {

            this.Compiler = compiler;
            this.Program = program;

            this.scopes = new Stack<SortedSet<String>>();
            this.combinedScope = new SortedSet<String>();
            this.namespaceStack = new Stack<String>();
            this.expressionNameLookup = new Dictionary<SymbolExpression, String>();
            this.instructionNameLookup = new Dictionary<SymbolExpression, String>();
            this.ExpressionNameLookup =
                new ReadOnlyDictionary<SymbolExpression, String>(this.expressionNameLookup);
            this.InstructionNameLookup =
                new ReadOnlyDictionary<SymbolExpression, String>(this.instructionNameLookup);
            this.errors = new List<CompilerResult>();
        }

        public void PushNamespace(String @namespace) {
            this.namespaceStack.Push(@namespace);
        }

        public void PopNamespace() {
            this.namespaceStack.Pop();
        }

        public void AddExpression(SymbolExpression identifierSymbol, String vizzyName) {
            this.expressionNameLookup.Add(identifierSymbol, vizzyName);
        }

        public void AddInstruction(
            SymbolExpression identifierSymbol,
            String vizzyName) {
            this.instructionNameLookup.Add(identifierSymbol, vizzyName);
        }

        public void PushScope(ICollection<SymbolExpression> variables) {
            var variableSet =
                new SortedSet<String>(variables.Select(v => v.Value));
            if (this.combinedScope.Overlaps(variableSet)) {
                var conflicts = new SortedSet<String>(variableSet);
                conflicts.IntersectWith(this.combinedScope);
                Console.Error.WriteLine(
                    $"The specified variable '{conflicts.First()}' hides a variable with the same name in an enclosing scope."
                );
                // throw new LizpyInternalCompilationException(
                //     $"The specified variable name conflicts with a variable in a parent scope.",
                //     variables.First(v => variableSet.Contains(v.Value))
                // );
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

        public ProgramInstruction[] CompileInstruction(SExpression sexpression) {
            return this.Compiler.CompileInstruction(this, sexpression);
        }

        public void AddError(CompilerResult errorResult) {
            if (errorResult.IsSuccess) {
                throw new ArgumentException($"The specified {nameof(CompilerResult)} is not an error.");
            }

            this.errors.Add(errorResult);
        }
    }
}
