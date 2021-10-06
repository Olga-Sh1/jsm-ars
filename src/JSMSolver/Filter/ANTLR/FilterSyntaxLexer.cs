﻿using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMSolver.Filter.ANTLR
{
    public partial class FilterSyntaxLexer : Lexer
    {
        public const int
            T__0 = 1, T__1 = 2, NUM = 3, CONN = 4, WS = 5;
        public static string[] modeNames = {
        "DEFAULT_MODE"
    };

        public static readonly string[] ruleNames = {
        "T__0", "T__1", "LN", "LU", "LL", "NULL", "NUM", "CONN", "WS"
    };


        public FilterSyntaxLexer(ICharStream input)
            : base(input)
        {
            _interp = new LexerATNSimulator(this, _ATN);
        }

        private static readonly string[] _LiteralNames = {
        null, "'('", "')'"
    };
        private static readonly string[] _SymbolicNames = {
        null, null, null, "NUM", "CONN", "WS"
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

        public override string[] ModeNames { get { return modeNames; } }

        public override string SerializedAtn { get { return _serializedATN; } }

        public static readonly string _serializedATN =
            "\x3\xAF6F\x8320\x479D\xB75C\x4880\x1605\x191C\xAB37\x2\a\x31\b\x1\x4\x2" +
            "\t\x2\x4\x3\t\x3\x4\x4\t\x4\x4\x5\t\x5\x4\x6\t\x6\x4\a\t\a\x4\b\t\b\x4" +
            "\t\t\t\x4\n\t\n\x3\x2\x3\x2\x3\x3\x3\x3\x3\x4\x3\x4\x3\x5\x3\x5\x3\x6" +
            "\x3\x6\x3\a\x3\a\x3\a\x3\a\x3\a\x3\b\x3\b\a\b\'\n\b\f\b\xE\b*\v\b\x3\t" +
            "\x3\t\x3\n\x3\n\x3\n\x3\n\x2\x2\x2\v\x3\x2\x3\x5\x2\x4\a\x2\x2\t\x2\x2" +
            "\v\x2\x2\r\x2\x2\xF\x2\x5\x11\x2\x6\x13\x2\a\x3\x2\t\x4\x2PPpp\x4\x2W" +
            "Www\x4\x2NNnn\x3\x2\x33;\x3\x2\x32;\x4\x2%%??\x4\x2\"\"..-\x2\x3\x3\x2" +
            "\x2\x2\x2\x5\x3\x2\x2\x2\x2\xF\x3\x2\x2\x2\x2\x11\x3\x2\x2\x2\x2\x13\x3" +
            "\x2\x2\x2\x3\x15\x3\x2\x2\x2\x5\x17\x3\x2\x2\x2\a\x19\x3\x2\x2\x2\t\x1B" +
            "\x3\x2\x2\x2\v\x1D\x3\x2\x2\x2\r\x1F\x3\x2\x2\x2\xF$\x3\x2\x2\x2\x11+" +
            "\x3\x2\x2\x2\x13-\x3\x2\x2\x2\x15\x16\a*\x2\x2\x16\x4\x3\x2\x2\x2\x17" +
            "\x18\a+\x2\x2\x18\x6\x3\x2\x2\x2\x19\x1A\t\x2\x2\x2\x1A\b\x3\x2\x2\x2" +
            "\x1B\x1C\t\x3\x2\x2\x1C\n\x3\x2\x2\x2\x1D\x1E\t\x4\x2\x2\x1E\f\x3\x2\x2" +
            "\x2\x1F \x5\a\x4\x2 !\x5\t\x5\x2!\"\x5\v\x6\x2\"#\x5\v\x6\x2#\xE\x3\x2" +
            "\x2\x2$(\t\x5\x2\x2%\'\t\x6\x2\x2&%\x3\x2\x2\x2\'*\x3\x2\x2\x2(&\x3\x2" +
            "\x2\x2()\x3\x2\x2\x2)\x10\x3\x2\x2\x2*(\x3\x2\x2\x2+,\t\a\x2\x2,\x12\x3" +
            "\x2\x2\x2-.\t\b\x2\x2./\x3\x2\x2\x2/\x30\b\n\x2\x2\x30\x14\x3\x2\x2\x2" +
            "\x4\x2(\x3\x2\x3\x2";
        public static readonly ATN _ATN =
            new ATNDeserializer().Deserialize(_serializedATN.ToCharArray());
    }
}
