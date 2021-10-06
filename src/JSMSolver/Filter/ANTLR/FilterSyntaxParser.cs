using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMSolver.Filter.ANTLR
{
    public partial class FilterSyntaxParser : Parser
    {
        public const int
            T__0 = 1, T__1 = 2, NUM = 3, CONN = 4, WS = 5, NULL = 6;
        public const int
            RULE_t1 = 0, RULE_t2 = 1, RULE_t22 = 2, RULE_t3 = 3;
        public static readonly string[] ruleNames = {
        "t1", "t2", "t22", "t3"
    };

        private static readonly string[] _LiteralNames = {
        null, "'('", "')'"
    };
        private static readonly string[] _SymbolicNames = {
        null, null, null, "NUM", "CONN", "WS", "NULL"
    };
        public static readonly IVocabulary DefaultVocabulary = new Vocabulary(_LiteralNames, _SymbolicNames);

        [System.Obsolete("Use Vocabulary instead.")]
        public static readonly string[] tokenNames = GenerateTokenNames(DefaultVocabulary, _SymbolicNames.Length);

        private static string[] GenerateTokenNames(IVocabulary vocabulary, int length)
        {
            string[] tokenNames = new string[length];
            for (int i = 0; i < tokenNames.Length; i++)
            {
                tokenNames[i] = vocabulary.GetLiteralName(i);
                if (tokenNames[i] == null)
                {
                    tokenNames[i] = vocabulary.GetSymbolicName(i);
                }

                if (tokenNames[i] == null)
                {
                    tokenNames[i] = "<INVALID>";
                }
            }

            return tokenNames;
        }

        [System.Obsolete("Use IRecognizer.Vocabulary instead.")]
        public override string[] TokenNames
        {
            get
            {
                return tokenNames;
            }
        }

        [NotNull]
        public override IVocabulary Vocabulary
        {
            get
            {
                return DefaultVocabulary;
            }
        }

        public override string GrammarFileName { get { return "FilterSyntax.g4"; } }

        public override string[] RuleNames { get { return ruleNames; } }

        public override string SerializedAtn { get { return _serializedATN; } }

        public FilterSyntaxParser(ITokenStream input)
            : base(input)
        {
            _interp = new ParserATNSimulator(this, _ATN);
        }
        public partial class T1Context : ParserRuleContext
        {
            public ITerminalNode[] NUM() { return GetTokens(FilterSyntaxParser.NUM); }
            public ITerminalNode NUM(int i)
            {
                return GetToken(FilterSyntaxParser.NUM, i);
            }
            public ITerminalNode CONN() { return GetToken(FilterSyntaxParser.CONN, 0); }
            public ITerminalNode NULL() { return GetToken(FilterSyntaxParser.NULL, 0); }
            public T1Context(ParserRuleContext parent, int invokingState)
                : base(parent, invokingState)
            {
            }
            public override int RuleIndex { get { return RULE_t1; } }
            public override void EnterRule(IParseTreeListener listener)
            {
                IFilterSyntaxListener typedListener = listener as IFilterSyntaxListener;
                if (typedListener != null) typedListener.EnterT1(this);
            }
            public override void ExitRule(IParseTreeListener listener)
            {
                IFilterSyntaxListener typedListener = listener as IFilterSyntaxListener;
                if (typedListener != null) typedListener.ExitT1(this);
            }
            public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
            {
                IFilterSyntaxVisitor<TResult> typedVisitor = visitor as IFilterSyntaxVisitor<TResult>;
                if (typedVisitor != null) return typedVisitor.VisitT1(this);
                else return visitor.VisitChildren(this);
            }
        }

        [RuleVersion(0)]
        public T1Context t1()
        {
            T1Context _localctx = new T1Context(_ctx, State);
            EnterRule(_localctx, 0, RULE_t1);
            int _la;
            try
            {
                EnterOuterAlt(_localctx, 1);
                {
                    State = 8; Match(NUM);
                    State = 9; Match(CONN);
                    State = 10;
                    _la = _input.La(1);
                    if (!(_la == NUM || _la == NULL))
                    {
                        _errHandler.RecoverInline(this);
                    }
                    else
                    {
                        if (_input.La(1) == TokenConstants.Eof)
                        {
                            matchedEOF = true;
                        }

                        _errHandler.ReportMatch(this);
                        Consume();
                    }
                }
            }
            catch (RecognitionException re)
            {
                _localctx.exception = re;
                _errHandler.ReportError(this, re);
                _errHandler.Recover(this, re);
            }
            finally
            {
                ExitRule();
            }
            return _localctx;
        }

        public partial class T2Context : ParserRuleContext
        {
            public T1Context[] t1()
            {
                return GetRuleContexts<T1Context>();
            }
            public T1Context t1(int i)
            {
                return GetRuleContext<T1Context>(i);
            }
            public T2Context(ParserRuleContext parent, int invokingState)
                : base(parent, invokingState)
            {
            }
            public override int RuleIndex { get { return RULE_t2; } }
            public override void EnterRule(IParseTreeListener listener)
            {
                IFilterSyntaxListener typedListener = listener as IFilterSyntaxListener;
                if (typedListener != null) typedListener.EnterT2(this);
            }
            public override void ExitRule(IParseTreeListener listener)
            {
                IFilterSyntaxListener typedListener = listener as IFilterSyntaxListener;
                if (typedListener != null) typedListener.ExitT2(this);
            }
            public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
            {
                IFilterSyntaxVisitor<TResult> typedVisitor = visitor as IFilterSyntaxVisitor<TResult>;
                if (typedVisitor != null) return typedVisitor.VisitT2(this);
                else return visitor.VisitChildren(this);
            }
        }

        [RuleVersion(0)]
        public T2Context t2()
        {
            T2Context _localctx = new T2Context(_ctx, State);
            EnterRule(_localctx, 2, RULE_t2);
            int _la;
            try
            {
                EnterOuterAlt(_localctx, 1);
                {
                    State = 12; t1();
                    State = 16;
                    _errHandler.Sync(this);
                    _la = _input.La(1);
                    while (_la == NUM)
                    {
                        {
                            {
                                State = 13; t1();
                            }
                        }
                        State = 18;
                        _errHandler.Sync(this);
                        _la = _input.La(1);
                    }
                }
            }
            catch (RecognitionException re)
            {
                _localctx.exception = re;
                _errHandler.ReportError(this, re);
                _errHandler.Recover(this, re);
            }
            finally
            {
                ExitRule();
            }
            return _localctx;
        }

        public partial class T22Context : ParserRuleContext
        {
            public T2Context t2()
            {
                return GetRuleContext<T2Context>(0);
            }
            public T22Context(ParserRuleContext parent, int invokingState)
                : base(parent, invokingState)
            {
            }
            public override int RuleIndex { get { return RULE_t22; } }
            public override void EnterRule(IParseTreeListener listener)
            {
                IFilterSyntaxListener typedListener = listener as IFilterSyntaxListener;
                if (typedListener != null) typedListener.EnterT22(this);
            }
            public override void ExitRule(IParseTreeListener listener)
            {
                IFilterSyntaxListener typedListener = listener as IFilterSyntaxListener;
                if (typedListener != null) typedListener.ExitT22(this);
            }
            public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
            {
                IFilterSyntaxVisitor<TResult> typedVisitor = visitor as IFilterSyntaxVisitor<TResult>;
                if (typedVisitor != null) return typedVisitor.VisitT22(this);
                else return visitor.VisitChildren(this);
            }
        }

        [RuleVersion(0)]
        public T22Context t22()
        {
            T22Context _localctx = new T22Context(_ctx, State);
            EnterRule(_localctx, 4, RULE_t22);
            try
            {
                EnterOuterAlt(_localctx, 1);
                {
                    State = 19; Match(T__0);
                    State = 20; t2();
                    State = 21; Match(T__1);
                }
            }
            catch (RecognitionException re)
            {
                _localctx.exception = re;
                _errHandler.ReportError(this, re);
                _errHandler.Recover(this, re);
            }
            finally
            {
                ExitRule();
            }
            return _localctx;
        }

        public partial class T3Context : ParserRuleContext
        {
            public ITerminalNode Eof() { return GetToken(FilterSyntaxParser.Eof, 0); }
            public T2Context t2()
            {
                return GetRuleContext<T2Context>(0);
            }
            public T22Context[] t22()
            {
                return GetRuleContexts<T22Context>();
            }
            public T22Context t22(int i)
            {
                return GetRuleContext<T22Context>(i);
            }
            public T3Context(ParserRuleContext parent, int invokingState)
                : base(parent, invokingState)
            {
            }
            public override int RuleIndex { get { return RULE_t3; } }
            public override void EnterRule(IParseTreeListener listener)
            {
                IFilterSyntaxListener typedListener = listener as IFilterSyntaxListener;
                if (typedListener != null) typedListener.EnterT3(this);
            }
            public override void ExitRule(IParseTreeListener listener)
            {
                IFilterSyntaxListener typedListener = listener as IFilterSyntaxListener;
                if (typedListener != null) typedListener.ExitT3(this);
            }
            public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor)
            {
                IFilterSyntaxVisitor<TResult> typedVisitor = visitor as IFilterSyntaxVisitor<TResult>;
                if (typedVisitor != null) return typedVisitor.VisitT3(this);
                else return visitor.VisitChildren(this);
            }
        }

        [RuleVersion(0)]
        public T3Context t3()
        {
            T3Context _localctx = new T3Context(_ctx, State);
            EnterRule(_localctx, 6, RULE_t3);
            int _la;
            try
            {
                EnterOuterAlt(_localctx, 1);
                {
                    State = 29;
                    _errHandler.Sync(this);
                    switch (_input.La(1))
                    {
                        case NUM:
                            {
                                State = 23; t2();
                            }
                            break;
                        case T__0:
                            {
                                State = 25;
                                _errHandler.Sync(this);
                                _la = _input.La(1);
                                do
                                {
                                    {
                                        {
                                            State = 24; t22();
                                        }
                                    }
                                    State = 27;
                                    _errHandler.Sync(this);
                                    _la = _input.La(1);
                                } while (_la == T__0);
                            }
                            break;
                        default:
                            throw new NoViableAltException(this);
                    }
                    State = 31; Match(Eof);
                }
            }
            catch (RecognitionException re)
            {
                _localctx.exception = re;
                _errHandler.ReportError(this, re);
                _errHandler.Recover(this, re);
            }
            finally
            {
                ExitRule();
            }
            return _localctx;
        }

        public static readonly string _serializedATN =
            "\x3\xAF6F\x8320\x479D\xB75C\x4880\x1605\x191C\xAB37\x3\b$\x4\x2\t\x2\x4" +
            "\x3\t\x3\x4\x4\t\x4\x4\x5\t\x5\x3\x2\x3\x2\x3\x2\x3\x2\x3\x3\x3\x3\a\x3" +
            "\x11\n\x3\f\x3\xE\x3\x14\v\x3\x3\x4\x3\x4\x3\x4\x3\x4\x3\x5\x3\x5\x6\x5" +
            "\x1C\n\x5\r\x5\xE\x5\x1D\x5\x5 \n\x5\x3\x5\x3\x5\x3\x5\x2\x2\x2\x6\x2" +
            "\x2\x4\x2\x6\x2\b\x2\x2\x3\x4\x2\x5\x5\b\b\"\x2\n\x3\x2\x2\x2\x4\xE\x3" +
            "\x2\x2\x2\x6\x15\x3\x2\x2\x2\b\x1F\x3\x2\x2\x2\n\v\a\x5\x2\x2\v\f\a\x6" +
            "\x2\x2\f\r\t\x2\x2\x2\r\x3\x3\x2\x2\x2\xE\x12\x5\x2\x2\x2\xF\x11\x5\x2" +
            "\x2\x2\x10\xF\x3\x2\x2\x2\x11\x14\x3\x2\x2\x2\x12\x10\x3\x2\x2\x2\x12" +
            "\x13\x3\x2\x2\x2\x13\x5\x3\x2\x2\x2\x14\x12\x3\x2\x2\x2\x15\x16\a\x3\x2" +
            "\x2\x16\x17\x5\x4\x3\x2\x17\x18\a\x4\x2\x2\x18\a\x3\x2\x2\x2\x19 \x5\x4" +
            "\x3\x2\x1A\x1C\x5\x6\x4\x2\x1B\x1A\x3\x2\x2\x2\x1C\x1D\x3\x2\x2\x2\x1D" +
            "\x1B\x3\x2\x2\x2\x1D\x1E\x3\x2\x2\x2\x1E \x3\x2\x2\x2\x1F\x19\x3\x2\x2" +
            "\x2\x1F\x1B\x3\x2\x2\x2 !\x3\x2\x2\x2!\"\a\x2\x2\x3\"\t\x3\x2\x2\x2\x5" +
            "\x12\x1D\x1F";
        public static readonly ATN _ATN =
            new ATNDeserializer().Deserialize(_serializedATN.ToCharArray());
    }
}
