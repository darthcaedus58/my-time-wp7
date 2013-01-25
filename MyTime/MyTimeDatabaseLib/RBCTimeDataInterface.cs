using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyTimeDatabaseLib.Model;

namespace MyTimeDatabaseLib
{
	public class RBCTimeDataInterface
	{
		/// <summary>
		/// Checks the database.
		/// </summary>
		public static void CheckDatabase()
		{
			using (var db = new RBCTimeDataContext()) {
				if (db.DatabaseExists() == false)
					db.CreateDatabase();
				//else {
				//    db.DeleteDatabase();
				//    db.CreateDatabase();
				//}
			}
		}


	}

	public class RBCTimeData
	{
		public int ItemID { get; internal set; }

		public int Minutes { get; set; }

		public float Hours { get { return ((float)Minutes / (float)60.0); } set { Minutes = Convert.ToInt32(value*60.0); } }

		public string Notes { get; set; }
	}
}
