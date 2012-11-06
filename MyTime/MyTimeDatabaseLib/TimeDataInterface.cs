using System;
using System.Collections.Generic;
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
                //else {
                //    db.DeleteDatabase();
                //    db.CreateDatabase();
                //}
            }
        }

        public static int AddTime(TimeData d)
        {
            using (var db = new TimeDataContext(TimeDataContext.DBConnectionString)) {
                try {
                    var td = new TimeDataItem()
                    {
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
                } catch (Exception e) {
                    throw e;
                }
            }
        }

        public static TimeData GetTimeDataItem(int id)
        {
            using (var db = new TimeDataContext(TimeDataContext.DBConnectionString)) {
                try {
                    var tdi = db.TimeDataItems.Single(s => s.ItemId == id);
                    var td = new TimeData()
                    {
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
                } catch (InvalidOperationException) {
                    return null;
                }
            }

        }

        public static void UpdateTime(int itemId, TimeData td)
        {
            using (var db = new TimeDataContext(TimeDataContext.DBConnectionString)) {
                try {
                    var tdi = db.TimeDataItems.Single(s => s.ItemId == itemId);
                    if (tdi != null) {
                        tdi.BibleStudies = td.BibleStudies;
                        tdi.Books = td.Books;
                        tdi.Brochures = td.Brochures;
                        tdi.Date = td.Date;
                        tdi.Magazines = td.Magazines;
                        tdi.Minutes = td.Minutes;
                        tdi.Notes = td.Notes;
                        tdi.ReturnVisits = td.ReturnVisits;

                        db.SubmitChanges();
                    }
                } catch {
                    throw new TimeDataItemNotFoundException("Couldn't find the time data with that id.");
                }
            }
            

        }

        public static TimeData[] GetEntries(DateTime @from, DateTime to, SortOrder so)
        {
            using (var db = new TimeDataContext(TimeDataContext.DBConnectionString)) {
                try {
                    var entries = from x in db.TimeDataItems
                                  where x.Date >= @from && x.Date <= to
                                  orderby x.Date
                                  select x;

                    if (entries.Any()) {
                        var times = new List<TimeData>();
                        var e = so == SortOrder.DateNewestToOldest ? entries.ToArray().Reverse() : entries.ToArray();
                        foreach (var tdi in e) {
                            times.Add(new TimeData()
                            {
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
                } catch (Exception ee) {
                    throw ee;
                }
            }
        }
    }

    public class TimeData
    {
        public int ItemId { get; internal set; }

        public int Minutes { get; set; }

        public DateTime Date { get; set; }

        public int Magazines { get; set; }

        public int Books { get; set; }

        public int Brochures { get; set; }

        public int BibleStudies { get; set; }

        public int ReturnVisits { get; set; }

        public string Notes { get; set; }
    }
}
