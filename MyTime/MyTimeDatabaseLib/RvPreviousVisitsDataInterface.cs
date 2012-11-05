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
                            Magazines = q.Magazines,
                            Books = q.Books,
                            Brochures = q.Brochures
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

        public static void SaveCall(RvPreviousVisitData call)
        {
            if (call.RvItemId == null || call.RvItemId < 0) throw new ArgumentNullException("rvItemId", "Rv Item Id can't be null");

            using (var db = new RvPreviousVisitsContext(RvPreviousVisitsContext.DBConnectionString)) {
                if (db.RvPreviousVisitItems.Any(s => s.ItemId == call.ItemId)) {
                    var c = db.RvPreviousVisitItems.Single(s => s.ItemId == call.ItemId);
                    
                    c.RvItemId = call.RvItemId;
                    c.Magazines = call.Magazines;
                    c.Books = call.Books;
                    c.Brochures = call.Brochures;
                    c.Date = call.Date;
                    c.Notes = call.Notes;

                    db.SubmitChanges();
                    return;
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
            }
        }
    }

    public class RvPreviousVisitData
    {
        public int ItemId {	internal set; get; }

        public int RvItemId	{ get; set;	}

        public string Notes { get; set; }

        public int Magazines { get; set; }

        public int Books { get; set; }

        public int Brochures { get; set; }

        public DateTime Date { get; set; }
    }
}
