// ***********************************************************************
// Assembly         : MyTimeDatabaseLib
// Author           : trevo_000
// Created          : 11-03-2012
//
// Last Modified By : trevo_000
// Last Modified On : 11-05-2012
// ***********************************************************************
// <copyright file="RvPreviousVisitsDataInterface.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Phone.Data.Linq;
using MyTimeDatabaseLib.Model;

namespace MyTimeDatabaseLib
{
	/// <summary>
	/// Class RvPreviousVisitsDataInterface
	/// </summary>
	public class RvPreviousVisitsDataInterface
	{
                public const int APP_VERSION = 2;

		/// <summary>
		/// Gets the previous visits.
		/// </summary>
		/// <param name="rvItemId">The rv item id.</param>
		/// <param name="so">The Sort Order.</param>
		/// <returns>RvPreviousVisitData[].</returns>
		public static RvPreviousVisitData[] GetPreviousVisits(int rvItemId, SortOrder so)
		{
			var rvVists = new List<RvPreviousVisitData>();
			using (var db = new RvPreviousVisitsContext(RvPreviousVisitsContext.DBConnectionString)) {
				IOrderedQueryable<RvPreviousVisitItem> qry = from x in db.RvPreviousVisitItems
															 where x.RvItemId == rvItemId
															 orderby x.Date
															 select x;

				if (qry.Any()) {
					IEnumerable<RvPreviousVisitItem> visits = so == SortOrder.DateNewestToOldest ? qry.ToArray().Reverse() : qry.ToArray();
					foreach (RvPreviousVisitItem v in visits) {
						rvVists.Add(RvPreviousVisitData.Copy(v));
					}
					return rvVists.ToArray();
				}
				return new RvPreviousVisitData[0];
			}
		}

		/// <summary>
		/// Checks the database and creates it if it doesnt exist.
		/// </summary>
		public static void CheckDatabase()
		{
			using (var db = new RvPreviousVisitsContext(RvPreviousVisitsContext.DBConnectionString)) {
                                if (db.DatabaseExists() == false) {
                                        db.CreateDatabase();
                                        DatabaseSchemaUpdater dbUpdater = db.CreateDatabaseSchemaUpdater();
                                        dbUpdater.DatabaseSchemaVersion = APP_VERSION;
                                        dbUpdater.Execute();
                                } else {
                                        var dbUpdater = db.CreateDatabaseSchemaUpdater();
                                        if (dbUpdater.DatabaseSchemaVersion < 2)  //update from 1.0 to 2.0 db version
		                        {
                                                dbUpdater.AddColumn<RvPreviousVisitItem>("Tracts");
                                                dbUpdater.DatabaseSchemaVersion = APP_VERSION;
                                                dbUpdater.Execute();
                                        }
                                }


			}
		}

		/// <summary>
		/// Saves the call.
		/// </summary>
		/// <param name="call">The call.</param>
		/// <exception cref="System.ArgumentNullException">rvItemId;Rv Item Id can't be null</exception>
		public static bool AddOrUpdateCall(ref RvPreviousVisitData call)
		{
			if (call.RvItemId < 0) throw new ArgumentNullException("call", "Rv Item Id can't be null");
			int itemId = call.ItemId;
			using (var db = new RvPreviousVisitsContext(RvPreviousVisitsContext.DBConnectionString)) {
				if (itemId > 0 && db.RvPreviousVisitItems.Any(s => s.ItemId == itemId)) {
					RvPreviousVisitItem c = db.RvPreviousVisitItems.Single(s => s.ItemId == itemId);

					c.RvItemId = call.RvItemId;
					c.Magazines = call.Magazines;
					c.Books = call.Books;
					c.Brochures = call.Brochures;
					c.Date = call.Date;
					c.Notes = call.Notes;
				        c.Tracts = call.Tracts;

					db.SubmitChanges();
					return c.ItemId > 0 && ReturnVisitsInterface.UpdateLastVisitDate(call.RvItemId, call.Date); // existing call saved.
				}

				var cc = RvPreviousVisitData.Copy(call);


				db.RvPreviousVisitItems.InsertOnSubmit(cc);
				db.SubmitChanges();
				call.ItemId = cc.ItemId;
                return call.ItemId >= 0 && ReturnVisitsInterface.UpdateLastVisitDate(call.RvItemId, call.Date);
			}
		}

		/// <summary>
		/// Gets the call.
		/// </summary>
		/// <param name="callId">The call id.</param>
		/// <returns>RvPreviousVisitData.</returns>
		/// <exception cref="MyTimeDatabaseLib.RvPreviousVisitNotFoundException">The Call was not found.</exception>
		/// <exception cref="RvPreviousVisitNotFoundException">The Call was not found.</exception>
		public static RvPreviousVisitData GetCall(int callId)
		{
			using (var db = new RvPreviousVisitsContext(RvPreviousVisitsContext.DBConnectionString)) {
				try {
					RvPreviousVisitItem call = db.RvPreviousVisitItems.Single(s => s.ItemId == callId);

					return RvPreviousVisitData.Copy(call);
				} catch {
					throw new RvPreviousVisitNotFoundException("The Call was not found.");
				}
			}
		}

		/// <summary>
		/// Deletes the call.
		/// </summary>
		/// <param name="callId">The call id.</param>
		public static bool DeleteCall(int callId)
		{
			using (var db = new RvPreviousVisitsContext(RvPreviousVisitsContext.DBConnectionString)) {
				try {
					RvPreviousVisitItem call = db.RvPreviousVisitItems.Single(s => s.ItemId == callId);

					db.RvPreviousVisitItems.DeleteOnSubmit(call);
					db.SubmitChanges();
					return ReturnVisitsInterface.DeleteCallFromRv(call.RvItemId, call.Date);
				} catch { return false; }
			}
		}

		/// <summary>
		/// Deletes all calls from rv.
		/// </summary>
		/// <param name="itemId">The item id.</param>
		public static bool DeleteAllCallsFromRv(int itemId)
		{
			using (var db = new RvPreviousVisitsContext(RvPreviousVisitsContext.DBConnectionString)) {
				try {
					var calls = from x in db.RvPreviousVisitItems
					            where x.RvItemId == itemId
					            select x;

					db.RvPreviousVisitItems.DeleteAllOnSubmit(calls);
					db.SubmitChanges();
					return true;
				} catch (Exception) { return false; } 
			}
		}

		public static RvPreviousVisitData[] GetCallsByDate(DateTime @from, DateTime tod)
		{
			//
			using (var db = new RvPreviousVisitsContext(RvPreviousVisitsContext.DBConnectionString)) {
				try { //new DateTime(tod.Year, tod.Month, tod.Day, 0,0,0).AddDays(1)
                    @from  = new DateTime(@from.Year,@from.Month, @from.Day,0,0,0);
				    tod = new DateTime(tod.Year, tod.Month, tod.Day, 0, 0, 0).AddDays(1);
					var calls = from x in db.RvPreviousVisitItems
                                where x.Date >= @from && x.Date < tod
								select x;
					if (calls.Any()) {
						return calls.Select(c => RvPreviousVisitData.Copy(c)).ToArray();
					}
				} catch {
					return null;
				}
			}
			return null;
		}

		public static bool IsInitialCall(RvPreviousVisitData call)
		{
			//
			using (var db = new RvPreviousVisitsContext(RvPreviousVisitsContext.DBConnectionString)) {
				try {
					var qry = from x in db.RvPreviousVisitItems
							  where x.RvItemId == call.RvItemId
							  orderby x.Date
							  select x;
					if (qry.Any()) {
						if (qry.Count() <= 1) return true;
						var c = qry.ToArray().First();
						if (c.ItemId == call.ItemId) return true;
					}
					return false;
				} catch (Exception) {
					return false;
				}
			} 
		}
	}

	/// <summary>
	/// Class RvPreviousVisitData
	/// </summary>
	public class RvPreviousVisitData
	{
		public RvPreviousVisitData() { ItemId = -1; }
		/// <summary>
		/// Gets or sets the item id.
		/// </summary>
		/// <value>The item id.</value>
		public int ItemId { internal set; get; }

		/// <summary>
		/// Gets or sets the rv item id.
		/// </summary>
		/// <value>The rv item id.</value>
		public int RvItemId { get; set; }

		/// <summary>
		/// Gets or sets the notes.
		/// </summary>
		/// <value>The notes.</value>
		public string Notes { get; set; }

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
		/// Gets or sets the date.
		/// </summary>
		/// <value>The date.</value>
		public DateTime Date { get; set; }

	        public int Tracts { get; set; }

	        internal static RvPreviousVisitItem Copy(RvPreviousVisitData call)
		{
			return new RvPreviousVisitItem {
											   RvItemId = call.RvItemId,
											   Magazines = call.Magazines,
											   Books = call.Books,
											   Brochures = call.Brochures,
                                                                                           Tracts = call.Tracts,
											   Date = call.Date,
											   Notes = call.Notes
										   };
		}

		internal static RvPreviousVisitData Copy(RvPreviousVisitItem call)
		{
			return new RvPreviousVisitData {
				                               ItemId = call.ItemId,
				                               RvItemId = call.RvItemId,
				                               Books = call.Books,
				                               Brochures = call.Brochures,
                                                               Tracts = call.Tracts ?? 0,
				                               Date = call.Date,
				                               Magazines = call.Magazines,
				                               Notes = call.Notes
			                               };
		}
	}
}