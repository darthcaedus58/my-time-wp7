using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using MyTimeDatabaseLib.Model;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

namespace MyTimeDatabaseLib
{
	public class RvPreviousVisitsDataInterface
	{
		public static RvPreviousVisitData[] GetPreviousVisits(int rvItemId)
		{
			var rvVists = new List<RvPreviousVisitData>();
			using (var db = new RvPreviousVisitsContext(RvPreviousVisitsContext.DBConnectionString)) {

				var qry = from x in db.RvPreviousVisitItems
						  where x.RvItemId == rvItemId
						  select x;

				if (qry.Any()) {
					foreach (var q in qry) {
						var visit = new RvPreviousVisitData()
						{
							ItemId = q.ItemId,
							RvItemId = q.RvItemId,
							Date = q.Date,
							Notes = q.Notes,
							Placements = q.Placements
						};
						rvVists.Add(visit);
					}
					return rvVists.ToArray();
				}
				return new RvPreviousVisitData[0];
			}
		}

		public static void CheckDatabase()
		{
			using (var db = new RvPreviousVisitsContext(RvPreviousVisitsContext.DBConnectionString)) {
				if (db.DatabaseExists() == false)
					db.CreateDatabase();
			}
		}
	}

	public class RvPreviousVisitData
	{
		public int ItemId {	internal set; get; }

		public int RvItemId	{ get; set;	}

		public string Notes { get; set; }

		public XDocument Placements { get; set; }

		public DateTime Date { get; set; }
	}
}
