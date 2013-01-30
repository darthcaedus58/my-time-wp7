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

		/// <summary>
		/// Gets the RBC Time Data for a given itemID.
		/// </summary>
		/// <param name="itemID">The item ID to look up.</param>
		/// <returns>The time data for the given item id.</returns>
		public static RBCTimeData GetRBCTimeData(int itemID)
		{
			//
			if (itemID < 0) return null;
			using (var db = new RBCTimeDataContext()) {
				try {
					var rtd = db.RBCTimeDataItems.Single(s => s.ItemId == itemID);
					if (rtd == null) return null;
					return RBCTimeData.Copy(rtd);
				} catch {
					return null;
				}
			}
		}

		/// <summary>
		/// Adds or Updates the RBC Time.
		/// </summary>
		/// <param name="td">The Time Data to Add/Update. If the ItemId is -1 it will automatically add it.</param>
		/// <returns>[true] if successful, otherwise [false].</returns>
		public static bool AddOrUpdateTime(ref RBCTimeData td)
		{
			if (td.ItemID < 0) return AddTime(ref td);
			using (var db = new RBCTimeDataContext()) {
				try {
					int i = td.ItemID;
					var tdUpdate = db.RBCTimeDataItems.Single(s => s.ItemId == i);
					if (tdUpdate == null) return AddTime(ref td);

					tdUpdate.Minutes = td.Minutes;
					tdUpdate.Notes = td.Notes;
					tdUpdate.Date = td.Date;

					db.SubmitChanges();
					return true;

				} catch (InvalidOperationException) {
					return AddTime(ref td);
				} catch {
					return false;
				}
			}
		}

		private static bool AddTime(ref RBCTimeData td)
		{
			//throw new NotImplementedException();
			using (var db = new RBCTimeDataContext()) {
				var newTd = Copy(td);
				db.RBCTimeDataItems.InsertOnSubmit(newTd);
				db.SubmitChanges();
				td.ItemID = newTd.ItemId;
				return td.ItemID >= 0;
			}
		}

		private static RBCTimeDataItem Copy(RBCTimeData td)
		{
			return new RBCTimeDataItem {
				                           /*ItemId = td.ItemID,*/
				                           Date = td.Date,
				                           Minutes = td.Minutes,
				                           Notes = td.Notes
			                           };
		}

		public static bool DeleteTime(RBCTimeData td) 
		{ 
			//
			if (td.ItemID < 0) return false;
			using (var db = new RBCTimeDataContext()) {
				try {
					var rtd = db.RBCTimeDataItems.Single(s => s.ItemId == td.ItemID);
					if (rtd == null) return false;
					db.RBCTimeDataItems.DeleteOnSubmit(rtd);
					db.SubmitChanges();
					return true;
				} catch { return false; }
			}
		}

		public static int GetMonthRBCTimeTotal(DateTime dt)
		{
			//throw new NotImplementedException();
			using (var db = new RBCTimeDataContext()) {
				try {
					var rtd = from x in db.RBCTimeDataItems
							  where x.Date >= dt && x.Date <= dt.AddMonths(1).AddDays(-1)
							  select x;
					return !rtd.Any() ? 0 : Enumerable.Sum(rtd, t => t.Minutes);
				} catch {
					return 0;
				}
			}
		}

		public static RBCTimeData[] GetRBCTimeEntries(DateTime fromDate, DateTime toDate) { return GetRBCTimeEntries(fromDate, toDate, SortOrder.DateNewestToOldest); }

		public static RBCTimeData[] GetRBCTimeEntries(DateTime fromDate, DateTime toDate, SortOrder sortOrder)
		{
			//throw new NotImplementedException();
			using (var db = new RBCTimeDataContext()) {
				try {
					var rtd = from x in db.RBCTimeDataItems
							  where x.Date >= fromDate && x.Date <= toDate
							  orderby x.Date
							  select x;

					return !rtd.Any() ? null : rtd.Select(i => RBCTimeData.Copy(i)).ToArray();
				} catch { return null; }
			}
		}
	}

	public class RBCTimeData
	{
		public int ItemID { get; internal set; }

		public DateTime Date { get; set; }

		public int Minutes { get; set; }

		public float Hours { get { return ((float)Minutes / (float)60.0); } set { Minutes = Convert.ToInt32(value*60.0); } }

		public string Notes { get; set; }

		internal static RBCTimeData Copy(RBCTimeDataItem rtd)
		{
			if (rtd == null) return null;
			return new RBCTimeData {
				                       Date = rtd.Date,
				                       ItemID = rtd.ItemId,
				                       Minutes = rtd.Minutes,
				                       Notes = rtd.Notes
			                       };
		}

		public RBCTimeData() { ItemID = -1; }
	}
}
