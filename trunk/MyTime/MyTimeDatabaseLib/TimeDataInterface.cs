using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using MyTimeDatabaseLib.Model;
using System.Linq;

namespace MyTimeDatabaseLib
{
	public class TimeDataInterface
	{

		public static void CheckDatabase()
		{
			using (var db = new TimeDataContext(TimeDataContext.DBConnectionString)) {
				if (db.DatabaseExists() == false)
					db.CreateDatabase();
			}
		}
	}
}
