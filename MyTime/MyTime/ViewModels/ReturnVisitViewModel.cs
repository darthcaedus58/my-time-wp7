using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FieldService.BingMapsGeocodeService;
using MyTimeDatabaseLib;

namespace FieldService.ViewModels
{
	public class ReturnVisitViewModel : INotifyPropertyChanged
	{
		public ReturnVisitViewModel()
		{
			_returnVisitData = new ReturnVisitData {
				                                         DateCreated = DateTime.Now
			                                         };
		    _loadPreviousVisits = true;
			PreviousVisits = new ObservableCollection<PreviousVisitViewModel>();
		}

	    public ReturnVisitViewModel(bool loadPreviousVisits)
        {
            _returnVisitData = new ReturnVisitData {
                DateCreated = DateTime.Now
            };
            PreviousVisits = new ObservableCollection<PreviousVisitViewModel>();
	        _loadPreviousVisits = loadPreviousVisits;
	    }

		private ReturnVisitData _returnVisitData;

		public int[] IntImage
		{
			get { return _returnVisitData.ImageSrc; }
			set
			{
				if (_returnVisitData.ImageSrc == value) return;
				_returnVisitData.ImageSrc = value;
				OnPropertyChanged("Image");
			}
		}

		public BitmapImage Image
		{
			get
			{
				var wb = new WriteableBitmap(100, 100);
				for (int i = 0; i < wb.Pixels.Length; i++) {
					wb.Pixels[i] = 0xFF3300;
				}
				var bmp = new BitmapImage();
				using (var ms = new MemoryStream()) {
					wb.SaveJpeg(ms, 100, 100, 0, 100);
					bmp.SetSource(ms);
				}

				var bi = new BitmapImage();
				if (_returnVisitData.ImageSrc != null && _returnVisitData.ImageSrc.Length >= 0) {
					var ris = new WriteableBitmap(450, 250);

					//get image from database
					for (int i = 0; i < _returnVisitData.ImageSrc.Length; i++) {
						ris.Pixels[i] = _returnVisitData.ImageSrc[i];
					}

					//put the image in a WritableBitmap
					using (var ms = new MemoryStream()) {
						ris.SaveJpeg(ms, 450, 250, 0, 100);
						bi.SetSource(ms);
					}

					//crop the image to 100x100 and centered
					var img = new Image
					{
						Source = bi,
						Width = 450,
						Height = 250
					};
					var wb2 = new WriteableBitmap(100, 100);
					var t = new CompositeTransform
					{
						ScaleX = 0.5,
						ScaleY = 0.5,
						TranslateX = -((450 / 2) / 2 - 50),
						TranslateY = -((250 / 2) / 2 - 50)
					};
					wb2.Render(img, t);
					wb2.Invalidate();
					bi = new BitmapImage();
					using (var ms = new MemoryStream()) {
						wb2.SaveJpeg(ms, 100, 100, 0, 100);
						bi.SetSource(ms);
					}
					//BitmapImage bi is now cropped
				} else {
					bi = bmp; //Default image.
				}
				return bi;
			}
		}

		#region Personal Info
		public string FullName
		{
			get { return _returnVisitData.FullName; }
			set
			{
				if (_returnVisitData.FullName == value) return;
				_returnVisitData.FullName = value;
				OnPropertyChanged("FullName");
				OnPropertyChanged("NameOrDescription");
			}
		}

		public string Age
		{
			get { return _returnVisitData.Age; }
			set
			{
				if (_returnVisitData.Age == value) return;
				_returnVisitData.Age = value;
				OnPropertyChanged("Age");
				OnPropertyChanged("NameOrDescription");
			}
		}

		public string Gender
		{
			get { return _returnVisitData.Gender; }
			set
			{
				if (_returnVisitData.Gender == value) return;
				_returnVisitData.Gender = value;
				OnPropertyChanged("Gender");
				OnPropertyChanged("NameOrDescription");
			}
		}

		public string NameOrDescription
		{
			get
			{
				if (!string.IsNullOrEmpty(_returnVisitData.FullName)) {
					return _returnVisitData.FullName;
				}
				if (!string.IsNullOrEmpty(_returnVisitData.Age)) {
					return string.Format("{0} Year Old {1}", _returnVisitData.Age, _returnVisitData.Gender);
				}
				///TODO: Fix Me.
				return string.Format("{0}{1}", _returnVisitData.Gender == "Male" ? "Man" : "Woman", string.IsNullOrEmpty(_returnVisitData.PhysicalDescription) ? string.Empty : string.Format(" ({0})", _returnVisitData.PhysicalDescription));
			}
		}

		public string PhoneNumber
		{
			get { return _returnVisitData.PhoneNumber; }
			set
			{
				if (_returnVisitData.PhoneNumber == value) return;
				_returnVisitData.PhoneNumber = value;
				OnPropertyChanged("PhoneNumber");
			}
		}

		public string PhysicalDescription
		{
			get { return _returnVisitData.PhysicalDescription; }
			set
			{
				if (_returnVisitData.PhysicalDescription == value) return;
				_returnVisitData.PhysicalDescription = value;
				OnPropertyChanged("PhysicalDescription");
				OnPropertyChanged("NameOrDescription");
			}
		}
		
		#endregion

		#region Address Info
		public string Address1
		{
			get { return _returnVisitData.AddressOne; }
			set
			{
				if (_returnVisitData.AddressOne == value) return;
				_returnVisitData.AddressOne = value;
				OnPropertyChanged("Address1");
				OnPropertyChanged("FormattedAddress");
			}
		}

		public string Address2
		{
			get { return _returnVisitData.AddressTwo; }
			set
			{
				if (_returnVisitData.AddressTwo == value) return;
				_returnVisitData.AddressTwo = value;
				OnPropertyChanged("Address2");
				OnPropertyChanged("FormattedAddress");
			}
		}

		public string City
		{
			get { return _returnVisitData.City; }
			set
			{
				if (_returnVisitData.City == value) return;
				_returnVisitData.City = value;
				OnPropertyChanged("City");
				OnPropertyChanged("FormattedAddress");
			}
		}

		public string StateProvinceDistrict
		{
			get { return _returnVisitData.StateProvince; }
			set
			{
				if (_returnVisitData.StateProvince == value) return;
				_returnVisitData.StateProvince = value;
				OnPropertyChanged("StateProvinceDistrict");
				OnPropertyChanged("FormattedAddress");
			}
		}

		public string PostalCode
		{
			get { return _returnVisitData.PostalCode; }
			set
			{
				if (_returnVisitData.PostalCode == value) return;
				_returnVisitData.PostalCode = value;
				OnPropertyChanged("PostalCode");
				OnPropertyChanged("FormattedAddress");
			}
		}

		public string Country
		{
			get { return _returnVisitData.Country; }
			set
			{
				if (_returnVisitData.Country == value) return;
				_returnVisitData.Country = value;
				OnPropertyChanged("Country");
				OnPropertyChanged("FormattedAddress");
			}
		}

		public string LocationNotes
		{
			get { return _returnVisitData.OtherNotes; }
			set
			{
				if (_returnVisitData.OtherNotes == value) return;
				_returnVisitData.OtherNotes = value;
				OnPropertyChanged("LocationNotes");
				OnPropertyChanged("FormattedAddress");
			}
		}

		public string FormattedAddress
		{
			get
			{
			        if (!IsAddressValid) return string.Empty;
				var country = AddressCountryCode;
				switch(country) {
					case "it": //italy
						return string.Format("{0} {1}\n{2} {3} {4}", _returnVisitData.AddressOne, _returnVisitData.AddressTwo, _returnVisitData.PostalCode, _returnVisitData.City, _returnVisitData.StateProvince);
					case "gb": //england
						return string.Format("{0} {1}\n{2} {3}", _returnVisitData.AddressOne, _returnVisitData.AddressTwo, _returnVisitData.City, _returnVisitData.PostalCode);
					case "th": //thailand
						return string.Format("{0} {1}\n{2} {3}", _returnVisitData.AddressOne, _returnVisitData.AddressTwo, _returnVisitData.City, _returnVisitData.StateProvince);
					default:
					case "us": //usa
						return string.Format("{0} {1}\n{2},{3} {4}", _returnVisitData.AddressOne, _returnVisitData.AddressTwo, _returnVisitData.City, _returnVisitData.StateProvince, _returnVisitData.PostalCode);
				}
			}
		}

		private string AddressCountryCode
		{
			get
			{
				var country = _returnVisitData.Country;
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
						return !(string.IsNullOrEmpty(Address1) ||string.IsNullOrEmpty(City));
					case "us":
					case "it":
					case "th":
					default:
						return !(string.IsNullOrEmpty(Address1) || string.IsNullOrEmpty(City) || string.IsNullOrEmpty(StateProvinceDistrict)); 
				}
			}
		}

		public Address GeocodeAddress
		{
			get
			{
				switch(AddressCountryCode) {
					case "gb":
						return new Address
						{
							AddressLine = Address1,
							Locality = City,
							CountryRegion = string.IsNullOrEmpty(Country) ? "England" : Country
						};
					case "th":
						return new Address {
							                   AddressLine = string.Format("{0}{1}", _returnVisitData.AddressOne, string.IsNullOrEmpty(_returnVisitData.AddressTwo) ? string.Empty : string.Format(" {0}", _returnVisitData.AddressTwo)),
							                   Locality = City,
							                   AdminDistrict = StateProvinceDistrict,
							                   CountryRegion = string.IsNullOrEmpty(Country) ? "Thailand" : Country
						                   };
					case "it":
					case "us":
					default:
						return new Address
						{
							AddressLine = Address1,
							AdminDistrict = StateProvinceDistrict,
							Locality = City,
							PostalCode = PostalCode,
							CountryRegion = string.IsNullOrEmpty(Country) ? "United States" : Country
						};
					
				}
			} 
		}
		#endregion

		public string NavigateToPage { get { return string.Format("/View/EditReturnVisit.xaml?id={0}", ItemId); } }

		public int ItemId
		{
			get { return _returnVisitData.ItemId; }
			set
			{
				if (value == _returnVisitData.ItemId) return;
				if (value >= 0) {
					_returnVisitData = ReturnVisitsInterface.GetReturnVisit(value);
					if (_loadPreviousVisits) LoadPreviousVisits(value);
				} else {
					_returnVisitData = new ReturnVisitData();
					IsPreviousVisitsLoaded = true;
					PreviousVisits.Clear();
				}
				OnPropertyChanged("FullName");
				OnPropertyChanged("NameOrDescription");
				OnPropertyChanged("LocationNotes");
				OnPropertyChanged("FormattedAddress");
				OnPropertyChanged("Country");
				OnPropertyChanged("PostalCode");
				OnPropertyChanged("StateProvinceDistrict");
				OnPropertyChanged("City");
				OnPropertyChanged("Address1");
				OnPropertyChanged("Address2");
				OnPropertyChanged("PhysicalDescription");
				OnPropertyChanged("PhoneNumber");
				OnPropertyChanged("Gender");
				OnPropertyChanged("Age");
                OnPropertyChanged("LastVisitDate");
			}
		}

		public DateTime LastVisitDate
		{
			get
			{
				if (_returnVisitData.ItemId < 0) return DateTime.Now;
			    if (_returnVisitData.LastVisitDate == DateTime.MinValue) {
			        var x = RvPreviousVisitsDataInterface.GetPreviousVisits(_returnVisitData.ItemId, SortOrder.DateNewestToOldest);
			        if (x.Any()) {
			            return x.First().Date;
			        }
			    }
			    else {
			        return _returnVisitData.LastVisitDate;
			    }
			    return DateTime.Now;
			}
		}

		public int DaysSinceLastVisit
		{
			get { return (DateTime.Now - LastVisitDate).Days; }
		}

		public void LoadPreviousVisits(int value)
		{
			if (IsPreviousVisitsLoaded) {
				IsPreviousVisitsLoaded = false;
				PreviousVisits.Clear();
			}
			var prevVisits = RvPreviousVisitsDataInterface.GetPreviousVisits(value, SortOrder.DateNewestToOldest);
			foreach (var p in prevVisits) {
				PreviousVisits.Add(new PreviousVisitViewModel() {
					                                                PreviousVisitData = p,
																	ParentRv = this
				                                                });
			}
			IsPreviousVisitsLoaded = true;
		}

		public ObservableCollection<PreviousVisitViewModel> PreviousVisits {get; private set;}

		public bool IsPreviousVisitsLoaded;
	    private bool _loadPreviousVisits;

	    #region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}

		public bool SaveOrUpdate()
		{
			if (_returnVisitData.DateCreated <= DateTime.MinValue || _returnVisitData.DateCreated >= DateTime.MaxValue) _returnVisitData.DateCreated = DateTime.Now;	
			App.ViewModel.IsRvDataChanged = true;
			return ReturnVisitsInterface.AddOrUpdateRV(ref _returnVisitData);
		}

		public bool Delete(bool deleteCalls)
		{
			//throw new System.NotImplementedException();
			App.ViewModel.IsRvDataChanged = true;
			return ReturnVisitsInterface.DeleteReturnVisit(_returnVisitData.ItemId, deleteCalls);
		}
	}
}