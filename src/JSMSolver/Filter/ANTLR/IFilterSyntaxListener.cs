using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMSolver.Filter.ANTLR
{
    public interface IFilterSyntaxListener : IParseTreeListener
    {
        /// <summary>
        /// Enter a parse tree produced by <see cref="FilterSyntaxParser.t1"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        void EnterT1([NotNull] FilterSyntaxParser.T1Context context);
        /// <summary>
        /// Exit a parse tree produced by <see cref="FilterSyntaxParser.t1"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        void ExitT1([NotNull] FilterSyntaxParser.T1Context context);

        /// <summary>
        /// Enter a parse tree produced by <see cref="FilterSyntaxParser.t2"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        void EnterT2([NotNull] FilterSyntaxParser.T2Context context);
        /// <summary>
        /// Exit a parse tree produced by <see cref="FilterSyntaxParser.t2"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        void ExitT2([NotNull] FilterSyntaxParser.T2Context context);

        /// <summary>
        /// Enter a parse tree produced by <see cref="FilterSyntaxParser.t22"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        void EnterT22([NotNull] FilterSyntaxParser.T22Context context);
        /// <summary>
        /// Exit a parse tree produced by <see cref="FilterSyntaxParser.t22"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        void ExitT22([NotNull] FilterSyntaxParser.T22Context context);

        /// <summary>
        /// Enter a parse tree produced by <see cref="FilterSyntaxParser.t3"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        void EnterT3([NotNull] FilterSyntaxParser.T3Context context);
        /// <summary>
        /// Exit a parse tree produced by <see cref="FilterSyntaxParser.t3"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        void ExitT3([NotNull] FilterSyntaxParser.T3Context context);
    }
}
