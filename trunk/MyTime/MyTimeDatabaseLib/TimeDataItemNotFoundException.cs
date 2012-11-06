using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyTimeDatabaseLib
{
    public class TimeDataItemNotFoundException : Exception
    {
        public TimeDataItemNotFoundException(string message) : base(message) { }
    }
}
