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
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using MyTimeDatabaseLib.Model;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

namespace MyTimeDatabaseLib
{
    /// <summary>
    /// Class RvPreviousVisitsDataInterface
    /// </summary>
    public class RvPreviousVisitsDataInterface
    {
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

                var qry = from x in db.RvPreviousVisitItems
                          where x.RvItemId == rvItemId
                          orderby x.Date
                          select x;

                if (qry.Any()) {
                    var visits = so == SortOrder.DateNewestToOldest ? qry.ToArray().Reverse() : qry.ToArray();
                    foreach (var v in visits) {
                        var visit = new RvPreviousVisitData()
                        {
                            ItemId = v.ItemId,
                            RvItemId = v.RvItemId,
                            Date = v.Date,
                            Notes = v.Notes,
                            Magazines = v.Magazines,
                            Books = v.Books,
                            Brochures = v.Brochures
                        };
                        rvVists.Add(visit);
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
                if (db.DatabaseExists() == false)
                    db.CreateDatabase();
            }
        }

        /// <summary>
        /// Saves the call.
        /// </summary>
        /// <param name="call">The call.</param>
        /// <param name="overwrite">if set to <c>true</c> [overwrite].</param>
        /// <returns>The created or updated call itemId.</returns>
        /// <exception cref="System.ArgumentNullException">rvItemId;Rv Item Id can't be null</exception>
        public static int SaveCall(RvPreviousVisitData call, bool overwrite)
        {
            if (call.RvItemId < 0) throw new ArgumentNullException("rvItemId", "Rv Item Id can't be null");

            using (var db = new RvPreviousVisitsContext(RvPreviousVisitsContext.DBConnectionString)) {
                if (db.RvPreviousVisitItems.Any(s => s.ItemId == call.ItemId) && overwrite) {
                    var c = db.RvPreviousVisitItems.Single(s => s.ItemId == call.ItemId);
                    
                    c.RvItemId = call.RvItemId;
                    c.Magazines = call.Magazines;
                    c.Books = call.Books;
                    c.Brochures = call.Brochures;
                    c.Date = call.Date;
                    c.Notes = call.Notes;

                    db.SubmitChanges();
                    return c.ItemId; // existing call saved.
                } 

                var cc = new RvPreviousVisitItem()
                {
                    RvItemId = call.RvItemId,
                    Magazines = call.Magazines,
                    Books = call.Books,
                    Brochures = call.Brochures,
                    Date = call.Date,
                    Notes = call.Notes
                };

                db.RvPreviousVisitItems.InsertOnSubmit(cc);
                db.SubmitChanges();
                return cc.ItemId;
            }
        }

        /// <summary>
        /// Gets the call.
        /// </summary>
        /// <param name="_callId">The _call id.</param>
        /// <returns>RvPreviousVisitData.</returns>
        /// <exception cref="RvPreviousVisitNotFoundException">The Call was not found.</exception>
        public static RvPreviousVisitData GetCall(int _callId)
        {
            using (var db = new RvPreviousVisitsContext(RvPreviousVisitsContext.DBConnectionString)) {
                try {
                    var call = db.RvPreviousVisitItems.Single(s => s.ItemId == _callId);

                    var c = new RvPreviousVisitData()
                    {
                        ItemId = call.ItemId,
                        RvItemId = call.RvItemId,
                        Books = call.Books,
                        Brochures = call.Brochures,
                        Date = call.Date,
                        Magazines = call.Magazines,
                        Notes = call.Notes
                    };
                    return c;
                } catch {
                    throw new RvPreviousVisitNotFoundException("The Call was not found.");
                }
            }
        }

        /// <summary>
        /// Deletes the call.
        /// </summary>
        /// <param name="callId">The call id.</param>
        public static void DeleteCall(int callId)
        {
            using (var db = new RvPreviousVisitsContext(RvPreviousVisitsContext.DBConnectionString)) {
                try {
                    var call = db.RvPreviousVisitItems.Single(s => s.ItemId == callId);

                    db.RvPreviousVisitItems.DeleteOnSubmit(call);
                    db.SubmitChanges();
                    return;
                } catch { }
                //TODO: Error Handling
            }
        }

        public static void DeleteAllCallsFromRv(int itemId)
        {
            using (var db = new RvPreviousVisitsContext(RvPreviousVisitsContext.DBConnectionString)) {
                try {
                    var calls = from x in db.RvPreviousVisitItems
                                where x.RvItemId == itemId
                                select x;

                    db.RvPreviousVisitItems.DeleteAllOnSubmit(calls);
                    db.SubmitChanges();
                } catch {}
            }
        }
    }

    /// <summary>
    /// Class RvPreviousVisitData
    /// </summary>
    public class RvPreviousVisitData
    {
        /// <summary>
        /// Gets or sets the item id.
        /// </summary>
        /// <value>The item id.</value>
        public int ItemId {	internal set; get; }

        /// <summary>
        /// Gets or sets the rv item id.
        /// </summary>
        /// <value>The rv item id.</value>
        public int RvItemId	{ get; set;	}

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
    }
}
