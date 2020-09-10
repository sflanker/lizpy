using System;
using System.IO;
using Lizpy;
using ModApi.Craft.Program;

namespace LizpyCommandLine {
    internal class Program {
        public static Int32 Main(String[] args) {
            if (args.Length == 0) {
                Console.Error.WriteLine("Missing required argument.");
                Console.Error.WriteLine("Usage: clizpy Filename");
                return 1;
            } else if (args.Length > 1) {
                Console.Error.WriteLine("Unexpected arguments.");
                Console.Error.WriteLine("Usage: clizpy Filename");
                return 1;
            } else if (args[0] == "-h" || args[0] == "--help") {
                Console.Error.WriteLine("Lizpy flight program compiler for SimpleRockets2.");
                Console.Error.WriteLine();
                Console.Error.WriteLine("Usage: clizpy Filename");
                return 0;
            }

            if (!File.Exists(args[0])) {
                Console.Error.WriteLine($"File not found: {args[0]}");
                return 2;
            }

            var compiler = LizpyCompiler.DefaultInstance;

            try {
                using (var reader = File.OpenText(args[0])) {
                    var program = compiler.Compile(args[0], reader);

                    var serializer = new ProgramSerializer();
                    var xml = serializer.SerializeFlightProgram(program);
                    Console.WriteLine(xml.ToString());

                    return 0;
                }
            } catch (LizpySyntaxException syntaxError) {
                Console.Error.WriteLine($"Syntax error: {syntaxError.Message} at {syntaxError.FileName} (line {syntaxError.Line}, column {syntaxError.Column})");
                return 3;
            } catch (LizpyCompilationException compilationError) {
                Console.Error.WriteLine($"Compilation error: {compilationError.Message} at '{compilationError.FileName}' (line {compilationError.Line}, column {compilationError.Column})");
                return 4;
            }
        }
    }
}
