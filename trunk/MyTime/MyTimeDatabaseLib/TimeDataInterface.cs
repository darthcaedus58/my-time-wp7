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
        /// <returns>System.Int32.</returns>
        public static int AddTime(TimeData d)
        {
            using (var db = new TimeDataContext(TimeDataContext.DBConnectionString)) {
                var td = new TimeDataItem {
                                              Date = d.Date,
                                              BibleStudies = d.BibleStudies,
                                              Books = d.Books,
                                              Brochures = d.Brochures,
                                              Magazines = d.Magazines,
                                              Minutes = d.Minutes,
                                              Notes = d.Notes,
                                              ReturnVisits = d.ReturnVisits
                                          };
                db.TimeDataItems.InsertOnSubmit(td);
                db.SubmitChanges();
                return td.ItemId;
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
                    var td = new TimeData {
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
                    return td;
                } catch (InvalidOperationException e) {
                    return null;
                }
            }
        }

        /// <summary>
        /// Updates the time.
        /// </summary>
        /// <param name="itemId">The item id.</param>
        /// <param name="td">The td.</param>
        /// <returns>System.Int32.</returns>
        /// <exception cref="MyTimeDatabaseLib.TimeDataItemNotFoundException">Couldn't find the time data with that id.</exception>
        public static int UpdateTime(int itemId, TimeData td)
        {
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
                    return tdi.ItemId;
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
        public static void DeleteTime(int itemId)
        {
            using (var db = new TimeDataContext(TimeDataContext.DBConnectionString)) {
                try {
                    TimeDataItem t = db.TimeDataItems.Single(s => s.ItemId == itemId);

                    db.TimeDataItems.DeleteOnSubmit(t);
                    db.SubmitChanges();
                } catch (InvalidOperationException) {}
            }
        }
    }

    /// <summary>
    /// Class TimeData
    /// </summary>
    public class TimeData
    {
        /// <summary>
        /// Gets the item id.
        /// </summary>
        /// <value>The item id.</value>
        public int ItemId { get; internal set; }

        /// <summary>
        /// Gets or sets the minutes.
        /// </summary>
        /// <value>The minutes.</value>
        public int Minutes { get; set; }

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
    }
}