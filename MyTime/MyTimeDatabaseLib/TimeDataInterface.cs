// ***********************************************************************
// Assembly         : MyTimeDatabaseLib
// Author           : trevo_000
// Created          : 11-03-2012
//
// Last Modified By : trevo_000
// Last Modified On : 11-07-2012
// ***********************************************************************
// <copyright file="TimeDataInterface.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using MyTimeDatabaseLib.Model;

namespace MyTimeDatabaseLib
{
	/// <summary>
	/// Class TimeDataInterface
	/// </summary>
	public class TimeDataInterface
	{
		/// <summary>
		/// Checks the database.
		/// </summary>
		public static void CheckDatabase()
		{
			using (var db = new TimeDataContext(TimeDataContext.DBConnectionString)) {
				if (db.DatabaseExists() == false)
					db.CreateDatabase();
				//else {
				//    db.DeleteDatabase();
				//    db.CreateDatabase();
				//}
			}
		}

		/// <summary>
		/// Adds the time.
		/// </summary>
		/// <param name="d">The d.</param>
		public static bool AddTime(ref TimeData d)
		{
			using (var db = new TimeDataContext(TimeDataContext.DBConnectionString)) {
				var newTd = TimeData.Copy(d);
				db.TimeDataItems.InsertOnSubmit(newTd);
				db.SubmitChanges();
				d.ItemId = newTd.ItemId;
				return newTd.ItemId > 0;
			}
		}

		/// <summary>
		/// Gets the time data item.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns>TimeData.</returns>
		public static TimeData GetTimeDataItem(int id)
		{
			using (var db = new TimeDataContext(TimeDataContext.DBConnectionString)) {
				try {
					TimeDataItem tdi = db.TimeDataItems.Single(s => s.ItemId == id);
					var td = TimeData.Copy(tdi);
					return td;
				} catch (InvalidOperationException e) {
					return null;
				}
			}
		}

		/// <summary>
		/// Updates the time.
		/// </summary>
		/// <param name="td">The td.</param>
		/// <returns>System.Int32.</returns>
		/// <exception cref="MyTimeDatabaseLib.TimeDataItemNotFoundException">Couldn't find the time data with that id.</exception>
		public static bool UpdateTime(ref TimeData td)
		{
			if (td.ItemId < 0) return AddTime(ref td);
			int itemId = td.ItemId;
			using (var db = new TimeDataContext(TimeDataContext.DBConnectionString)) {
				try {
					TimeDataItem tdi = db.TimeDataItems.Single(s => s.ItemId == itemId);

					tdi.BibleStudies = td.BibleStudies;
					tdi.Books = td.Books;
					tdi.Brochures = td.Brochures;
					tdi.Date = td.Date;
					tdi.Magazines = td.Magazines;
					tdi.Minutes = td.Minutes;
					tdi.Notes = td.Notes;
					tdi.ReturnVisits = td.ReturnVisits;

					db.SubmitChanges();
					return true;
				} catch(InvalidOperationException) {
					return AddTime(ref td);
				} catch {
					throw new TimeDataItemNotFoundException("Couldn't find the time data with that id.");
				}
			}
		}

		/// <summary>
		/// Gets the entries.
		/// </summary>
		/// <param name="from">From.</param>
		/// <param name="to">To.</param>
		/// <param name="so">The so.</param>
		/// <returns>TimeData[][].</returns>
		public static TimeData[] GetEntries(DateTime @from, DateTime to, SortOrder so)
		{
			using (var db = new TimeDataContext(TimeDataContext.DBConnectionString)) {
				IOrderedQueryable<TimeDataItem> entries = from x in db.TimeDataItems
														  where x.Date >= @from && x.Date <= to
														  orderby x.Date
														  select x;

				if (entries.Any()) {
					var times = new List<TimeData>();
					IEnumerable<TimeDataItem> e = so == SortOrder.DateNewestToOldest ? entries.ToArray().Reverse() : entries.ToArray();
					foreach (TimeDataItem tdi in e) {
						times.Add(new TimeData {
												   BibleStudies = tdi.BibleStudies,
												   Books = tdi.Books,
												   Brochures = tdi.Brochures,
												   Date = tdi.Date,
												   ItemId = tdi.ItemId,
												   Magazines = tdi.Magazines,
												   Minutes = tdi.Minutes,
												   Notes = tdi.Notes,
												   ReturnVisits = tdi.ReturnVisits
											   });
					}
					return times.ToArray();
				}
				return new TimeData[0];
			}
		}

		/// <summary>
		/// Deletes the time.
		/// </summary>
		/// <param name="itemId">The item id.</param>
		public static bool DeleteTime(int itemId)
		{
			using (var db = new TimeDataContext(TimeDataContext.DBConnectionString)) {
				try {
					TimeDataItem t = db.TimeDataItems.Single(s => s.ItemId == itemId);

					db.TimeDataItems.DeleteOnSubmit(t);
					db.SubmitChanges();
					return true;
				} catch (InvalidOperationException) { return false; }
			}
			return false;
		}

		public static bool AddOrUpdateTime(ref TimeData td)
		{
			if (td.ItemId < 0) return AddTime(ref td);
			return UpdateTime(ref td);
		}

		public static bool IsDoubleDataEntry(DateTime date, out int id)
		{
			//
			using (var db = new TimeDataContext(TimeDataContext.DBConnectionString)) {
				try {
					var q = from x in db.TimeDataItems
							where x.Date.Date == date.Date
							select x;

					if (q.Any()) {
						id = q.First().ItemId;
						return true;
					}
					id = -1;
					return false;
				} catch {
					id = -1;
					return false;
				}
			}
		}
	}

	/// <summary>
	/// Class TimeData
	/// </summary>
	public class TimeData
	{

		public TimeData()
		{
			ItemId = -1;
		}

		/// <summary>
		/// Gets the item id.
		/// </summary>
		/// <value>The item id.</value>
		public int ItemId { get; internal set; }

		/// <summary>
		/// Gets or sets the minutes.
		/// </summary>
		/// <value>The minutes.</value>
		public int Minutes { get { return (int)TotalTime.TotalMinutes; } set { TotalTime = TimeSpan.FromMinutes(value); } }

		public double Hours { get { return TotalTime.TotalHours; } set { TotalTime = TimeSpan.FromHours(value); } }

		public TimeSpan TotalTime { get; set; }

		/// <summary>
		/// Gets or sets the date.
		/// </summary>
		/// <value>The date.</value>
		public DateTime Date { get; set; }

		/// <summary>
		/// Gets or sets the magazines.
		/// </summary>
		/// <value>The magazines.</value>
		public int Magazines { get; set; }

		/// <summary>
		/// Gets or sets the books.
		/// </summary>
		/// <value>The books.</value>
		public int Books { get; set; }

		/// <summary>
		/// Gets or sets the brochures.
		/// </summary>
		/// <value>The brochures.</value>
		public int Brochures { get; set; }

		/// <summary>
		/// Gets or sets the bible studies.
		/// </summary>
		/// <value>The bible studies.</value>
		public int BibleStudies { get; set; }

		/// <summary>
		/// Gets or sets the return visits.
		/// </summary>
		/// <value>The return visits.</value>
		public int ReturnVisits { get; set; }

		/// <summary>
		/// Gets or sets the notes.
		/// </summary>
		/// <value>The notes.</value>
		public string Notes { get; set; }

		internal static TimeDataItem Copy(TimeData d)
		{
			return new TimeDataItem {
				                        Date = d.Date,
				                        BibleStudies = d.BibleStudies,
				                        Books = d.Books,
				                        Brochures = d.Brochures,
				                        Magazines = d.Magazines,
				                        Minutes = d.Minutes,
				                        Notes = d.Notes,
				                        ReturnVisits = d.ReturnVisits
			                        };
		}

		internal static TimeData Copy(TimeDataItem tdi)
		{
			return new TimeData {
				                    BibleStudies = tdi.BibleStudies,
				                    Books = tdi.Books,
				                    Brochures = tdi.Brochures,
				                    Date = tdi.Date,
				                    ItemId = tdi.ItemId,
				                    Magazines = tdi.Magazines,
				                    Minutes = tdi.Minutes,
				                    Notes = tdi.Notes,
				                    ReturnVisits = tdi.ReturnVisits
			                    };

		}
	}
}