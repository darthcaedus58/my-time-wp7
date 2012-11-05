using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyTimeDatabaseLib
{
    public class RvPreviousVisitNotFoundException : Exception
    {
        public RvPreviousVisitNotFoundException(string message) : base(message) { }
    }
}
