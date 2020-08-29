using ModApi.Craft.Program;

namespace Lizpy.Compiler {
    public interface ICompilerPass {
        void ApplyPass(FlightProgram program);
    }
}
