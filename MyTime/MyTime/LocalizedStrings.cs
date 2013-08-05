using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FieldService
{
	public class LocalizedStrings
	{
		public LocalizedStrings()
		{
		}

		private static StringResources localizedResources = new StringResources();

		public StringResources StringResources
		{
			get { return localizedResources; }
		}
	}
}
