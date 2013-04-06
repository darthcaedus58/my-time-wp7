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
using System.Linq;
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
		CityZToA = 3
	}

	/// <summary>
	/// Class ReturnVisitsInterface
	/// </summary>
	public class ReturnVisitsInterface
	{
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
						try {
							RvPreviousVisitData[] x = RvPreviousVisitsDataInterface.GetPreviousVisits(r.ItemId, SortOrder.DateNewestToOldest);
							if (x.Any()) {
								lv = x.First().Date;
							}
						} catch (Exception) {}
						var rr = new ReturnVisitData {
														 LastVisitDate = lv,
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
														 PhoneNumber = r.PhoneNumber
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
			using (var db = new ReturnVisitDataContext(ReturnVisitDataContext.DBConnectionString)) {
				if (db.DatabaseExists() == false)
					db.CreateDatabase();
				//else {
				//    db.DeleteDatabase();
				//    db.CreateDatabase();
				//}
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
					DateTime lv = DateTime.MinValue;
					try {
						RvPreviousVisitData[] x = RvPreviousVisitsDataInterface.GetPreviousVisits(r.ItemId, SortOrder.DateNewestToOldest);
						if (x.Any()) {
							lv = x.First().Date;
						}
					} catch (Exception) {}
					var rr = new ReturnVisitData {
													 LastVisitDate = lv,
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
													 PhoneNumber = r.PhoneNumber
												 };
					return rr;
				} catch {
					return new ReturnVisitData();
				}
			}
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
					if(deleteCalls) return RvPreviousVisitsDataInterface.DeleteAllCallsFromRv(itemId);
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

		public static int[] GetReturnVisitByLastVisitDate(SortOrder so, int maxReturnCount = 8)
		{
			//throw new NotImplementedException();
			using (var db = new ReturnVisitDataContext(ReturnVisitDataContext.DBConnectionString)) {
				try {
					using (var visitDb = new RvPreviousVisitsContext(RvPreviousVisitsContext.DBConnectionString)) {
						if (so == SortOrder.DateOldestToNewest) {
							var qry = from x in visitDb.RvPreviousVisitItems
									  orderby x.Date
									  orderby x.RvItemId 
									  select x;
							if (qry.Any()) {
								var rvs = new List<RvPreviousVisitItem>();
								int id = qry.First().RvItemId;
								rvs.Add(qry.First());
								foreach (var r in qry) {
									if (id != r.RvItemId) {
										id = r.RvItemId;
										rvs.Add(r);
									}
								}
								if (maxReturnCount < 0) maxReturnCount = rvs.Count();
								var sorted = rvs.OrderBy(s => s.Date).Take(maxReturnCount);
								return sorted.Select(i => i.RvItemId).ToArray();
							}
							return new int[0];
						}
					}
				} catch {
					return null;
				}
			}
			return null;
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
	}

	/// <summary>
	/// Class ReturnVisitData
	/// </summary>
	public class ReturnVisitData
	{
		public ReturnVisitData() { ItemId = -1; }

		/// <summary>
		/// Gets the last visit date.
		/// </summary>
		/// <value>The last visit date.</value>
		public DateTime LastVisitDate { get; internal set; }

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
				                               PhoneNumber = newRv.PhoneNumber
			                               };
		}		
	}
}