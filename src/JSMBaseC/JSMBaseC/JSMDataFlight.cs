using JSMBase.Flights;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase
{
    public sealed class JSMDataFlight : JSMDataBase<Flight>
    {
        public JSMDataFlight(Flight fl) : base(fl)
        {
            this.ID = fl.ID;
        }
        public override bool IsEmpty
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override int CountNonEmptyProps => throw new NotImplementedException();

        public override JSMDataBase<Flight> Intersect(JSMDataBase<Flight> other, string[] groups)
        {
            throw new NotImplementedException();
        }

        public override JSMDataBase<Flight> Difference(JSMDataBase<Flight> other)
        {
            throw new NotImplementedException();
        }

        public override JSMDataBase<Flight> Intersect(JSMDataBase<Flight> other)
        {
            throw new NotImplementedException();
        }

        public override bool IsEnclosed(JSMDataBase<Flight> other)
        {
            throw new NotImplementedException();
        }

        public override bool IsEqual(JSMDataBase<Flight> other)
        {
            throw new NotImplementedException();
        }

        public override bool IsJSMEqual(JSMDataBase<Flight> other)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return this.ID;
        }

        public override JSMDataBase<Flight> Sum(JSMDataBase<Flight> other)
        {
            throw new NotImplementedException();
        }
    }
}
