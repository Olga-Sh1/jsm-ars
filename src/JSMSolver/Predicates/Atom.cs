using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMSolver.Predicates
{
    public class Atom
    {
        public Union Union { get; private set; }
        public Addings Argument1 { get; private set; }
        public Addings Argumnet2 { get; private set; }
        public Atom(Union un, Addings arg1, Addings arg2)
        {
            Union = un;
            Argument1 = arg1;
            Argumnet2 = arg2;
        }
    }
}
