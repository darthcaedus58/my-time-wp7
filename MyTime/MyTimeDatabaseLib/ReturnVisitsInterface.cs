// ***********************************************************************
// Assembly         : MyTimeDatabaseLib
// Author           : trevo_000
// Created          : 11-03-2012
//
// Last Modified By : trevo_000
// Last Modified On : 11-10-2012
// ***********************************************************************
// <copyright file="ReturnVisitsInterface.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Data.Linq;
using MyTimeDatabaseLib.Model;

namespace MyTimeDatabaseLib
{
    /// <summary>
    /// Enum SortOrder
    /// </summary>
    public enum SortOrder
    {
        /// <summary>
        /// The date newest to oldest
        /// </summary>
        DateNewestToOldest = 0,

        /// <summary>
        /// The date oldest to newest
        /// </summary>
        DateOldestToNewest = 1,
        /// <summary>
        /// The city A to Z
        /// </summary>
        CityAToZ = 2,
        /// <summary>
        /// The city Z to A
        /// </summary>
        CityZToA = 3,
        StreetAToZ,
        StreetZToA,
        AddressNumberHighToLow,
        AddressNumberLowToHigh,
        AscendingGeneric,
        DescendingGeneric
    }

    /// <summary>
    /// Class ReturnVisitsInterface
    /// </summary>
    public class ReturnVisitsInterface
    {
        public const int APP_VERSION = 2;
        /// <summary>
        /// Gets the return visits of a given quantity and sorted.
        /// </summary>
        /// <param name="so">The sort order.</param>
        /// <param name="maxReturnCount">The max return count. -1 for all.</param>
        /// <returns><c>ReturnVisitData</c> Array</returns>
        public static ReturnVisitData[] GetReturnVisits(SortOrder so, int maxReturnCount = 25)
        {
            var rvs = new List<ReturnVisitData>();
            using (var db = new ReturnVisitDataContext(ReturnVisitDataContext.DBConnectionString)) {
                if (maxReturnCount == -1) maxReturnCount = db.ReturnVisitItems.Count();
                IQueryable<ReturnVisitDataItem> q;
                IEnumerable<ReturnVisitDataItem> demRVs = null;
                if (so == SortOrder.DateNewestToOldest || so == SortOrder.DateOldestToNewest) {
                    q = from x in db.ReturnVisitItems
                        orderby x.DateCreated
                        select x;
                    q = q.Take(maxReturnCount);
                    demRVs = so == SortOrder.DateNewestToOldest ? q.ToArray().Reverse() : q.ToArray();
                } else if (so == SortOrder.CityAToZ || so == SortOrder.CityZToA) {
                    q = from x in db.ReturnVisitItems
                        orderby x.City
                        select x;
                    q = q.Take(maxReturnCount);
                    demRVs = so == SortOrder.CityZToA ? q.ToArray().Reverse() : q.ToArray();
                }

                if (demRVs != null)
                    foreach (ReturnVisitDataItem r in demRVs) {
                        DateTime lv = DateTime.MinValue;
                        var rr = new ReturnVisitData {
                            ItemId = r.ItemId,
                            DateCreated = r.DateCreated,
                            AddressOne = r.AddressOne,
                            AddressTwo = r.AddressTwo,
                            Age = r.Age,
                            City = r.City,
                            Country = r.Country,
                            FullName = r.FullName,
                            Gender = r.Gender,
                            OtherNotes = r.OtherNotes,
                            PhysicalDescription = r.PhysicalDescription,
                            PostalCode = r.PostalCode,
                            StateProvince = r.StateProvince,
                            ImageSrc = r.ImageSrc,
                            PhoneNumber = r.PhoneNumber,
                            Latitude = r.Latitude ?? 0,
                            Longitude = r.Longitude ?? 0,
                            LastVisitDate = r.LastVisitDate ?? GetLastVisitDate(r)
                        };
                        rvs.Add(rr);
                    }
            }
            return rvs.ToArray();
        }

        /// <summary>
        /// Adds the new return visit.
        /// </summary>
        /// <param name="newRv">The new rv.</param>
        /// <returns><c>True</c> if successful and <c>False</c> if unsuccessful.</returns>
        /// <exception cref="MyTimeDatabaseLib.ReturnVisitAlreadyExistsException">The Return Visit already exists.</exception>
        public static bool AddNewReturnVisit(ref ReturnVisitData newRv)
        {
            using (var db = new ReturnVisitDataContext(ReturnVisitDataContext.DBConnectionString)) {
                var r = ReturnVisitData.Copy(newRv);
                IQueryable<ReturnVisitDataItem> qry = from x in db.ReturnVisitItems
                                                      where x.AddressOne.Equals(r.AddressOne) &&
                                                            x.AddressTwo.Equals(r.AddressTwo) &&
                                                            x.City.Equals(r.City) &&
                                                            x.Country.Equals(r.Country) &&
                                                            x.StateProvince.Equals(r.StateProvince) &&
                                                            x.PostalCode.Equals(r.PostalCode) &&
                                                            x.FullName.Equals(r.FullName)
                                                      select x;
                if (qry.Any())
                    throw new ReturnVisitAlreadyExistsException("The Return Visit already exists.", qry.First().ItemId);
                db.ReturnVisitItems.InsertOnSubmit(r);
                db.SubmitChanges();
                newRv.ItemId = r.ItemId;
                return newRv.ItemId >= 0;
            }
        }

        /// <summary>
        /// Checks the database.
        /// </summary>
        public static void CheckDatabase()
        {
            using (var db = new ReturnVisitDataContext(ReturnVisitDataContext.DBConnectionString))
            {
                if (db.DatabaseExists() == false)
                {
                    db.CreateDatabase();
                    DatabaseSchemaUpdater dbUpdater = db.CreateDatabaseSchemaUpdater();
                    dbUpdater.DatabaseSchemaVersion = APP_VERSION;
                    dbUpdater.Execute();
                }
                else
                {
                    var dbUpdater = db.CreateDatabaseSchemaUpdater();
                    if (dbUpdater.DatabaseSchemaVersion < 2)
                    {
                        //update from 1.0 to 2.0 db version
                        dbUpdater.AddColumn<ReturnVisitDataItem>("Longitude");
                        dbUpdater.AddColumn<ReturnVisitDataItem>("Latitude");
                        dbUpdater.AddColumn<ReturnVisitDataItem>("LastVisitDate");
                        dbUpdater.DatabaseSchemaVersion = APP_VERSION;
                        dbUpdater.Execute();
                        var rvList = GetReturnVisits(SortOrder.DateNewestToOldest, -1);
                        foreach (var r in rvList)
                        {
                            var x = r;
                            if (r.LastVisitDate > DateTime.MinValue) {
                                r.LastVisitDate = GetLastVisitDate(ReturnVisitData.Copy(x));
                            }
                            UpdateReturnVisit(ref x);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the return visit.
        /// </summary>
        /// <param name="itemID">The item ID.</param>
        /// <returns>ReturnVisitData.</returns>
        public static ReturnVisitData GetReturnVisit(int itemID)
        {
            using (var db = new ReturnVisitDataContext(ReturnVisitDataContext.DBConnectionString)) {
                try {
                    ReturnVisitDataItem r = db.ReturnVisitItems.Single(s => s.ItemId == itemID);
                    var rr = new ReturnVisitData {
                        ItemId = r.ItemId,
                        DateCreated = r.DateCreated,
                        AddressOne = r.AddressOne,
                        AddressTwo = r.AddressTwo,
                        Age = r.Age,
                        City = r.City,
                        Country = r.Country,
                        FullName = r.FullName,
                        Gender = r.Gender,
                        OtherNotes = r.OtherNotes,
                        PhysicalDescription = r.PhysicalDescription,
                        PostalCode = r.PostalCode,
                        StateProvince = r.StateProvince,
                        ImageSrc = r.ImageSrc,
                        PhoneNumber = r.PhoneNumber,
                        Latitude = r.Latitude ?? 0,
                        Longitude = r.Longitude ?? 0,
                        LastVisitDate = r.LastVisitDate ?? GetLastVisitDate(r)
                    };
                    return rr;
                } catch {
                    return new ReturnVisitData();
                }
            }
        }

        private static DateTime GetLastVisitDate(ReturnVisitDataItem r)
        {
            DateTime lv = DateTime.MinValue;
            try {
                RvPreviousVisitData[] x = RvPreviousVisitsDataInterface.GetPreviousVisits(r.ItemId, SortOrder.DateNewestToOldest);
                if (x.Any()) { lv = x.First().Date; }
            } catch (Exception) { }
            return lv;
        }

        /// <summary>
        /// Updates the return visit.
        /// </summary>
        /// <param name="newRv">The new rv.</param>
        public static bool UpdateReturnVisit(ref ReturnVisitData newRv)
        {
            using (var db = new ReturnVisitDataContext(ReturnVisitDataContext.DBConnectionString)) {
                try {
                    int itemId = newRv.ItemId;
                    ReturnVisitDataItem rv = db.ReturnVisitItems.Single(s => s.ItemId == itemId);

                    rv.AddressOne = newRv.AddressOne;
                    rv.AddressTwo = newRv.AddressTwo;
                    rv.Age = newRv.Age;
                    rv.City = newRv.City;
                    rv.Country = newRv.Country;
                    rv.FullName = newRv.FullName;
                    rv.ImageSrc = newRv.ImageSrc;
                    rv.OtherNotes = newRv.OtherNotes;
                    rv.PhoneNumber = newRv.PhoneNumber;
                    rv.PhysicalDescription = newRv.PhysicalDescription;
                    rv.PostalCode = newRv.PostalCode;
                    rv.StateProvince = newRv.StateProvince;
                    rv.Gender = newRv.Gender;
                    rv.Latitude = newRv.Latitude;
                    rv.Longitude = newRv.Longitude;
                    rv.LastVisitDate = newRv.LastVisitDate;

                    db.SubmitChanges();
                    return true;
                } catch (InvalidOperationException) {
                    return AddNewReturnVisit(ref newRv); //rv not found, lets create it.
                }
            }
        }

        /// <summary>
        /// Deletes the return visit.
        /// </summary>
        /// <param name="itemId">The item id.</param>
        public static bool DeleteReturnVisit(int itemId, bool deleteCalls)
        {
            using (var db = new ReturnVisitDataContext(ReturnVisitDataContext.DBConnectionString)) {
                try {
                    ReturnVisitDataItem rv = db.ReturnVisitItems.Single(s => s.ItemId == itemId);

                    db.ReturnVisitItems.DeleteOnSubmit(rv);
                    db.SubmitChanges();
                    if (deleteCalls) return RvPreviousVisitsDataInterface.DeleteAllCallsFromRv(itemId);
                    return true;
                } catch (InvalidOperationException) { return false; }
            }
        }

        public static bool AddOrUpdateRV(ref ReturnVisitData rv)
        {
            try {

                if (rv.ItemId < 0) return AddNewReturnVisit(ref rv);
                return UpdateReturnVisit(ref rv);
            } catch (ReturnVisitAlreadyExistsException) {
                return UpdateReturnVisit(ref rv);
            }
        }

        public static int[] NoLongerInUse(SortOrder so, int maxReturnCount = 8)  // Formerly the function known as GetReturnVisitByLastVisitDate
        {
            try {
                using (var visitDb = new RvPreviousVisitsContext(RvPreviousVisitsContext.DBConnectionString)) {
                    if (so != SortOrder.DateOldestToNewest) return new int[0];
                    var qry = from x in visitDb.RvPreviousVisitItems
                        orderby x.Date
                        group x by x.RvItemId into dates
                        select dates;
                    if (maxReturnCount == -1) maxReturnCount = qry.Count();
                    if (qry.Any()) {
                        var rvList = new List<RvPreviousVisitItem>();
                        foreach (var rv in qry) {
                            var r = so == SortOrder.DateOldestToNewest ? rv.Last() : rv.First();
                            if (rvList.Count() == maxReturnCount) {
                                if (so == SortOrder.DateOldestToNewest && rvList.Select(x => x.Date > r.Date).Any()) {
                                    rvList.Remove(rvList.Last());
                                    rvList.Add(r);
                                    rvList = rvList.OrderBy(s => s.Date).ToList();
                                } else if (rvList.Select(x => x.Date < r.Date).Any()) {
                                    rvList.Remove(rvList.First());
                                    rvList.Add(r);
                                    rvList = rvList.OrderBy(s => s.Date).ToList();
                                }
                            } else {
                                rvList.Add(r);
                                rvList = rvList.OrderBy(s => s.Date).ToList();
                            }
                        }
                        return rvList.Select(r => r.RvItemId).ToArray();
                    }
                    //if (qry.Any()) {
                    //	var rvs = new List<RvPreviousVisitItem>();
                    //	int id = qry.First().RvItemId;
                    //	rvs.Add(qry.First());
                    //	foreach (var r in qry) {
                    //		if (id != r.RvItemId) {
                    //			id = r.RvItemId;
                    //			rvs.Add(r);
                    //		}
                    //	}
                    //	if (maxReturnCount < 0) maxReturnCount = rvs.Count();
                    //	var sorted = rvs.OrderBy(s => s.Date).Take(maxReturnCount);
                    //	return sorted.Select(i => i.RvItemId).ToArray();
                    //}
                    return new int[0];
                }
            } catch {
                return null;
            }
        }

        public static bool IdExists(int rv)
        {
            //throw new NotImplementedException();
            using (var db = new ReturnVisitDataContext(ReturnVisitDataContext.DBConnectionString)) {
                try {
                    var b = db.ReturnVisitItems.Single(s => s.ItemId == rv);
                    if (b == null) return false;
                    return true;
                } catch {
                    return false;
                }

            }
        }

        public static List<ReturnVisitData> GetReturnVisitsByLastVisitDate(int maxReturnCount = 8)
        {
            try {
                using (var db = new ReturnVisitDataContext(ReturnVisitDataContext.DBConnectionString)) {
                    var qry = from x in db.ReturnVisitItems
                        orderby x.LastVisitDate
                        select x;

                    if (!qry.Any()) return null;
                    bool save = false;
                    foreach (var r in qry) {
                        if (r.LastVisitDate == null) {
                            r.LastVisitDate = GetLastVisitDate(r);
                            save = true;
                        }
                    }
                    if(save) db.SubmitChanges();
                    return qry.Take(maxReturnCount == -1 ? qry.Count() : maxReturnCount).Select(rv => ReturnVisitData.Copy(rv)).ToList();
                }
            }
            catch {
                return null;
            }
        }

        public static bool UpdateLastVisitDate(int rvItemId, DateTime date)
        {
            try {
                using (var db = new ReturnVisitDataContext()) {
                    var rv = db.ReturnVisitItems.Single(s => s.ItemId == rvItemId);
                    if (rv != null && date > rv.LastVisitDate) { //Don't update if the new rv visit date is prior to the current last visit date
                        rv.LastVisitDate = date;
                        db.SubmitChanges();
                    } else if (rv != null) {
                        return true;
                    }
                }
                return false;
            }
            catch (InvalidOperationException) {
                return false;
            }
            return false;
        }

        public static bool DeleteCallFromRv(int rvItemId, DateTime date)
        {
            try {
                using (var db = new ReturnVisitDataContext()) {
                    var rv = db.ReturnVisitItems.Single(s => s.ItemId == rvItemId);
                    if (date >= rv.LastVisitDate) {
                        //Just to be safe checking '>='
                        rv.LastVisitDate = GetLastVisitDate(rv);
                        db.SubmitChanges();
                    }
                    return true;
                }
            }
            catch {
                return false;
            }
        }
    }

    /// <summary>
    /// Class ReturnVisitData
    /// </summary>
    public class ReturnVisitData
    {
        public override string ToString()
        {
            return AddressOne;
        }

        public ReturnVisitData() { ItemId = -1; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        /// <summary>
        /// Gets the last visit date.
        /// </summary>
        /// <value>The last visit date.</value>
        public DateTime LastVisitDate { get; set; }

        /// <summary>
        /// Gets or sets the item id.
        /// </summary>
        /// <value>The item id.</value>
        public int ItemId { internal set; get; }

        /// <summary>
        /// Gets or sets the image SRC.
        /// </summary>
        /// <value>The image SRC.</value>
        public int[] ImageSrc { get; set; }

        public BitmapImage Image
        {
            get {
                return BitmapConverter.GetBitmapImage(ImageSrc);
            }
        }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        /// <value>The date created.</value>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the full name.
        /// </summary>
        /// <value>The full name.</value>
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets the gender.
        /// </summary>
        /// <value>The gender.</value>
        public string Gender { get; set; }

        /// <summary>
        /// Gets or sets the physical description.
        /// </summary>
        /// <value>The physical description.</value>
        public string PhysicalDescription { get; set; }

        /// <summary>
        /// Gets or sets the age.
        /// </summary>
        /// <value>The age.</value>
        public string Age { get; set; }

        /// <summary>
        /// Gets or sets the address one.
        /// </summary>
        /// <value>The address one.</value>
        public string AddressOne { get; set; }

        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        /// <value>The phone number.</value>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the address two.
        /// </summary>
        /// <value>The address two.</value>
        public string AddressTwo { get; set; }

        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        /// <value>The city.</value>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the state province.
        /// </summary>
        /// <value>The state province.</value>
        public string StateProvince { get; set; }

        /// <summary>
        /// Gets or sets the country.
        /// </summary>
        /// <value>The country.</value>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the postal code.
        /// </summary>
        /// <value>The postal code.</value>
        public string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the other notes.
        /// </summary>
        /// <value>The other notes.</value>
        public string OtherNotes { get; set; }

        public string NameOrDescription
        {
            get
            {
                if (!string.IsNullOrEmpty(FullName)) {
                    return FullName;
                }
                if (!string.IsNullOrEmpty(Age)) {
                    return string.Format("{0} Year Old {1}", Age, Gender);
                }
                ///TODO: Fix Me.
                return string.Format("{0}{1}", Gender == "Male" ? "Man" : "Woman", string.IsNullOrEmpty(PhysicalDescription) ? string.Empty : string.Format(" ({0})", PhysicalDescription));
            }
        }

        private string AddressCountryCode
        {
            get
            {
                var country = Country;
                return GetCountryCode(country);
            }
        }

        public static string GetCountryCode(string country)
        {
            if (string.IsNullOrEmpty(country)) country = string.Empty;
            switch (country.ToLower()) {
                case "":
                    country = CultureInfo.CurrentCulture.Name.Remove(0, 3).ToLower();
                    break;
                case "thailand":
                    country = "th";
                    break;
                case "italy":
                case "italia":
                case "it":
                    country = "it";
                    break;
                case "england":
                case "great britian":
                case "britian":
                case "gb":
                case "uk":
                case "u.k.":
                case "united kingdom":
                    country = "gb";
                    break;
                case "usa":
                case "us":
                case "u.s.a.":
                case "united states":
                case "united states of america":
                case "america":
                    country = "us";
                    break;
                default:
                    country = CultureInfo.CurrentCulture.Name.Remove(0, 3).ToLower();
                    break;
            }
            return country;
        }

        public bool IsAddressValid
        {
            get
            {
                switch (AddressCountryCode) {
                    case "gb":
                        return !(string.IsNullOrEmpty(AddressOne) || string.IsNullOrEmpty(City));
                    case "us":
                    case "it":
                    case "th":
                    default:
                        return !(string.IsNullOrEmpty(AddressOne) || string.IsNullOrEmpty(City) || string.IsNullOrEmpty(StateProvince));
                }
            }
        }

        public string FormattedAddress
        {
            get
            {
                if (!IsAddressValid) return string.Empty;
                var country = AddressCountryCode;
                switch (country) {
                    case "it": //italy
                        return string.Format("{0} {1}\n{2} {3} {4}", AddressOne, AddressTwo, PostalCode, City, StateProvince);
                    case "gb": //england
                        return string.Format("{0} {1}\n{2} {3}", AddressOne, AddressTwo, City, PostalCode);
                    case "th": //thailand
                        return string.Format("{0} {1}\n{2} {3}", AddressOne, AddressTwo, City, StateProvince);
                    default:
                    case "us": //usa
                        return string.Format("{0} {1}\n{2},{3} {4}", AddressOne, AddressTwo, City, StateProvince, PostalCode);
                }
            }
        }

        internal static ReturnVisitDataItem Copy(ReturnVisitData newRv)
        {
            return new ReturnVisitDataItem {
                AddressOne = newRv.AddressOne,
                AddressTwo = newRv.AddressTwo,
                Age = newRv.Age,
                City = newRv.City,
                Country = newRv.Country,
                DateCreated = newRv.DateCreated,
                FullName = newRv.FullName,
                Gender = newRv.Gender,
                OtherNotes = newRv.OtherNotes,
                PhysicalDescription = newRv.PhysicalDescription,
                PostalCode = newRv.PostalCode,
                StateProvince = newRv.StateProvince,
                ImageSrc = newRv.ImageSrc,
                PhoneNumber = newRv.PhoneNumber,
                Latitude = newRv.Latitude,
                Longitude = newRv.Longitude,
                LastVisitDate = newRv.LastVisitDate
            };
        }

        internal static ReturnVisitData Copy(ReturnVisitDataItem newRv)
        {
            return new ReturnVisitData()
            {
                ItemId = newRv.ItemId,
                AddressOne = newRv.AddressOne,
                AddressTwo = newRv.AddressTwo,
                Age = newRv.Age,
                City = newRv.City,
                Country = newRv.Country,
                DateCreated = newRv.DateCreated,
                FullName = newRv.FullName,
                Gender = newRv.Gender,
                OtherNotes = newRv.OtherNotes,
                PhysicalDescription = newRv.PhysicalDescription,
                PostalCode = newRv.PostalCode,
                StateProvince = newRv.StateProvince,
                ImageSrc = newRv.ImageSrc,
                PhoneNumber = newRv.PhoneNumber,
                Latitude = newRv.Latitude ?? 0.0,
                Longitude = newRv.Longitude ?? 0.0,
                LastVisitDate = newRv.LastVisitDate ?? DateTime.MinValue
            };
        }
    }
}