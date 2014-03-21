using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Maps.Services;
using MyTimeDatabaseLib.Annotations;
using MyTimeDatabaseLib.Model;

namespace MyTimeDatabaseLib
{
    internal class House2HouseRecordsInterface
    {
        public static void CheckDatabase()
        {
            using (var db = new House2HouseRecordDataContext()) {
                if (db.DatabaseExists() == false)
                    db.CreateDatabase();
            }
        }

        public static bool AddOrRemoveHouse2HouseRecord(ref House2HouseRecordData rec)
        {
            try {
                if (rec.ItemId < 0) return AddH2HRecord(ref rec);
                return UpdateH2HRecord(ref rec);
            } catch (H2HRecordAlreadyExistsException) {
                return UpdateH2HRecord(ref rec);
            }
        }

        private static bool AddH2HRecord(ref House2HouseRecordData rec)
        {
            using (var db = new House2HouseRecordDataContext()) {
                var r = House2HouseRecordData.Copy(rec);
                var qry = from x in db.House2HouseRecordItems
                          //unique items
                          where x.HouseAptNumber.Equals(r.HouseAptNumber) &&
                                x.StreetItemId.Equals(r.StreetItemId) &&
                                x.TerritoryItemId.Equals(r.TerritoryItemId)
                          select x;
                if (qry.Any())
                    throw new H2HRecordAlreadyExistsException();
                db.House2HouseRecordItems.InsertOnSubmit(r);
                db.SubmitChanges();
                rec.ItemId = r.ItemId;
                return rec.ItemId >= 0;
            }
        }

        private static bool UpdateH2HRecord(ref House2HouseRecordData rec)
        {
            if (rec.ItemId < 0) return AddH2HRecord(ref rec);
            try {
                using (var db = new House2HouseRecordDataContext()) {
                    int itemId = rec.ItemId;
                    var rec2 = db.House2HouseRecordItems.Single(s => s.ItemId == itemId);

                    rec2.Date = rec.Date;
                    rec2.HouseAptNumber = rec.HouseAptNumber;
                    rec2.NamePlacementRemarks = rec.NamePlacementRemarks;
                    rec2.StreetItemId = rec.StreetItemId;
                    rec2.Symbol = rec.Symbol;
                    rec2.TerritoryItemId = rec.TerritoryItemId;

                    db.SubmitChanges();

                    return true;
                }
            } catch (InvalidOperationException) {
                return AddH2HRecord(ref rec); //rv not found, lets create it`
            }
        }

        public static House2HouseRecordData GetHouse2HouseRecord(int itemId)
        {
            using (var db = new House2HouseRecordDataContext()) {
                try {
                    var r = db.House2HouseRecordItems.Single(s => s.ItemId == itemId);
                    return House2HouseRecordData.Copy(r);
                } catch {
                    return new House2HouseRecordData();
                }
            }
        }

        public static House2HouseRecordData[] GetHouse2HouseRecords(SortOrder so, int maxReturnCount = 25)
        {
            var records = new List<House2HouseRecordData>();
            using (var db = new House2HouseRecordDataContext()) {
                try {
                    if (maxReturnCount == -1) maxReturnCount = db.House2HouseRecordItems.Count();
                    IEnumerable<House2HouseRecordDataItem> demRecs = null;
                    IQueryable<House2HouseRecordDataItem> q;
                    if (so == SortOrder.DateNewestToOldest || so == SortOrder.DateOldestToNewest) {
                        q = (from x in db.House2HouseRecordItems
                             orderby x.Date
                             select x).Take(maxReturnCount);
                        demRecs = so == SortOrder.DateNewestToOldest ? q.ToArray().Reverse() : q.ToArray();
                    } else if (so == SortOrder.StreetAToZ || so == SortOrder.StreetZToA) {
                        q = (from x in db.House2HouseRecordItems
                             orderby StreetBuildingInterface.GetStreetName(x.StreetItemId)
                             select x).Take(maxReturnCount);
                        demRecs = so == SortOrder.StreetZToA ? q.ToArray().Reverse() : q.ToArray();

                    } else if (so == SortOrder.AddressNumberHighToLow || so == SortOrder.AddressNumberLowToHigh) {
                        q = (from x in db.House2HouseRecordItems
                             orderby x.HouseAptNumber
                             select x).Take(maxReturnCount);
                        demRecs = so == SortOrder.AddressNumberHighToLow ? q.ToArray().Reverse() : q.ToArray();
                    }
                    if (demRecs != null)
                        records.AddRange(demRecs.Select(House2HouseRecordData.Copy));
                    return records.ToArray();
                } catch {
                    return records.ToArray();
                }
            }
        }
    }

    public class House2HouseRecordData
    {
        public House2HouseRecordData()
        {
            ItemId = -1;
            Date = DateTime.Now;
        }

        public int ItemId { get; internal set; }

        public DateTime Date { get; set; }

        public House2HouseSymbol Symbol { get; set; }

        public string HouseAptNumber { get; set; }

        public int TerritoryItemId { get; set; }

        public int StreetItemId { get; set; }

        public string NamePlacementRemarks { get; set; }

        internal static House2HouseRecordDataItem Copy(House2HouseRecordData item)
        {
            return new House2HouseRecordDataItem() {
                Date = item.Date,
                Symbol = item.Symbol,
                HouseAptNumber = item.HouseAptNumber,
                NamePlacementRemarks = item.NamePlacementRemarks,
                StreetItemId = item.StreetItemId,
                TerritoryItemId = item.TerritoryItemId
            };
        }

        internal static House2HouseRecordData Copy(House2HouseRecordDataItem item)
        {
            return new House2HouseRecordData() {
                ItemId = item.ItemId,
                Date = item.Date,
                HouseAptNumber = item.HouseAptNumber,
                NamePlacementRemarks = item.HouseAptNumber,
                StreetItemId = item.StreetItemId,
                Symbol = item.Symbol,
                TerritoryItemId = item.TerritoryItemId
            };
        }
    }

    public class TerritoryCardsInterface
    {
        public static TerritoryCardData[] GetTerritoryCards(SortOrder so)
        {
            try {
                using (var db = new TerritoryCardsDataContext()) {
                    var qry = from x in db.TerritoryCardItems
                        orderby x.DateCreated
                        select x;

                    if (!qry.Any()) return null;
                    return qry.Select(c => TerritoryCardData.Copy(c)).ToArray();
                }
            }
            catch {
                return null;
            }
        }

        public static TerritoryCardData GetTerritoryCard(int itemId)
        {
            try {
                using (var db = new TerritoryCardsDataContext()) {
                    var c = db.TerritoryCardItems.Single(x => x.ItemId == itemId);
                    if (c == null) return null;

                    return TerritoryCardData.Copy(c);
                }
            }
            catch (InvalidOperationException e) {
                return null;
            }
        }

        public static bool AddOrUpdateTerritoryCard(ref TerritoryCardData card)
        {
            if (card.ItemId <= 0) return AddTerritoryCard(ref card);
            try {
                using (var db = new TerritoryCardsDataContext()) {
                    var i = card.ItemId;
                    var c = db.TerritoryCardItems.Single(x => x.ItemId == i);
                    if (c == null) return AddTerritoryCard(ref card);

                    c.ImageSrc = card.ImageSrc;
                    c.Notes = card.Notes;
                    c.TerritoryNumber = card.TerritoryNumber;
                    c.DateCreated = card.DateCreated;

                    db.SubmitChanges();
                    return true;
                }
            }
            catch (InvalidOperationException) {
                return AddTerritoryCard(ref card);
            }
            catch (Exception e) {
                throw e;
            }
        }

        private static bool AddTerritoryCard(ref TerritoryCardData card)
        {
            try {
                using (var db = new TerritoryCardsDataContext()) {
                    var newCard = TerritoryCardData.Copy(card);
                    db.TerritoryCardItems.InsertOnSubmit(newCard);
                    db.SubmitChanges();
                    card.ItemId = newCard.ItemId;
                    return card.ItemId > 0;
                }
            }
            catch {
                return false;
            }
        }
    }

    public class TerritoryCardData
    {
        public TerritoryCardData()
        {
            ItemId = -1;
            DateCreated = DateTime.Now;
        }
        public int ItemId { get; internal set; }
        public string TerritoryNumber { get; set; }
        public DateTime DateCreated { get; set; }
        public int[] ImageSrc { get; set; }
        public BitmapImage Image
        {
            get
            {
                return BitmapConverter.GetBitmapImage(ImageSrc);
            }
        }
        public string Notes { get; set; }

        internal static TerritoryCardItem Copy(TerritoryCardData card)
        {
            return new TerritoryCardItem
            {
                ItemId = card.ItemId,
                DateCreated = card.DateCreated,
                ImageSrc = card.ImageSrc,
                Notes = card.Notes,
                TerritoryNumber = card.TerritoryNumber
            };
        }

        internal static TerritoryCardData Copy(TerritoryCardItem card)
        {
            return new TerritoryCardData {
                ItemId = card.ItemId,
                DateCreated = card.DateCreated,
                ImageSrc = card.ImageSrc,
                Notes = card.Notes,
                TerritoryNumber = card.TerritoryNumber
            };
        }
    }

    #region StreetBuildingInterface
    public class StreetBuildingInterface
    {
        public static string GetStreetName(int itemId)
        {
            using (var db = new StreetsBuildingDataContext()) {
                try {
                    var s = db.StreetsBuildingItems.Single(r => r.ItemId == itemId);
                    return string.IsNullOrEmpty(s.BuildingNumber) ? s.Street : string.Format("{0} ({1})", s.Street, s.BuildingNumber);
                } catch {
                    throw new StreetBuildingItemDoesntExistException();
                }
            }
        }

        public static void CheckDatabase()
        {
            using (var db = new StreetsBuildingDataContext())
                if (db.DatabaseExists() == false)
                    db.CreateDatabase();
        }

        public static bool AddOrUpdate(ref StreetBuildingData st)
        {
            try {
                if (st.ItemId < 0) return AddNewStreetBuilding(ref st);
                return UpdateStreetBuilding(ref st);
            } catch (StreetBuildingItemDoesntExistException) {
                return UpdateStreetBuilding(ref st);
            } catch (StreetBuildingAlreadyExistsException) {
                return AddNewStreetBuilding(ref st);
            }

        }

        private static bool UpdateStreetBuilding(ref StreetBuildingData st)
        {
            using (var db = new StreetsBuildingDataContext()) {
                try {
                    var i = st.ItemId;
                    var stEx = db.StreetsBuildingItems.Single(s => s.ItemId == i);
                    stEx.BuildingNumber = st.BuildingNumber;
                    stEx.DateCreated = st.DateCreated;
                    stEx.Street = st.Street;
                    stEx.TerritoyrCardId = st.TerritoryCardId;

                    db.SubmitChanges();
                    return true;
                } catch (InvalidOperationException) {
                    throw new StreetBuildingItemDoesntExistException();
                }
            }
        }

        private static bool AddNewStreetBuilding(ref StreetBuildingData st)
        {
            using (var db = new StreetsBuildingDataContext()) {
                var s = StreetBuildingData.Copy(st);
                var q = from x in db.StreetsBuildingItems
                        where x.TerritoyrCardId.Equals(s.TerritoyrCardId) &&
                              x.Street.Equals(s.Street) &&
                              x.BuildingNumber.Equals(s.BuildingNumber)
                        select x;
                if (q.Any())
                    throw new StreetBuildingAlreadyExistsException();
                db.StreetsBuildingItems.InsertOnSubmit(s);
                db.SubmitChanges();
                st.ItemId = s.ItemId;
                return st.ItemId >= 0;
            }
        }

        public static StreetBuildingData GetStreetBuilding(int itemId)
        {
            using (var db = new StreetsBuildingDataContext()) {
                try {
                    var s = db.StreetsBuildingItems.Single(r => r.ItemId == itemId);
                    return StreetBuildingData.Copy(s);
                } catch (InvalidOperationException) {
                    throw new StreetBuildingItemDoesntExistException();
                }
            }
        }


    }

    public class StreetBuildingAlreadyExistsException : Exception { }

    public class StreetBuildingItemDoesntExistException : Exception { }

    public class StreetBuildingData
    {
        public int ItemId { get; internal set; }

        public int TerritoryCardId { get; set; }

        public DateTime DateCreated { get; set; }

        public string Street { get; set; }

        public string BuildingNumber { get; set; }

        public StreetBuildingData()
        {
            ItemId = -1;
            DateCreated = DateTime.Now;
        }

        internal static StreetsBuildingItem Copy(StreetBuildingData item)
        {
            return new StreetsBuildingItem() {
                ItemId = item.ItemId,
                BuildingNumber = item.BuildingNumber,
                DateCreated = item.DateCreated,
                Street = item.Street,
                TerritoyrCardId = item.TerritoryCardId
            };
        }

        internal static StreetBuildingData Copy(StreetsBuildingItem item)
        {
            return new StreetBuildingData() {
                ItemId = item.ItemId,
                BuildingNumber = item.BuildingNumber,
                DateCreated = item.DateCreated,
                Street = item.Street,
                TerritoryCardId = item.TerritoyrCardId
            };
        }
    }
    #endregion
}

internal class H2HRecordAlreadyExistsException : Exception { }
