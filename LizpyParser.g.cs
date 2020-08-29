//
// IronMeta LizpyParser Parser; Generated 2020-08-27 03:07:03Z UTC
//

using System;
using System.Collections.Generic;
using System.Linq;

using IronMeta.Matcher;
using Lizpy.Syntax;
using Lizpy.Internal;

#pragma warning disable 0219
#pragma warning disable 1591

namespace Lizpy
{

    using _LizpyParser_Inputs = IEnumerable<char>;
    using _LizpyParser_Results = IEnumerable<SExpression>;
    using _LizpyParser_Item = IronMeta.Matcher.MatchItem<char, SExpression>;
    using _LizpyParser_Args = IEnumerable<IronMeta.Matcher.MatchItem<char, SExpression>>;
    using _LizpyParser_Memo = IronMeta.Matcher.MatchState<char, SExpression>;
    using _LizpyParser_Rule = System.Action<IronMeta.Matcher.MatchState<char, SExpression>, int, IEnumerable<IronMeta.Matcher.MatchItem<char, SExpression>>>;
    using _LizpyParser_Base = IronMeta.Matcher.Matcher<char, SExpression>;

    public partial class LizpyParser : Matcher<char, SExpression>
    {
        public LizpyParser()
            : base()
        {
            _setTerminals();
        }

        public LizpyParser(bool handle_left_recursion)
            : base(handle_left_recursion)
        {
            _setTerminals();
        }

        void _setTerminals()
        {
            this.Terminals = new HashSet<string>()
            {
                "Boolean",
                "Comment",
                "Decimal",
                "Digit",
                "EscapeSequence",
                "Integer",
                "Number",
                "Scientific",
                "Sign",
                "String",
                "StringCharacterExpression",
                "TopLevelExpression",
                "Whitespace",
            };
        }


        public void TopLevelExpression(_LizpyParser_Memo _memo, int _index, _LizpyParser_Args _args)
        {

            int _arg_index = 0;
            int _arg_input_index = 0;

            // CALL Trim
            var _start_i0 = _index;
            _LizpyParser_Item _r0;

            _LizpyParser_Args _actual_args0 = new _LizpyParser_Item[] { new _LizpyParser_Item(SExpression) };
            if (_args != null) _actual_args0 = _actual_args0.Concat(_args.Skip(_arg_index));
            _r0 = _MemoCall(_memo, "Trim", _index, Trim, _actual_args0);

            if (_r0 != null) _index = _r0.NextIndex;

        }


        public void SExpression(_LizpyParser_Memo _memo, int _index, _LizpyParser_Args _args)
        {

            int _arg_index = 0;
            int _arg_input_index = 0;

            // OR 0
            int _start_i0 = _index;

            // OR 1
            int _start_i1 = _index;

            // OR 2
            int _start_i2 = _index;

            // CALLORVAR Atom
            _LizpyParser_Item _r3;

            _r3 = _MemoCall(_memo, "Atom", _index, Atom, null);

            if (_r3 != null) _index = _r3.NextIndex;

            // OR shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Pop(); _index = _start_i2; } else goto label2;

            // CALLORVAR List
            _LizpyParser_Item _r4;

            _r4 = _MemoCall(_memo, "List", _index, List, null);

            if (_r4 != null) _index = _r4.NextIndex;

        label2: // OR
            int _dummy_i2 = _index; // no-op for label

            // OR shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Pop(); _index = _start_i1; } else goto label1;

            // CALLORVAR Map
            _LizpyParser_Item _r5;

            _r5 = _MemoCall(_memo, "Map", _index, Map, null);

            if (_r5 != null) _index = _r5.NextIndex;

        label1: // OR
            int _dummy_i1 = _index; // no-op for label

            // OR shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Pop(); _index = _start_i0; } else goto label0;

            // CALLORVAR Array
            _LizpyParser_Item _r6;

            _r6 = _MemoCall(_memo, "Array", _index, Array, null);

            if (_r6 != null) _index = _r6.NextIndex;

        label0: // OR
            int _dummy_i0 = _index; // no-op for label

        }


        public void Trim(_LizpyParser_Memo _memo, int _index, _LizpyParser_Args _args)
        {

            int _arg_index = 0;
            int _arg_input_index = 0;

            _LizpyParser_Item pattern = null;
            _LizpyParser_Item p = null;

            // ARGS 0
            _arg_index = 0;
            _arg_input_index = 0;

            // ANY
            _ParseAnyArgs(_memo, ref _arg_index, ref _arg_input_index, _args);

            // BIND pattern
            pattern = _memo.ArgResults.Peek();

            if (_memo.ArgResults.Pop() == null)
            {
                _memo.Results.Push(null);
                goto label0;
            }

            // AND 4
            int _start_i4 = _index;

            // AND 5
            int _start_i5 = _index;

            // STAR 6
            int _start_i6 = _index;
            var _res6 = Enumerable.Empty<SExpression>();
        label6:

            // CALLORVAR Whitespace
            _LizpyParser_Item _r7;

            _r7 = _MemoCall(_memo, "Whitespace", _index, Whitespace, null);

            if (_r7 != null) _index = _r7.NextIndex;

            // STAR 6
            var _r6 = _memo.Results.Pop();
            if (_r6 != null)
            {
                _res6 = _res6.Concat(_r6.Results);
                goto label6;
            }
            else
            {
                _memo.Results.Push(new _LizpyParser_Item(_start_i6, _index, _memo.InputEnumerable, _res6.Where(_NON_NULL), true));
            }

            // AND shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Push(null); goto label5; }

            // CALLORVAR pattern
            _LizpyParser_Item _r9;

            if (pattern.Production != null)
            {
                var _p9 = (System.Action<_LizpyParser_Memo, int, IEnumerable<_LizpyParser_Item>>)(object)pattern.Production;
                _r9 = _MemoCall(_memo, pattern.Production.Method.Name, _index, _p9, _args != null ? _args.Skip(_arg_index) : null);
            }
            else
            {
                _r9 = _ParseLiteralObj(_memo, ref _index, pattern.Inputs);
            }

            if (_r9 != null) _index = _r9.NextIndex;

            // BIND p
            p = _memo.Results.Peek();

        label5: // AND
            var _r5_2 = _memo.Results.Pop();
            var _r5_1 = _memo.Results.Pop();

            if (_r5_1 != null && _r5_2 != null)
            {
                _memo.Results.Push( new _LizpyParser_Item(_start_i5, _index, _memo.InputEnumerable, _r5_1.Results.Concat(_r5_2.Results).Where(_NON_NULL), true) );
            }
            else
            {
                _memo.Results.Push(null);
                _index = _start_i5;
            }

            // AND shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Push(null); goto label4; }

            // STAR 10
            int _start_i10 = _index;
            var _res10 = Enumerable.Empty<SExpression>();
        label10:

            // CALLORVAR Whitespace
            _LizpyParser_Item _r11;

            _r11 = _MemoCall(_memo, "Whitespace", _index, Whitespace, null);

            if (_r11 != null) _index = _r11.NextIndex;

            // STAR 10
            var _r10 = _memo.Results.Pop();
            if (_r10 != null)
            {
                _res10 = _res10.Concat(_r10.Results);
                goto label10;
            }
            else
            {
                _memo.Results.Push(new _LizpyParser_Item(_start_i10, _index, _memo.InputEnumerable, _res10.Where(_NON_NULL), true));
            }

        label4: // AND
            var _r4_2 = _memo.Results.Pop();
            var _r4_1 = _memo.Results.Pop();

            if (_r4_1 != null && _r4_2 != null)
            {
                _memo.Results.Push( new _LizpyParser_Item(_start_i4, _index, _memo.InputEnumerable, _r4_1.Results.Concat(_r4_2.Results).Where(_NON_NULL), true) );
            }
            else
            {
                _memo.Results.Push(null);
                _index = _start_i4;
            }

            // ACT
            var _r3 = _memo.Results.Peek();
            if (_r3 != null)
            {
                _memo.Results.Pop();
                _memo.Results.Push( new _LizpyParser_Item(_r3.StartIndex, _r3.NextIndex, _memo.InputEnumerable, _Thunk(_IM_Result => { return p; }, _r3), true) );
            }

        label0: // ARGS 0
            _arg_input_index = _arg_index; // no-op for label

        }


        public void Whitespace(_LizpyParser_Memo _memo, int _index, _LizpyParser_Args _args)
        {

            int _arg_index = 0;
            int _arg_input_index = 0;

            // OR 0
            int _start_i0 = _index;

            // OR 1
            int _start_i1 = _index;

            // OR 2
            int _start_i2 = _index;

            // OR 3
            int _start_i3 = _index;

            // LITERAL ' '
            _ParseLiteralChar(_memo, ref _index, ' ');

            // OR shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Pop(); _index = _start_i3; } else goto label3;

            // LITERAL '\r'
            _ParseLiteralChar(_memo, ref _index, '\r');

        label3: // OR
            int _dummy_i3 = _index; // no-op for label

            // OR shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Pop(); _index = _start_i2; } else goto label2;

            // LITERAL '\n'
            _ParseLiteralChar(_memo, ref _index, '\n');

        label2: // OR
            int _dummy_i2 = _index; // no-op for label

            // OR shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Pop(); _index = _start_i1; } else goto label1;

            // LITERAL '\t'
            _ParseLiteralChar(_memo, ref _index, '\t');

        label1: // OR
            int _dummy_i1 = _index; // no-op for label

            // OR shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Pop(); _index = _start_i0; } else goto label0;

            // CALLORVAR Comment
            _LizpyParser_Item _r8;

            _r8 = _MemoCall(_memo, "Comment", _index, Comment, null);

            if (_r8 != null) _index = _r8.NextIndex;

        label0: // OR
            int _dummy_i0 = _index; // no-op for label

        }


        public void Comment(_LizpyParser_Memo _memo, int _index, _LizpyParser_Args _args)
        {

            int _arg_index = 0;
            int _arg_input_index = 0;

            // AND 0
            int _start_i0 = _index;

            // AND 1
            int _start_i1 = _index;

            // LITERAL ';'
            _ParseLiteralChar(_memo, ref _index, ';');

            // AND shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Push(null); goto label1; }

            // REGEXP [^\n]+
            _ParseRegexp(_memo, ref _index, _re0);

        label1: // AND
            var _r1_2 = _memo.Results.Pop();
            var _r1_1 = _memo.Results.Pop();

            if (_r1_1 != null && _r1_2 != null)
            {
                _memo.Results.Push( new _LizpyParser_Item(_start_i1, _index, _memo.InputEnumerable, _r1_1.Results.Concat(_r1_2.Results).Where(_NON_NULL), true) );
            }
            else
            {
                _memo.Results.Push(null);
                _index = _start_i1;
            }

            // AND shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Push(null); goto label0; }

            // LITERAL '\n'
            _ParseLiteralChar(_memo, ref _index, '\n');

        label0: // AND
            var _r0_2 = _memo.Results.Pop();
            var _r0_1 = _memo.Results.Pop();

            if (_r0_1 != null && _r0_2 != null)
            {
                _memo.Results.Push( new _LizpyParser_Item(_start_i0, _index, _memo.InputEnumerable, _r0_1.Results.Concat(_r0_2.Results).Where(_NON_NULL), true) );
            }
            else
            {
                _memo.Results.Push(null);
                _index = _start_i0;
            }

        }


        public void Atom(_LizpyParser_Memo _memo, int _index, _LizpyParser_Args _args)
        {

            int _arg_index = 0;
            int _arg_input_index = 0;

            // OR 0
            int _start_i0 = _index;

            // OR 1
            int _start_i1 = _index;

            // OR 2
            int _start_i2 = _index;

            // CALLORVAR Number
            _LizpyParser_Item _r3;

            _r3 = _MemoCall(_memo, "Number", _index, Number, null);

            if (_r3 != null) _index = _r3.NextIndex;

            // OR shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Pop(); _index = _start_i2; } else goto label2;

            // CALLORVAR String
            _LizpyParser_Item _r4;

            _r4 = _MemoCall(_memo, "String", _index, String, null);

            if (_r4 != null) _index = _r4.NextIndex;

        label2: // OR
            int _dummy_i2 = _index; // no-op for label

            // OR shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Pop(); _index = _start_i1; } else goto label1;

            // CALLORVAR Boolean
            _LizpyParser_Item _r5;

            _r5 = _MemoCall(_memo, "Boolean", _index, Boolean, null);

            if (_r5 != null) _index = _r5.NextIndex;

        label1: // OR
            int _dummy_i1 = _index; // no-op for label

            // OR shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Pop(); _index = _start_i0; } else goto label0;

            // CALLORVAR Symbol
            _LizpyParser_Item _r6;

            _r6 = _MemoCall(_memo, "Symbol", _index, Symbol, null);

            if (_r6 != null) _index = _r6.NextIndex;

        label0: // OR
            int _dummy_i0 = _index; // no-op for label

        }


        public void Number(_LizpyParser_Memo _memo, int _index, _LizpyParser_Args _args)
        {

            int _arg_index = 0;
            int _arg_input_index = 0;

            _LizpyParser_Item d = null;
            _LizpyParser_Item i = null;

            // OR 0
            int _start_i0 = _index;

            // OR 3
            int _start_i3 = _index;

            // CALLORVAR Decimal
            _LizpyParser_Item _r4;

            _r4 = _MemoCall(_memo, "Decimal", _index, Decimal, null);

            if (_r4 != null) _index = _r4.NextIndex;

            // OR shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Pop(); _index = _start_i3; } else goto label3;

            // CALLORVAR Scientific
            _LizpyParser_Item _r5;

            _r5 = _MemoCall(_memo, "Scientific", _index, Scientific, null);

            if (_r5 != null) _index = _r5.NextIndex;

        label3: // OR
            int _dummy_i3 = _index; // no-op for label

            // BIND d
            d = _memo.Results.Peek();

            // ACT
            var _r1 = _memo.Results.Peek();
            if (_r1 != null)
            {
                _memo.Results.Pop();
                _memo.Results.Push( new _LizpyParser_Item(_r1.StartIndex, _r1.NextIndex, _memo.InputEnumerable, _Thunk(_IM_Result => { return new DecimalExpression(d.StartIndex, d.NextIndex, Double.Parse(d.InputString())); }, _r1), true) );
            }

            // OR shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Pop(); _index = _start_i0; } else goto label0;

            // CALLORVAR Integer
            _LizpyParser_Item _r8;

            _r8 = _MemoCall(_memo, "Integer", _index, Integer, null);

            if (_r8 != null) _index = _r8.NextIndex;

            // BIND i
            i = _memo.Results.Peek();

            // ACT
            var _r6 = _memo.Results.Peek();
            if (_r6 != null)
            {
                _memo.Results.Pop();
                _memo.Results.Push( new _LizpyParser_Item(_r6.StartIndex, _r6.NextIndex, _memo.InputEnumerable, _Thunk(_IM_Result => { return new IntegerExpression(i.StartIndex, i.NextIndex, Int32.Parse(i.InputString())); }, _r6), true) );
            }

        label0: // OR
            int _dummy_i0 = _index; // no-op for label

        }


        public void Decimal(_LizpyParser_Memo _memo, int _index, _LizpyParser_Args _args)
        {

            int _arg_index = 0;
            int _arg_input_index = 0;

            // AND 0
            int _start_i0 = _index;

            // AND 1
            int _start_i1 = _index;

            // AND 2
            int _start_i2 = _index;

            // CALLORVAR Sign
            _LizpyParser_Item _r4;

            _r4 = _MemoCall(_memo, "Sign", _index, Sign, null);

            if (_r4 != null) _index = _r4.NextIndex;

            // QUES
            if (_memo.Results.Peek() == null) { _memo.Results.Pop(); _memo.Results.Push(new _LizpyParser_Item(_index, _memo.InputEnumerable)); }

            // AND shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Push(null); goto label2; }

            // PLUS 5
            int _start_i5 = _index;
            var _res5 = Enumerable.Empty<SExpression>();
        label5:

            // CALLORVAR Digit
            _LizpyParser_Item _r6;

            _r6 = _MemoCall(_memo, "Digit", _index, Digit, null);

            if (_r6 != null) _index = _r6.NextIndex;

            // PLUS 5
            var _r5 = _memo.Results.Pop();
            if (_r5 != null)
            {
                _res5 = _res5.Concat(_r5.Results);
                goto label5;
            }
            else
            {
                if (_index > _start_i5)
                    _memo.Results.Push(new _LizpyParser_Item(_start_i5, _index, _memo.InputEnumerable, _res5.Where(_NON_NULL), true));
                else
                    _memo.Results.Push(null);
            }

        label2: // AND
            var _r2_2 = _memo.Results.Pop();
            var _r2_1 = _memo.Results.Pop();

            if (_r2_1 != null && _r2_2 != null)
            {
                _memo.Results.Push( new _LizpyParser_Item(_start_i2, _index, _memo.InputEnumerable, _r2_1.Results.Concat(_r2_2.Results).Where(_NON_NULL), true) );
            }
            else
            {
                _memo.Results.Push(null);
                _index = _start_i2;
            }

            // AND shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Push(null); goto label1; }

            // LITERAL '.'
            _ParseLiteralChar(_memo, ref _index, '.');

        label1: // AND
            var _r1_2 = _memo.Results.Pop();
            var _r1_1 = _memo.Results.Pop();

            if (_r1_1 != null && _r1_2 != null)
            {
                _memo.Results.Push( new _LizpyParser_Item(_start_i1, _index, _memo.InputEnumerable, _r1_1.Results.Concat(_r1_2.Results).Where(_NON_NULL), true) );
            }
            else
            {
                _memo.Results.Push(null);
                _index = _start_i1;
            }

            // AND shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Push(null); goto label0; }

            // STAR 8
            int _start_i8 = _index;
            var _res8 = Enumerable.Empty<SExpression>();
        label8:

            // CALLORVAR Digit
            _LizpyParser_Item _r9;

            _r9 = _MemoCall(_memo, "Digit", _index, Digit, null);

            if (_r9 != null) _index = _r9.NextIndex;

            // STAR 8
            var _r8 = _memo.Results.Pop();
            if (_r8 != null)
            {
                _res8 = _res8.Concat(_r8.Results);
                goto label8;
            }
            else
            {
                _memo.Results.Push(new _LizpyParser_Item(_start_i8, _index, _memo.InputEnumerable, _res8.Where(_NON_NULL), true));
            }

        label0: // AND
            var _r0_2 = _memo.Results.Pop();
            var _r0_1 = _memo.Results.Pop();

            if (_r0_1 != null && _r0_2 != null)
            {
                _memo.Results.Push( new _LizpyParser_Item(_start_i0, _index, _memo.InputEnumerable, _r0_1.Results.Concat(_r0_2.Results).Where(_NON_NULL), true) );
            }
            else
            {
                _memo.Results.Push(null);
                _index = _start_i0;
            }

        }


        public void Sign(_LizpyParser_Memo _memo, int _index, _LizpyParser_Args _args)
        {

            int _arg_index = 0;
            int _arg_input_index = 0;

            // OR 0
            int _start_i0 = _index;

            // LITERAL '+'
            _ParseLiteralChar(_memo, ref _index, '+');

            // OR shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Pop(); _index = _start_i0; } else goto label0;

            // LITERAL '-'
            _ParseLiteralChar(_memo, ref _index, '-');

        label0: // OR
            int _dummy_i0 = _index; // no-op for label

        }


        public void Digit(_LizpyParser_Memo _memo, int _index, _LizpyParser_Args _args)
        {

            int _arg_index = 0;
            int _arg_input_index = 0;

            // REGEXP [0-9]
            _ParseRegexp(_memo, ref _index, _re1);

        }


        public void Scientific(_LizpyParser_Memo _memo, int _index, _LizpyParser_Args _args)
        {

            int _arg_index = 0;
            int _arg_input_index = 0;

            // AND 0
            int _start_i0 = _index;

            // AND 1
            int _start_i1 = _index;

            // CALLORVAR Decimal
            _LizpyParser_Item _r2;

            _r2 = _MemoCall(_memo, "Decimal", _index, Decimal, null);

            if (_r2 != null) _index = _r2.NextIndex;

            // AND shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Push(null); goto label1; }

            // OR 3
            int _start_i3 = _index;

            // LITERAL 'e'
            _ParseLiteralChar(_memo, ref _index, 'e');

            // OR shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Pop(); _index = _start_i3; } else goto label3;

            // LITERAL 'E'
            _ParseLiteralChar(_memo, ref _index, 'E');

        label3: // OR
            int _dummy_i3 = _index; // no-op for label

        label1: // AND
            var _r1_2 = _memo.Results.Pop();
            var _r1_1 = _memo.Results.Pop();

            if (_r1_1 != null && _r1_2 != null)
            {
                _memo.Results.Push( new _LizpyParser_Item(_start_i1, _index, _memo.InputEnumerable, _r1_1.Results.Concat(_r1_2.Results).Where(_NON_NULL), true) );
            }
            else
            {
                _memo.Results.Push(null);
                _index = _start_i1;
            }

            // AND shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Push(null); goto label0; }

            // CALLORVAR Integer
            _LizpyParser_Item _r6;

            _r6 = _MemoCall(_memo, "Integer", _index, Integer, null);

            if (_r6 != null) _index = _r6.NextIndex;

        label0: // AND
            var _r0_2 = _memo.Results.Pop();
            var _r0_1 = _memo.Results.Pop();

            if (_r0_1 != null && _r0_2 != null)
            {
                _memo.Results.Push( new _LizpyParser_Item(_start_i0, _index, _memo.InputEnumerable, _r0_1.Results.Concat(_r0_2.Results).Where(_NON_NULL), true) );
            }
            else
            {
                _memo.Results.Push(null);
                _index = _start_i0;
            }

        }


        public void Integer(_LizpyParser_Memo _memo, int _index, _LizpyParser_Args _args)
        {

            int _arg_index = 0;
            int _arg_input_index = 0;

            // AND 0
            int _start_i0 = _index;

            // CALLORVAR Sign
            _LizpyParser_Item _r2;

            _r2 = _MemoCall(_memo, "Sign", _index, Sign, null);

            if (_r2 != null) _index = _r2.NextIndex;

            // QUES
            if (_memo.Results.Peek() == null) { _memo.Results.Pop(); _memo.Results.Push(new _LizpyParser_Item(_index, _memo.InputEnumerable)); }

            // AND shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Push(null); goto label0; }

            // PLUS 3
            int _start_i3 = _index;
            var _res3 = Enumerable.Empty<SExpression>();
        label3:

            // CALLORVAR Digit
            _LizpyParser_Item _r4;

            _r4 = _MemoCall(_memo, "Digit", _index, Digit, null);

            if (_r4 != null) _index = _r4.NextIndex;

            // PLUS 3
            var _r3 = _memo.Results.Pop();
            if (_r3 != null)
            {
                _res3 = _res3.Concat(_r3.Results);
                goto label3;
            }
            else
            {
                if (_index > _start_i3)
                    _memo.Results.Push(new _LizpyParser_Item(_start_i3, _index, _memo.InputEnumerable, _res3.Where(_NON_NULL), true));
                else
                    _memo.Results.Push(null);
            }

        label0: // AND
            var _r0_2 = _memo.Results.Pop();
            var _r0_1 = _memo.Results.Pop();

            if (_r0_1 != null && _r0_2 != null)
            {
                _memo.Results.Push( new _LizpyParser_Item(_start_i0, _index, _memo.InputEnumerable, _r0_1.Results.Concat(_r0_2.Results).Where(_NON_NULL), true) );
            }
            else
            {
                _memo.Results.Push(null);
                _index = _start_i0;
            }

        }


        public void String(_LizpyParser_Memo _memo, int _index, _LizpyParser_Args _args)
        {

            int _arg_index = 0;
            int _arg_input_index = 0;

            _LizpyParser_Item cs = null;
            _LizpyParser_Item str = null;

            // AND 2
            int _start_i2 = _index;

            // AND 3
            int _start_i3 = _index;

            // LITERAL '"'
            _ParseLiteralChar(_memo, ref _index, '"');

            // AND shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Push(null); goto label3; }

            // STAR 6
            int _start_i6 = _index;
            var _res6 = Enumerable.Empty<SExpression>();
        label6:

            // CALLORVAR StringCharacterExpression
            _LizpyParser_Item _r7;

            _r7 = _MemoCall(_memo, "StringCharacterExpression", _index, StringCharacterExpression, null);

            if (_r7 != null) _index = _r7.NextIndex;

            // STAR 6
            var _r6 = _memo.Results.Pop();
            if (_r6 != null)
            {
                _res6 = _res6.Concat(_r6.Results);
                goto label6;
            }
            else
            {
                _memo.Results.Push(new _LizpyParser_Item(_start_i6, _index, _memo.InputEnumerable, _res6.Where(_NON_NULL), true));
            }

            // BIND cs
            cs = _memo.Results.Peek();

        label3: // AND
            var _r3_2 = _memo.Results.Pop();
            var _r3_1 = _memo.Results.Pop();

            if (_r3_1 != null && _r3_2 != null)
            {
                _memo.Results.Push( new _LizpyParser_Item(_start_i3, _index, _memo.InputEnumerable, _r3_1.Results.Concat(_r3_2.Results).Where(_NON_NULL), true) );
            }
            else
            {
                _memo.Results.Push(null);
                _index = _start_i3;
            }

            // AND shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Push(null); goto label2; }

            // LITERAL '"'
            _ParseLiteralChar(_memo, ref _index, '"');

        label2: // AND
            var _r2_2 = _memo.Results.Pop();
            var _r2_1 = _memo.Results.Pop();

            if (_r2_1 != null && _r2_2 != null)
            {
                _memo.Results.Push( new _LizpyParser_Item(_start_i2, _index, _memo.InputEnumerable, _r2_1.Results.Concat(_r2_2.Results).Where(_NON_NULL), true) );
            }
            else
            {
                _memo.Results.Push(null);
                _index = _start_i2;
            }

            // BIND str
            str = _memo.Results.Peek();

            // ACT
            var _r0 = _memo.Results.Peek();
            if (_r0 != null)
            {
                _memo.Results.Pop();
                _memo.Results.Push( new _LizpyParser_Item(_r0.StartIndex, _r0.NextIndex, _memo.InputEnumerable, _Thunk(_IM_Result => { return new StringExpression(str.StartIndex, str.NextIndex, cs.ResultsAs<CharacterExpression>()); }, _r0), true) );
            }

        }


        public void StringCharacterExpression(_LizpyParser_Memo _memo, int _index, _LizpyParser_Args _args)
        {

            int _arg_index = 0;
            int _arg_input_index = 0;

            _LizpyParser_Item c = null;

            // OR 0
            int _start_i0 = _index;

            // REGEXP [^""\\]
            _ParseRegexp(_memo, ref _index, _re2);

            // BIND c
            c = _memo.Results.Peek();

            // ACT
            var _r1 = _memo.Results.Peek();
            if (_r1 != null)
            {
                _memo.Results.Pop();
                _memo.Results.Push( new _LizpyParser_Item(_r1.StartIndex, _r1.NextIndex, _memo.InputEnumerable, _Thunk(_IM_Result => { return new CharacterExpression(c); }, _r1), true) );
            }

            // OR shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Pop(); _index = _start_i0; } else goto label0;

            // CALLORVAR EscapeSequence
            _LizpyParser_Item _r4;

            _r4 = _MemoCall(_memo, "EscapeSequence", _index, EscapeSequence, null);

            if (_r4 != null) _index = _r4.NextIndex;

        label0: // OR
            int _dummy_i0 = _index; // no-op for label

        }


        public void EscapeSequence(_LizpyParser_Memo _memo, int _index, _LizpyParser_Args _args)
        {

            int _arg_index = 0;
            int _arg_input_index = 0;

            _LizpyParser_Item c = null;

            // OR 0
            int _start_i0 = _index;

            // OR 1
            int _start_i1 = _index;

            // OR 2
            int _start_i2 = _index;

            // OR 3
            int _start_i3 = _index;

            // LITERAL "\\n"
            _ParseLiteralString(_memo, ref _index, "\\n");

            // ACT
            var _r4 = _memo.Results.Peek();
            if (_r4 != null)
            {
                _memo.Results.Pop();
                _memo.Results.Push( new _LizpyParser_Item(_r4.StartIndex, _r4.NextIndex, _memo.InputEnumerable, _Thunk(_IM_Result => { return new CharacterExpression('\n'); }, _r4), true) );
            }

            // OR shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Pop(); _index = _start_i3; } else goto label3;

            // LITERAL "\\r"
            _ParseLiteralString(_memo, ref _index, "\\r");

            // ACT
            var _r6 = _memo.Results.Peek();
            if (_r6 != null)
            {
                _memo.Results.Pop();
                _memo.Results.Push( new _LizpyParser_Item(_r6.StartIndex, _r6.NextIndex, _memo.InputEnumerable, _Thunk(_IM_Result => { return new CharacterExpression('\r'); }, _r6), true) );
            }

        label3: // OR
            int _dummy_i3 = _index; // no-op for label

            // OR shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Pop(); _index = _start_i2; } else goto label2;

            // LITERAL "\\t"
            _ParseLiteralString(_memo, ref _index, "\\t");

            // ACT
            var _r8 = _memo.Results.Peek();
            if (_r8 != null)
            {
                _memo.Results.Pop();
                _memo.Results.Push( new _LizpyParser_Item(_r8.StartIndex, _r8.NextIndex, _memo.InputEnumerable, _Thunk(_IM_Result => { return new CharacterExpression('\t'); }, _r8), true) );
            }

        label2: // OR
            int _dummy_i2 = _index; // no-op for label

            // OR shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Pop(); _index = _start_i1; } else goto label1;

            // LITERAL "\\0"
            _ParseLiteralString(_memo, ref _index, "\\0");

            // ACT
            var _r10 = _memo.Results.Peek();
            if (_r10 != null)
            {
                _memo.Results.Pop();
                _memo.Results.Push( new _LizpyParser_Item(_r10.StartIndex, _r10.NextIndex, _memo.InputEnumerable, _Thunk(_IM_Result => { return new CharacterExpression('\0'); }, _r10), true) );
            }

        label1: // OR
            int _dummy_i1 = _index; // no-op for label

            // OR shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Pop(); _index = _start_i0; } else goto label0;

            // AND 13
            int _start_i13 = _index;

            // LITERAL '\\'
            _ParseLiteralChar(_memo, ref _index, '\\');

            // AND shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Push(null); goto label13; }

            // ANY
            _ParseAny(_memo, ref _index);

            // BIND c
            c = _memo.Results.Peek();

        label13: // AND
            var _r13_2 = _memo.Results.Pop();
            var _r13_1 = _memo.Results.Pop();

            if (_r13_1 != null && _r13_2 != null)
            {
                _memo.Results.Push( new _LizpyParser_Item(_start_i13, _index, _memo.InputEnumerable, _r13_1.Results.Concat(_r13_2.Results).Where(_NON_NULL), true) );
            }
            else
            {
                _memo.Results.Push(null);
                _index = _start_i13;
            }

            // ACT
            var _r12 = _memo.Results.Peek();
            if (_r12 != null)
            {
                _memo.Results.Pop();
                _memo.Results.Push( new _LizpyParser_Item(_r12.StartIndex, _r12.NextIndex, _memo.InputEnumerable, _Thunk(_IM_Result => { return new CharacterExpression(c); }, _r12), true) );
            }

        label0: // OR
            int _dummy_i0 = _index; // no-op for label

        }


        public void Boolean(_LizpyParser_Memo _memo, int _index, _LizpyParser_Args _args)
        {

            int _arg_index = 0;
            int _arg_input_index = 0;

            _LizpyParser_Item v = null;

            // OR 0
            int _start_i0 = _index;

            // LITERAL "true"
            _ParseLiteralString(_memo, ref _index, "true");

            // BIND v
            v = _memo.Results.Peek();

            // ACT
            var _r1 = _memo.Results.Peek();
            if (_r1 != null)
            {
                _memo.Results.Pop();
                _memo.Results.Push( new _LizpyParser_Item(_r1.StartIndex, _r1.NextIndex, _memo.InputEnumerable, _Thunk(_IM_Result => { return new BooleanExpression(v.StartIndex, v.NextIndex, true); }, _r1), true) );
            }

            // OR shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Pop(); _index = _start_i0; } else goto label0;

            // LITERAL "false"
            _ParseLiteralString(_memo, ref _index, "false");

            // BIND v
            v = _memo.Results.Peek();

            // ACT
            var _r4 = _memo.Results.Peek();
            if (_r4 != null)
            {
                _memo.Results.Pop();
                _memo.Results.Push( new _LizpyParser_Item(_r4.StartIndex, _r4.NextIndex, _memo.InputEnumerable, _Thunk(_IM_Result => { return new BooleanExpression(v.StartIndex, v.NextIndex, false); }, _r4), true) );
            }

        label0: // OR
            int _dummy_i0 = _index; // no-op for label

        }


        public void Symbol(_LizpyParser_Memo _memo, int _index, _LizpyParser_Args _args)
        {

            int _arg_index = 0;
            int _arg_input_index = 0;

            _LizpyParser_Item s = null;
            _LizpyParser_Item m = null;

            // AND 1
            int _start_i1 = _index;

            // REGEXP [A-Za-z\`~!@#$%^&*\-_=+|;:,<\.>\/?][0-9A-Za-z\`~!@#$%^&*\-_=+|;:',<\.>\/?]*
            _ParseRegexp(_memo, ref _index, _re3);

            // BIND s
            s = _memo.Results.Peek();

            // AND shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Push(null); goto label1; }

            // CALLORVAR Metadata
            _LizpyParser_Item _r6;

            _r6 = _MemoCall(_memo, "Metadata", _index, Metadata, null);

            if (_r6 != null) _index = _r6.NextIndex;

            // QUES
            if (_memo.Results.Peek() == null) { _memo.Results.Pop(); _memo.Results.Push(new _LizpyParser_Item(_index, _memo.InputEnumerable)); }

            // BIND m
            m = _memo.Results.Peek();

        label1: // AND
            var _r1_2 = _memo.Results.Pop();
            var _r1_1 = _memo.Results.Pop();

            if (_r1_1 != null && _r1_2 != null)
            {
                _memo.Results.Push( new _LizpyParser_Item(_start_i1, _index, _memo.InputEnumerable, _r1_1.Results.Concat(_r1_2.Results).Where(_NON_NULL), true) );
            }
            else
            {
                _memo.Results.Push(null);
                _index = _start_i1;
            }

            // ACT
            var _r0 = _memo.Results.Peek();
            if (_r0 != null)
            {
                _memo.Results.Pop();
                _memo.Results.Push( new _LizpyParser_Item(_r0.StartIndex, _r0.NextIndex, _memo.InputEnumerable, _Thunk(_IM_Result => { return new SymbolExpression(s.StartIndex, s.NextIndex, s.InputString(), m?.ResultAs<MapExpression>()); }, _r0), true) );
            }

        }


        public void Metadata(_LizpyParser_Memo _memo, int _index, _LizpyParser_Args _args)
        {

            int _arg_index = 0;
            int _arg_input_index = 0;

            _LizpyParser_Item m = null;

            // AND 1
            int _start_i1 = _index;

            // REGEXP \s+^
            _ParseRegexp(_memo, ref _index, _re4);

            // AND shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Push(null); goto label1; }

            // CALLORVAR Map
            _LizpyParser_Item _r4;

            _r4 = _MemoCall(_memo, "Map", _index, Map, null);

            if (_r4 != null) _index = _r4.NextIndex;

            // BIND m
            m = _memo.Results.Peek();

        label1: // AND
            var _r1_2 = _memo.Results.Pop();
            var _r1_1 = _memo.Results.Pop();

            if (_r1_1 != null && _r1_2 != null)
            {
                _memo.Results.Push( new _LizpyParser_Item(_start_i1, _index, _memo.InputEnumerable, _r1_1.Results.Concat(_r1_2.Results).Where(_NON_NULL), true) );
            }
            else
            {
                _memo.Results.Push(null);
                _index = _start_i1;
            }

            // ACT
            var _r0 = _memo.Results.Peek();
            if (_r0 != null)
            {
                _memo.Results.Pop();
                _memo.Results.Push( new _LizpyParser_Item(_r0.StartIndex, _r0.NextIndex, _memo.InputEnumerable, _Thunk(_IM_Result => { return m; }, _r0), true) );
            }

        }


        public void Map(_LizpyParser_Memo _memo, int _index, _LizpyParser_Args _args)
        {

            int _arg_index = 0;
            int _arg_input_index = 0;

            _LizpyParser_Item kvps = null;
            _LizpyParser_Item m = null;

            // AND 2
            int _start_i2 = _index;

            // AND 3
            int _start_i3 = _index;

            // AND 4
            int _start_i4 = _index;

            // LITERAL '{'
            _ParseLiteralChar(_memo, ref _index, '{');

            // AND shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Push(null); goto label4; }

            // STAR 6
            int _start_i6 = _index;
            var _res6 = Enumerable.Empty<SExpression>();
        label6:

            // CALLORVAR Whitespace
            _LizpyParser_Item _r7;

            _r7 = _MemoCall(_memo, "Whitespace", _index, Whitespace, null);

            if (_r7 != null) _index = _r7.NextIndex;

            // STAR 6
            var _r6 = _memo.Results.Pop();
            if (_r6 != null)
            {
                _res6 = _res6.Concat(_r6.Results);
                goto label6;
            }
            else
            {
                _memo.Results.Push(new _LizpyParser_Item(_start_i6, _index, _memo.InputEnumerable, _res6.Where(_NON_NULL), true));
            }

        label4: // AND
            var _r4_2 = _memo.Results.Pop();
            var _r4_1 = _memo.Results.Pop();

            if (_r4_1 != null && _r4_2 != null)
            {
                _memo.Results.Push( new _LizpyParser_Item(_start_i4, _index, _memo.InputEnumerable, _r4_1.Results.Concat(_r4_2.Results).Where(_NON_NULL), true) );
            }
            else
            {
                _memo.Results.Push(null);
                _index = _start_i4;
            }

            // AND shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Push(null); goto label3; }

            // STAR 9
            int _start_i9 = _index;
            var _res9 = Enumerable.Empty<SExpression>();
        label9:

            // CALLORVAR KeyValuePair
            _LizpyParser_Item _r10;

            _r10 = _MemoCall(_memo, "KeyValuePair", _index, KeyValuePair, null);

            if (_r10 != null) _index = _r10.NextIndex;

            // STAR 9
            var _r9 = _memo.Results.Pop();
            if (_r9 != null)
            {
                _res9 = _res9.Concat(_r9.Results);
                goto label9;
            }
            else
            {
                _memo.Results.Push(new _LizpyParser_Item(_start_i9, _index, _memo.InputEnumerable, _res9.Where(_NON_NULL), true));
            }

            // BIND kvps
            kvps = _memo.Results.Peek();

        label3: // AND
            var _r3_2 = _memo.Results.Pop();
            var _r3_1 = _memo.Results.Pop();

            if (_r3_1 != null && _r3_2 != null)
            {
                _memo.Results.Push( new _LizpyParser_Item(_start_i3, _index, _memo.InputEnumerable, _r3_1.Results.Concat(_r3_2.Results).Where(_NON_NULL), true) );
            }
            else
            {
                _memo.Results.Push(null);
                _index = _start_i3;
            }

            // AND shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Push(null); goto label2; }

            // LITERAL '}'
            _ParseLiteralChar(_memo, ref _index, '}');

        label2: // AND
            var _r2_2 = _memo.Results.Pop();
            var _r2_1 = _memo.Results.Pop();

            if (_r2_1 != null && _r2_2 != null)
            {
                _memo.Results.Push( new _LizpyParser_Item(_start_i2, _index, _memo.InputEnumerable, _r2_1.Results.Concat(_r2_2.Results).Where(_NON_NULL), true) );
            }
            else
            {
                _memo.Results.Push(null);
                _index = _start_i2;
            }

            // BIND m
            m = _memo.Results.Peek();

            // ACT
            var _r0 = _memo.Results.Peek();
            if (_r0 != null)
            {
                _memo.Results.Pop();
                _memo.Results.Push( new _LizpyParser_Item(_r0.StartIndex, _r0.NextIndex, _memo.InputEnumerable, _Thunk(_IM_Result => { return new MapExpression(m.StartIndex, m.NextIndex, kvps.ResultsAs<ListExpression>()); }, _r0), true) );
            }

        }


        public void KeyValuePair(_LizpyParser_Memo _memo, int _index, _LizpyParser_Args _args)
        {

            int _arg_index = 0;
            int _arg_input_index = 0;

            _LizpyParser_Item k = null;
            _LizpyParser_Item v = null;

            // AND 1
            int _start_i1 = _index;

            // AND 2
            int _start_i2 = _index;

            // AND 3
            int _start_i3 = _index;

            // AND 4
            int _start_i4 = _index;

            // AND 5
            int _start_i5 = _index;

            // CALLORVAR Atom
            _LizpyParser_Item _r7;

            _r7 = _MemoCall(_memo, "Atom", _index, Atom, null);

            if (_r7 != null) _index = _r7.NextIndex;

            // BIND k
            k = _memo.Results.Peek();

            // AND shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Push(null); goto label5; }

            // PLUS 8
            int _start_i8 = _index;
            var _res8 = Enumerable.Empty<SExpression>();
        label8:

            // CALLORVAR Whitespace
            _LizpyParser_Item _r9;

            _r9 = _MemoCall(_memo, "Whitespace", _index, Whitespace, null);

            if (_r9 != null) _index = _r9.NextIndex;

            // PLUS 8
            var _r8 = _memo.Results.Pop();
            if (_r8 != null)
            {
                _res8 = _res8.Concat(_r8.Results);
                goto label8;
            }
            else
            {
                if (_index > _start_i8)
                    _memo.Results.Push(new _LizpyParser_Item(_start_i8, _index, _memo.InputEnumerable, _res8.Where(_NON_NULL), true));
                else
                    _memo.Results.Push(null);
            }

        label5: // AND
            var _r5_2 = _memo.Results.Pop();
            var _r5_1 = _memo.Results.Pop();

            if (_r5_1 != null && _r5_2 != null)
            {
                _memo.Results.Push( new _LizpyParser_Item(_start_i5, _index, _memo.InputEnumerable, _r5_1.Results.Concat(_r5_2.Results).Where(_NON_NULL), true) );
            }
            else
            {
                _memo.Results.Push(null);
                _index = _start_i5;
            }

            // AND shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Push(null); goto label4; }

            // CALLORVAR SExpression
            _LizpyParser_Item _r11;

            _r11 = _MemoCall(_memo, "SExpression", _index, SExpression, null);

            if (_r11 != null) _index = _r11.NextIndex;

            // BIND v
            v = _memo.Results.Peek();

        label4: // AND
            var _r4_2 = _memo.Results.Pop();
            var _r4_1 = _memo.Results.Pop();

            if (_r4_1 != null && _r4_2 != null)
            {
                _memo.Results.Push( new _LizpyParser_Item(_start_i4, _index, _memo.InputEnumerable, _r4_1.Results.Concat(_r4_2.Results).Where(_NON_NULL), true) );
            }
            else
            {
                _memo.Results.Push(null);
                _index = _start_i4;
            }

            // AND shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Push(null); goto label3; }

            // STAR 12
            int _start_i12 = _index;
            var _res12 = Enumerable.Empty<SExpression>();
        label12:

            // CALLORVAR Whitespace
            _LizpyParser_Item _r13;

            _r13 = _MemoCall(_memo, "Whitespace", _index, Whitespace, null);

            if (_r13 != null) _index = _r13.NextIndex;

            // STAR 12
            var _r12 = _memo.Results.Pop();
            if (_r12 != null)
            {
                _res12 = _res12.Concat(_r12.Results);
                goto label12;
            }
            else
            {
                _memo.Results.Push(new _LizpyParser_Item(_start_i12, _index, _memo.InputEnumerable, _res12.Where(_NON_NULL), true));
            }

        label3: // AND
            var _r3_2 = _memo.Results.Pop();
            var _r3_1 = _memo.Results.Pop();

            if (_r3_1 != null && _r3_2 != null)
            {
                _memo.Results.Push( new _LizpyParser_Item(_start_i3, _index, _memo.InputEnumerable, _r3_1.Results.Concat(_r3_2.Results).Where(_NON_NULL), true) );
            }
            else
            {
                _memo.Results.Push(null);
                _index = _start_i3;
            }

            // AND shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Push(null); goto label2; }

            // LITERAL ','
            _ParseLiteralChar(_memo, ref _index, ',');

            // QUES
            if (_memo.Results.Peek() == null) { _memo.Results.Pop(); _memo.Results.Push(new _LizpyParser_Item(_index, _memo.InputEnumerable)); }

        label2: // AND
            var _r2_2 = _memo.Results.Pop();
            var _r2_1 = _memo.Results.Pop();

            if (_r2_1 != null && _r2_2 != null)
            {
                _memo.Results.Push( new _LizpyParser_Item(_start_i2, _index, _memo.InputEnumerable, _r2_1.Results.Concat(_r2_2.Results).Where(_NON_NULL), true) );
            }
            else
            {
                _memo.Results.Push(null);
                _index = _start_i2;
            }

            // AND shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Push(null); goto label1; }

            // STAR 16
            int _start_i16 = _index;
            var _res16 = Enumerable.Empty<SExpression>();
        label16:

            // CALLORVAR Whitespace
            _LizpyParser_Item _r17;

            _r17 = _MemoCall(_memo, "Whitespace", _index, Whitespace, null);

            if (_r17 != null) _index = _r17.NextIndex;

            // STAR 16
            var _r16 = _memo.Results.Pop();
            if (_r16 != null)
            {
                _res16 = _res16.Concat(_r16.Results);
                goto label16;
            }
            else
            {
                _memo.Results.Push(new _LizpyParser_Item(_start_i16, _index, _memo.InputEnumerable, _res16.Where(_NON_NULL), true));
            }

        label1: // AND
            var _r1_2 = _memo.Results.Pop();
            var _r1_1 = _memo.Results.Pop();

            if (_r1_1 != null && _r1_2 != null)
            {
                _memo.Results.Push( new _LizpyParser_Item(_start_i1, _index, _memo.InputEnumerable, _r1_1.Results.Concat(_r1_2.Results).Where(_NON_NULL), true) );
            }
            else
            {
                _memo.Results.Push(null);
                _index = _start_i1;
            }

            // ACT
            var _r0 = _memo.Results.Peek();
            if (_r0 != null)
            {
                _memo.Results.Pop();
                _memo.Results.Push( new _LizpyParser_Item(_r0.StartIndex, _r0.NextIndex, _memo.InputEnumerable, _Thunk(_IM_Result => { return new ListExpression(k.StartIndex, v.NextIndex, new SExpression[] { k, v }); }, _r0), true) );
            }

        }


        public void List(_LizpyParser_Memo _memo, int _index, _LizpyParser_Args _args)
        {

            int _arg_index = 0;
            int _arg_input_index = 0;

            _LizpyParser_Item items = null;
            _LizpyParser_Item l = null;

            // AND 2
            int _start_i2 = _index;

            // AND 3
            int _start_i3 = _index;

            // AND 4
            int _start_i4 = _index;

            // LITERAL '('
            _ParseLiteralChar(_memo, ref _index, '(');

            // AND shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Push(null); goto label4; }

            // STAR 6
            int _start_i6 = _index;
            var _res6 = Enumerable.Empty<SExpression>();
        label6:

            // CALLORVAR Whitespace
            _LizpyParser_Item _r7;

            _r7 = _MemoCall(_memo, "Whitespace", _index, Whitespace, null);

            if (_r7 != null) _index = _r7.NextIndex;

            // STAR 6
            var _r6 = _memo.Results.Pop();
            if (_r6 != null)
            {
                _res6 = _res6.Concat(_r6.Results);
                goto label6;
            }
            else
            {
                _memo.Results.Push(new _LizpyParser_Item(_start_i6, _index, _memo.InputEnumerable, _res6.Where(_NON_NULL), true));
            }

        label4: // AND
            var _r4_2 = _memo.Results.Pop();
            var _r4_1 = _memo.Results.Pop();

            if (_r4_1 != null && _r4_2 != null)
            {
                _memo.Results.Push( new _LizpyParser_Item(_start_i4, _index, _memo.InputEnumerable, _r4_1.Results.Concat(_r4_2.Results).Where(_NON_NULL), true) );
            }
            else
            {
                _memo.Results.Push(null);
                _index = _start_i4;
            }

            // AND shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Push(null); goto label3; }

            // STAR 9
            int _start_i9 = _index;
            var _res9 = Enumerable.Empty<SExpression>();
        label9:

            // CALL ListItem
            var _start_i10 = _index;
            _LizpyParser_Item _r10;

            _LizpyParser_Args _actual_args10 = new _LizpyParser_Item[] { new _LizpyParser_Item(SExpression) };
            if (_args != null) _actual_args10 = _actual_args10.Concat(_args.Skip(_arg_index));
            _r10 = _MemoCall(_memo, "ListItem", _index, ListItem, _actual_args10);

            if (_r10 != null) _index = _r10.NextIndex;

            // STAR 9
            var _r9 = _memo.Results.Pop();
            if (_r9 != null)
            {
                _res9 = _res9.Concat(_r9.Results);
                goto label9;
            }
            else
            {
                _memo.Results.Push(new _LizpyParser_Item(_start_i9, _index, _memo.InputEnumerable, _res9.Where(_NON_NULL), true));
            }

            // BIND items
            items = _memo.Results.Peek();

        label3: // AND
            var _r3_2 = _memo.Results.Pop();
            var _r3_1 = _memo.Results.Pop();

            if (_r3_1 != null && _r3_2 != null)
            {
                _memo.Results.Push( new _LizpyParser_Item(_start_i3, _index, _memo.InputEnumerable, _r3_1.Results.Concat(_r3_2.Results).Where(_NON_NULL), true) );
            }
            else
            {
                _memo.Results.Push(null);
                _index = _start_i3;
            }

            // AND shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Push(null); goto label2; }

            // LITERAL ')'
            _ParseLiteralChar(_memo, ref _index, ')');

        label2: // AND
            var _r2_2 = _memo.Results.Pop();
            var _r2_1 = _memo.Results.Pop();

            if (_r2_1 != null && _r2_2 != null)
            {
                _memo.Results.Push( new _LizpyParser_Item(_start_i2, _index, _memo.InputEnumerable, _r2_1.Results.Concat(_r2_2.Results).Where(_NON_NULL), true) );
            }
            else
            {
                _memo.Results.Push(null);
                _index = _start_i2;
            }

            // BIND l
            l = _memo.Results.Peek();

            // ACT
            var _r0 = _memo.Results.Peek();
            if (_r0 != null)
            {
                _memo.Results.Pop();
                _memo.Results.Push( new _LizpyParser_Item(_r0.StartIndex, _r0.NextIndex, _memo.InputEnumerable, _Thunk(_IM_Result => { return new ListExpression(l.StartIndex, l.NextIndex, items.Results); }, _r0), true) );
            }

        }


        public void ListItem(_LizpyParser_Memo _memo, int _index, _LizpyParser_Args _args)
        {

            int _arg_index = 0;
            int _arg_input_index = 0;

            _LizpyParser_Item pattern = null;
            _LizpyParser_Item item = null;

            // ARGS 0
            _arg_index = 0;
            _arg_input_index = 0;

            // ANY
            _ParseAnyArgs(_memo, ref _arg_index, ref _arg_input_index, _args);

            // BIND pattern
            pattern = _memo.ArgResults.Peek();

            if (_memo.ArgResults.Pop() == null)
            {
                _memo.Results.Push(null);
                goto label0;
            }

            // AND 4
            int _start_i4 = _index;

            // AND 5
            int _start_i5 = _index;

            // AND 6
            int _start_i6 = _index;

            // CALLORVAR pattern
            _LizpyParser_Item _r8;

            if (pattern.Production != null)
            {
                var _p8 = (System.Action<_LizpyParser_Memo, int, IEnumerable<_LizpyParser_Item>>)(object)pattern.Production;
                _r8 = _MemoCall(_memo, pattern.Production.Method.Name, _index, _p8, _args != null ? _args.Skip(_arg_index) : null);
            }
            else
            {
                _r8 = _ParseLiteralObj(_memo, ref _index, pattern.Inputs);
            }

            if (_r8 != null) _index = _r8.NextIndex;

            // BIND item
            item = _memo.Results.Peek();

            // AND shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Push(null); goto label6; }

            // STAR 9
            int _start_i9 = _index;
            var _res9 = Enumerable.Empty<SExpression>();
        label9:

            // CALLORVAR Whitespace
            _LizpyParser_Item _r10;

            _r10 = _MemoCall(_memo, "Whitespace", _index, Whitespace, null);

            if (_r10 != null) _index = _r10.NextIndex;

            // STAR 9
            var _r9 = _memo.Results.Pop();
            if (_r9 != null)
            {
                _res9 = _res9.Concat(_r9.Results);
                goto label9;
            }
            else
            {
                _memo.Results.Push(new _LizpyParser_Item(_start_i9, _index, _memo.InputEnumerable, _res9.Where(_NON_NULL), true));
            }

        label6: // AND
            var _r6_2 = _memo.Results.Pop();
            var _r6_1 = _memo.Results.Pop();

            if (_r6_1 != null && _r6_2 != null)
            {
                _memo.Results.Push( new _LizpyParser_Item(_start_i6, _index, _memo.InputEnumerable, _r6_1.Results.Concat(_r6_2.Results).Where(_NON_NULL), true) );
            }
            else
            {
                _memo.Results.Push(null);
                _index = _start_i6;
            }

            // AND shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Push(null); goto label5; }

            // LITERAL ','
            _ParseLiteralChar(_memo, ref _index, ',');

            // QUES
            if (_memo.Results.Peek() == null) { _memo.Results.Pop(); _memo.Results.Push(new _LizpyParser_Item(_index, _memo.InputEnumerable)); }

        label5: // AND
            var _r5_2 = _memo.Results.Pop();
            var _r5_1 = _memo.Results.Pop();

            if (_r5_1 != null && _r5_2 != null)
            {
                _memo.Results.Push( new _LizpyParser_Item(_start_i5, _index, _memo.InputEnumerable, _r5_1.Results.Concat(_r5_2.Results).Where(_NON_NULL), true) );
            }
            else
            {
                _memo.Results.Push(null);
                _index = _start_i5;
            }

            // AND shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Push(null); goto label4; }

            // STAR 13
            int _start_i13 = _index;
            var _res13 = Enumerable.Empty<SExpression>();
        label13:

            // CALLORVAR Whitespace
            _LizpyParser_Item _r14;

            _r14 = _MemoCall(_memo, "Whitespace", _index, Whitespace, null);

            if (_r14 != null) _index = _r14.NextIndex;

            // STAR 13
            var _r13 = _memo.Results.Pop();
            if (_r13 != null)
            {
                _res13 = _res13.Concat(_r13.Results);
                goto label13;
            }
            else
            {
                _memo.Results.Push(new _LizpyParser_Item(_start_i13, _index, _memo.InputEnumerable, _res13.Where(_NON_NULL), true));
            }

        label4: // AND
            var _r4_2 = _memo.Results.Pop();
            var _r4_1 = _memo.Results.Pop();

            if (_r4_1 != null && _r4_2 != null)
            {
                _memo.Results.Push( new _LizpyParser_Item(_start_i4, _index, _memo.InputEnumerable, _r4_1.Results.Concat(_r4_2.Results).Where(_NON_NULL), true) );
            }
            else
            {
                _memo.Results.Push(null);
                _index = _start_i4;
            }

            // ACT
            var _r3 = _memo.Results.Peek();
            if (_r3 != null)
            {
                _memo.Results.Pop();
                _memo.Results.Push( new _LizpyParser_Item(_r3.StartIndex, _r3.NextIndex, _memo.InputEnumerable, _Thunk(_IM_Result => { return item; }, _r3), true) );
            }

        label0: // ARGS 0
            _arg_input_index = _arg_index; // no-op for label

        }


        public void Array(_LizpyParser_Memo _memo, int _index, _LizpyParser_Args _args)
        {

            int _arg_index = 0;
            int _arg_input_index = 0;

            _LizpyParser_Item items = null;
            _LizpyParser_Item a = null;

            // AND 2
            int _start_i2 = _index;

            // AND 3
            int _start_i3 = _index;

            // AND 4
            int _start_i4 = _index;

            // LITERAL '['
            _ParseLiteralChar(_memo, ref _index, '[');

            // AND shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Push(null); goto label4; }

            // STAR 6
            int _start_i6 = _index;
            var _res6 = Enumerable.Empty<SExpression>();
        label6:

            // CALLORVAR Whitespace
            _LizpyParser_Item _r7;

            _r7 = _MemoCall(_memo, "Whitespace", _index, Whitespace, null);

            if (_r7 != null) _index = _r7.NextIndex;

            // STAR 6
            var _r6 = _memo.Results.Pop();
            if (_r6 != null)
            {
                _res6 = _res6.Concat(_r6.Results);
                goto label6;
            }
            else
            {
                _memo.Results.Push(new _LizpyParser_Item(_start_i6, _index, _memo.InputEnumerable, _res6.Where(_NON_NULL), true));
            }

        label4: // AND
            var _r4_2 = _memo.Results.Pop();
            var _r4_1 = _memo.Results.Pop();

            if (_r4_1 != null && _r4_2 != null)
            {
                _memo.Results.Push( new _LizpyParser_Item(_start_i4, _index, _memo.InputEnumerable, _r4_1.Results.Concat(_r4_2.Results).Where(_NON_NULL), true) );
            }
            else
            {
                _memo.Results.Push(null);
                _index = _start_i4;
            }

            // AND shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Push(null); goto label3; }

            // STAR 9
            int _start_i9 = _index;
            var _res9 = Enumerable.Empty<SExpression>();
        label9:

            // CALL ListItem
            var _start_i10 = _index;
            _LizpyParser_Item _r10;

            _LizpyParser_Args _actual_args10 = new _LizpyParser_Item[] { new _LizpyParser_Item(Atom) };
            if (_args != null) _actual_args10 = _actual_args10.Concat(_args.Skip(_arg_index));
            _r10 = _MemoCall(_memo, "ListItem", _index, ListItem, _actual_args10);

            if (_r10 != null) _index = _r10.NextIndex;

            // STAR 9
            var _r9 = _memo.Results.Pop();
            if (_r9 != null)
            {
                _res9 = _res9.Concat(_r9.Results);
                goto label9;
            }
            else
            {
                _memo.Results.Push(new _LizpyParser_Item(_start_i9, _index, _memo.InputEnumerable, _res9.Where(_NON_NULL), true));
            }

            // BIND items
            items = _memo.Results.Peek();

        label3: // AND
            var _r3_2 = _memo.Results.Pop();
            var _r3_1 = _memo.Results.Pop();

            if (_r3_1 != null && _r3_2 != null)
            {
                _memo.Results.Push( new _LizpyParser_Item(_start_i3, _index, _memo.InputEnumerable, _r3_1.Results.Concat(_r3_2.Results).Where(_NON_NULL), true) );
            }
            else
            {
                _memo.Results.Push(null);
                _index = _start_i3;
            }

            // AND shortcut
            if (_memo.Results.Peek() == null) { _memo.Results.Push(null); goto label2; }

            // LITERAL ']'
            _ParseLiteralChar(_memo, ref _index, ']');

        label2: // AND
            var _r2_2 = _memo.Results.Pop();
            var _r2_1 = _memo.Results.Pop();

            if (_r2_1 != null && _r2_2 != null)
            {
                _memo.Results.Push( new _LizpyParser_Item(_start_i2, _index, _memo.InputEnumerable, _r2_1.Results.Concat(_r2_2.Results).Where(_NON_NULL), true) );
            }
            else
            {
                _memo.Results.Push(null);
                _index = _start_i2;
            }

            // BIND a
            a = _memo.Results.Peek();

            // ACT
            var _r0 = _memo.Results.Peek();
            if (_r0 != null)
            {
                _memo.Results.Pop();
                _memo.Results.Push( new _LizpyParser_Item(_r0.StartIndex, _r0.NextIndex, _memo.InputEnumerable, _Thunk(_IM_Result => { return new ArrayExpression(a.StartIndex, a.NextIndex, items.ResultsAs<AtomExpression>()); }, _r0), true) );
            }

        }

        static readonly Verophyle.Regexp.StringRegexp _re0 = new Verophyle.Regexp.StringRegexp(@"[^\n]+");
        static readonly Verophyle.Regexp.StringRegexp _re1 = new Verophyle.Regexp.StringRegexp(@"[0-9]");
        static readonly Verophyle.Regexp.StringRegexp _re2 = new Verophyle.Regexp.StringRegexp(@"[^""\\]");
        static readonly Verophyle.Regexp.StringRegexp _re3 = new Verophyle.Regexp.StringRegexp(@"[A-Za-z\`~!@#$%^&*\-_=+|;:,<\.>\/?][0-9A-Za-z\`~!@#$%^&*\-_=+|;:',<\.>\/?]*");
        static readonly Verophyle.Regexp.StringRegexp _re4 = new Verophyle.Regexp.StringRegexp(@"\s+^");

    } // class LizpyParser

} // namespace Lizpy

