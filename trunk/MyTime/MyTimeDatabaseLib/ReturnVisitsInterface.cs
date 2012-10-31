using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Linq;
using System.Data.Linq;
using MyTimeDatabaseLib.Model;
using System.Collections.Generic;

namespace MyTimeDatabaseLib
{
	public enum SortOrder {
		DateFirstToLast,
		DateLastToFirst
	}
	public class ReturnVisitsInterface
	{

		/// <summary>
		/// Gets the return visits of a given quantity and sorted.
		/// </summary>
		/// <param name="so">The sort order.</param>
		/// <param name="maxReturnCount">The max return count. -1 for all.</param>
		/// <returns><c>ReturnVisitData</c> Array</returns>
		public static ReturnVisitData[] GetReturnVisits(SortOrder so, int maxReturnCount)
		{
			var rvs = new List<ReturnVisitData>();
			using (var db = new ReturnVisitDataContext(ReturnVisitDataContext.DBConnectionString)) {
				
				var q = db.ReturnVisitItems.OrderBy(o => o.DateCreated).Take(maxReturnCount > 0 ? maxReturnCount : 25);
				ReturnVisitDataItem[] dem_RVs;

				if (so == SortOrder.DateFirstToLast)
					dem_RVs = q.ToArray().Reverse().ToArray();
				else
					dem_RVs = q.ToArray();
				 
				foreach (var r in dem_RVs) {
					var rr = new ReturnVisitData()
					{
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
		/// <returns><c>True</c> if successful and <c>False</c> if unsucessful.</returns>
		public static bool AddNewReturnVisit(ReturnVisitData newRv)
		{
			using (var db = new ReturnVisitDataContext(ReturnVisitDataContext.DBConnectionString)) {
				var r = new ReturnVisitDataItem()
				{
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
				try {
						
					var qry = from x in db.ReturnVisitItems
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
					else {
						db.ReturnVisitItems.InsertOnSubmit(r);
						int x = db.ChangeConflicts.Count;
						db.SubmitChanges();
						return x == db.ChangeConflicts.Count;
					}
				} catch (ReturnVisitAlreadyExistsException e) {
					throw e;
				} catch (Exception e) {
					throw e;
				}
			}
			return false;
		}

		public static void CheckDatabase()
		{
			using (var db = new ReturnVisitDataContext(ReturnVisitDataContext.DBConnectionString)) {
				if (db.DatabaseExists() == false)
					db.CreateDatabase();
				else {
					db.DeleteDatabase();
					db.CreateDatabase();
				}
					
			}
		}

		public static ReturnVisitData GetReturnVisit(int ItemID)
		{
			using (var db = new ReturnVisitDataContext(ReturnVisitDataContext.DBConnectionString)) {
				try {
					var r = db.ReturnVisitItems.Single(s => s.ItemId == ItemID);
					var rr = new ReturnVisitData()
					{
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
	}

	public class ReturnVisitData
	{
		public int ItemId
		{
			internal set;
			get;
		}

		public int[] _image;

		public int[] ImageSrc
		{
			get;
			set;
		}

		public DateTime DateCreated
		{
			get;
			set;
		}

		public string FullName
		{
			get;
			set;
		}

		public string Gender
		{
			get;
			set;
		}

		public string PhysicalDescription
		{
			get;
			set;
		}

		public string Age
		{
			get;
			set;
		}

		public string AddressOne
		{
			get;
			set;
		}

		public string PhoneNumber
		{
			get;
			set;
		}

		public string AddressTwo
		{
			get;
			set;
		}

		public string City
		{
			get;
			set;
		}

		public string StateProvince
		{
			get;
			set;
		}

		public string Country
		{
			get;
			set;
		}

		public string PostalCode
		{
			get;
			set;
		}

		public string OtherNotes
		{
			get;
			set;
		}
	}
}
