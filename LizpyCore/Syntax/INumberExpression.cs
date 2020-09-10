using System;

namespace Lizpy.Syntax {
    public interface INumberExpression {
        Double AsDouble();

        Int32 AsInteger();
    }
}
