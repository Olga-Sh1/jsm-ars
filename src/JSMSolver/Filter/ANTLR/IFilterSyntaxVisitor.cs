using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMSolver.Filter.ANTLR
{
    public interface IFilterSyntaxVisitor<Result> : IParseTreeVisitor<Result>
    {
        /// <summary>
        /// Visit a parse tree produced by <see cref="FilterSyntaxParser.t1"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        Result VisitT1([NotNull] FilterSyntaxParser.T1Context context);

        /// <summary>
        /// Visit a parse tree produced by <see cref="FilterSyntaxParser.t2"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        Result VisitT2([NotNull] FilterSyntaxParser.T2Context context);

        /// <summary>
        /// Visit a parse tree produced by <see cref="FilterSyntaxParser.t22"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        Result VisitT22([NotNull] FilterSyntaxParser.T22Context context);

        /// <summary>
        /// Visit a parse tree produced by <see cref="FilterSyntaxParser.t3"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        Result VisitT3([NotNull] FilterSyntaxParser.T3Context context);
    }
}
