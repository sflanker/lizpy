using System;

namespace Lizpy.Syntax {
    public enum ExpressionType {
        Symbol = 0x001,
        String = 0x002,
        Character = 0x004,
        Integer = 0x008,
        Decimal = 0x010,
        Boolean = 0x020,
        List = 0x040,
        Array = 0x080,
        Map = 0x100,

        Number = Integer | Decimal,
        Atom = Symbol | String | Character | Number | Boolean,
    }
}
