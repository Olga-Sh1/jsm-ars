using System;
using System.Collections.Generic;
using System.Text;

namespace JSMSolver.Research
{
    public sealed class Stratagy
    {
        public Stratagy(Addings pl, Addings mn)
        {
            Plus = pl;
            Minus = mn;
        }
        public Addings Plus { get; private set; }
        public Addings Minus { get; private set; }

    }
}
