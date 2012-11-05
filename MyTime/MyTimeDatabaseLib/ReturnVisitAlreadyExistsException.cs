using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyTimeDatabaseLib
{
	public class ReturnVisitAlreadyExistsException : Exception
	{
		public int ItemId
		{
			get;
			private set;
		}

		public ReturnVisitAlreadyExistsException(string message, int Id) : base(message) { ItemId = Id; }
	}
}
