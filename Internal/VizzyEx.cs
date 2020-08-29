using ModApi.Craft.Program;

namespace Lizpy.Internal {
    public static class VizzyEx {
        public static ProgramExpression StringEqual(ProgramExpression value1, ProgramExpression value2) {
            return Vizzy.And(
                Vizzy.Contains(value1, value2),
                Vizzy.Equal(Vizzy.StringLength(value1), Vizzy.StringLength(value2))
            );
        }
    }
}
