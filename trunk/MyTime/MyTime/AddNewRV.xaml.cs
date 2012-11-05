using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Controls.Maps.Platform;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Tasks;
using MyTime.BingMapsGeocodeService;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;
using MyTimeDatabaseLib;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;

namespace MyTime
{
    public partial class AddNewRV : PhoneApplicationPage
    {
        private readonly string _address;
        private readonly string _cityStateZip;
        private readonly string _fullName;
        private readonly string _phNum, _age;
        private GeocodeResult _currentBingGeocodeLocation;
        private GeoCoordinateWatcher gcw;

        private ReturnVisitData _currentReturnVisitData = null;

        public AddNewRV()
        {
            InitializeComponent();
            pbGetAddress.IsIndeterminate = true;
            _address = lblInfo_Address.Text;
            _fullName = lblInfo_FullName.Text;
            _cityStateZip = lblInfo_CityStateZip.Text;
            _phNum = lblInfo_telephone.Text;
            _age = lblInfo_Age.Text;
            dlsAge.SelectedItem = 30;
            string[] genders = { "Male", "Female" };
            lpGender.ItemsSource = genders;
            lpGender.SelectedIndex = 0;
            SetInfoText();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            RefreshReminderList();
            if (!NavigationContext.QueryString.ContainsKey("id")) {
                //appbar_delete.IsEnabled = false;
                return;
            }
            string id = NavigationContext.QueryString["id"];
            if (string.IsNullOrEmpty(id)) {
                //appbar_delete.IsEnabled = false;
                return;
            }
            try {
                int ItemID = int.Parse(id);
                ReturnVisitData rv = ReturnVisitsInterface.GetReturnVisit(ItemID);
                _currentReturnVisitData = rv;
                tbAddress1.Text = rv.AddressOne;
                tbAddress2.Text = rv.AddressTwo;
                tbCity.Text = rv.City;
                tbDistrict.Text = rv.StateProvince;
                tbFullName.Text = rv.FullName;
                tbOtherNotes.Text = rv.OtherNotes;
                tbZipCode.Text = rv.PostalCode;
                SetInfoText();
                //appbar_delete.IsEnabled = true;
            } catch { } finally {
                RefreshVisitList();
            }
        }

        private void RefreshVisitList()
        {
            if (_currentReturnVisitData == null) return;

            var visits = RvPreviousVisitsDataInterface.GetPreviousVisits(_currentReturnVisitData.ItemId);

           App.ViewModel.UpdatePreviousVisits(visits);
        }

        #region Events

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            gcw = new GeoCoordinateWatcher();
            mapInfo.SetView(new GeoCoordinate(40, -95), 4);
            gcw.PositionChanged += currentLocation_PositionChanged;
            gcw.Start(true);
        }

        private void Pivot_LoadedPivotItem(object sender, PivotItemEventArgs e) { Focus(); }
        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e) { SetInfoText(); }
        
        private void appbar_DeleteRv_Click(object sender, EventArgs e)
        {
            
        }

        private void appbar_save_Click(object sender, EventArgs e)
        {
            try {
                if (!ValidateRVData()) {
                    MessageBox.Show("At a minimum you must a valid address filled out to save an rv.");
                    return;
                }
                var wb = new WriteableBitmap(100, 100);
                wb.Render(mapInfo, new TranslateTransform());
                wb.Invalidate();

                var newRv = new ReturnVisitData()
                {
                    AddressOne = tbAddress1.Text,
                    AddressTwo = tbAddress2.Text,
                    City = tbCity.Text,
                    Country = _currentBingGeocodeLocation.Address.CountryRegion,
                    StateProvince = tbDistrict.Text,
                    PostalCode = tbZipCode.Text,
                    Age = dlsAge.SelectedItem.ToString(),
                    Gender = lpGender.SelectedItem.ToString(),
                    FullName = tbFullName.Text,
                    DateCreated = DateTime.Now,
                    OtherNotes = tbOtherNotes.Text,
                    PhysicalDescription = tbDescription.Text,
                    ImageSrc = wb.Pixels
                };

                ReturnVisitsInterface.AddNewReturnVisit(newRv);
                MessageBox.Show(string.Format("Return Visit {0}.", _currentReturnVisitData == null ? "Added" : "Saved"));
            } catch (Exception ee) {
                MessageBox.Show("Can't Add.\n\nException:\n"+ee.Message);
            }
        }

        private bool ValidateRVData()
        {
            if (string.IsNullOrEmpty(tbAddress1.Text) ||
                string.IsNullOrEmpty(tbCity.Text) ||
                string.IsNullOrEmpty(tbDistrict.Text)) return false;
            return true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e) { }
        
        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            try {
                string reminderName = Guid.NewGuid().ToString();
                // use guid for reminder name, since reminder names must be unique
                Reminder reminder = new Reminder(reminderName);
                // NOTE: setting the Title property is supported for reminders 
                // in contrast to alarms where setting the Title property is not supported
                reminder.Title = this.txtTitle.Text;
                reminder.Content = this.txtContent.Text;

                DateTime remindNow = new DateTime(dpDatePicker.Value.Value.Year, dpDatePicker.Value.Value.Month, dpDatePicker.Value.Value.Day, tpStartTime.Value.Value.Hour, tpStartTime.Value.Value.Minute, tpStartTime.Value.Value.Second);

                var span = remindNow - DateTime.Now;

                reminder.BeginTime = DateTime.Now.AddSeconds(span.TotalSeconds);

                reminder.ExpirationTime = reminder.BeginTime.AddSeconds(5.0);
                reminder.RecurrenceType = RecurrenceInterval.None;

                ScheduledActionService.Add(reminder);

                MessageBox.Show(string.Format("Reminder: '{0}' added.", txtTitle.Text), "Field Service App", MessageBoxButton.OK);

                txtTitle.Text = string.Empty;
                txtContent.Text = string.Empty;
                dpDatePicker.Value = DateTime.Now;
                tpStartTime.Value = DateTime.Now;
            } catch {
                MessageBox.Show("Could Not Add Reminder.");
            } finally {
                RefreshReminderList();
            }
        }

        private void currentLocation_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            var p = new Pushpin {Location = e.Position.Location};
            mapCurrentAddress.Children.Clear();
            mapCurrentAddress.Children.Add(p);
            mapCurrentAddress.SetView(e.Position.Location, 12);
            MakeReverseGeocodeRequest(e.Position.Location);
        }

        private void geocodeService_GeocodeCompleted(object sender, GeocodeCompletedEventArgs e)
        {
            if (e.Result.Results == null || e.Result.Results.Count == 0) return;
            var p = new Pushpin {Location = e.Result.Results[0].Locations[0]};
            mapInfo.Children.Clear();
            mapInfo.Children.Add(p);
            mapInfo.SetView(p.Location, 11);
        }

        private void geocodeService_ReverseGeocodeCompleted(object sender, ReverseGeocodeCompletedEventArgs e)
        {
            if (e.Result.Results == null || e.Result.Results.Count == 0) return;
            GeocodeResult Results = e.Result.Results[0];
            foreach (GeocodeResult r in e.Result.Results) {
                if (r.Confidence == Confidence.High) Results = r;
            }

            lblCurrentAddress.Text = string.Format("{0}", Results.Address.AddressLine);
            lblCurrentCityStateZip.Text = string.Format("{0}, {1} {2}", Results.Address.Locality, Results.Address.AdminDistrict, Results.Address.PostalCode);
            _currentBingGeocodeLocation = Results;
        }

        private void mapCurrentAddress_Tap(object sender, GestureEventArgs e)
        {
            tbAddress1.Text = _currentBingGeocodeLocation.Address.AddressLine;
            tbCity.Text = _currentBingGeocodeLocation.Address.Locality;
            tbZipCode.Text = _currentBingGeocodeLocation.Address.PostalCode;
            tbDistrict.Text = _currentBingGeocodeLocation.Address.AdminDistrict;
        }

        private void mapInfo_Tap(object sender, GestureEventArgs e)
        {
            var bmt = new BingMapsTask();
            bmt.Center = mapInfo.Center;
            bmt.ZoomLevel = 18;
            bmt.Show();
        }

        private void miAddPicture_Click(object sender, EventArgs e) { }
        private void miGetDirection_Click(object sender, EventArgs e) { }


        private void tbAddress1_LostFocus(object sender, RoutedEventArgs e) { SetInfoText(); }

        private void tbAddress2_LostFocus(object sender, RoutedEventArgs e) { SetInfoText(); }

        private void tbCity_TextChanged(object sender, TextChangedEventArgs e) { SetInfoText(); }
        private void tbFullName_LostFocus(object sender, RoutedEventArgs e) { SetInfoText(); }

        private void tbZipCode_LostFocus(object sender, RoutedEventArgs e) { SetInfoText(); }
        private void textBlock1_LostFocus(object sender, RoutedEventArgs e) { }

        #endregion

        private void RefreshReminderList()
        {
            var reminders = ScheduledActionService.GetActions<Reminder>();
            foreach (var r in reminders) if (r.BeginTime < DateTime.Now) ScheduledActionService.Remove(r.Name);
            lbReminders.ItemsSource = ScheduledActionService.GetActions<Reminder>();
        }

        private void MakeReverseGeocodeRequest(Location l)
        {
            try {
                // Set a Bing Maps key before making a request
                const string KEY = "AkRhgqPR6aujo-xib-KiR8Lt20wsn89GY4R9SP0RA6h4w7QT9mS3kKwYKKxjklfV";

                // Set the credentials using a valid Bing Maps key
                // Set the point to use to find a matching address
                var reverseGeocodeRequest = new ReverseGeocodeRequest {Credentials = new Credentials {ApplicationId = KEY}};

                Location point = l;

                reverseGeocodeRequest.Location = point;
                reverseGeocodeRequest.UserProfile = new UserProfile {DeviceType = DeviceType.Mobile};

                // Make the reverse geocode request
                var geocodeService = new GeocodeServiceClient("BasicHttpBinding_IGeocodeService");
                geocodeService.ReverseGeocodeCompleted += geocodeService_ReverseGeocodeCompleted;
                geocodeService.ReverseGeocodeAsync(reverseGeocodeRequest);
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        public void MakeGeocodeRequest(Address a)
        {
            try {
                // Set a Bing Maps key before making a request
                const string KEY = "AkRhgqPR6aujo-xib-KiR8Lt20wsn89GY4R9SP0RA6h4w7QT9mS3kKwYKKxjklfV";

                // Set the credentials using a valid Bing Maps key
                // Set the point to use to find a matching address
                var geocodeRequest = new GeocodeRequest {Credentials = new Credentials {ApplicationId = KEY}};

                geocodeRequest.Address = a;
                geocodeRequest.UserProfile = new UserProfile {DeviceType = DeviceType.Mobile};

                // Make the reverse geocode request
                var geocodeService = new GeocodeServiceClient("BasicHttpBinding_IGeocodeService");
                geocodeService.GeocodeCompleted += geocodeService_GeocodeCompleted;
                geocodeService.GeocodeAsync(geocodeRequest);
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void SetInfoText()
        {
            lblInfo_Address.Text = string.Format(_address, tbAddress1.Text, tbAddress2.Text);
            lblInfo_FullName.Text = string.Format(_fullName, tbFullName.Text);
            lblInfo_CityStateZip.Text = string.Format(_cityStateZip, tbCity.Text, tbDistrict.Text, tbZipCode.Text);
            lblInfo_Age.Text = string.Format("{0} year old {1}", dlsAge.SelectedItem, lpGender.SelectedItem);
            lblInfo_telephone.Text = string.Format(_phNum, tbPhoneNumber.Text);
            if (!string.IsNullOrEmpty(tbAddress1.Text) && !string.IsNullOrEmpty(tbCity.Text) && !string.IsNullOrEmpty(tbDistrict.Text))
                MakeGeocodeRequest(new Address {AddressLine = tbAddress1.Text, AdminDistrict = tbDistrict.Text, Locality = tbCity.Text, PostalCode = tbZipCode.Text, CountryRegion = _currentBingGeocodeLocation != null ? _currentBingGeocodeLocation.Address.CountryRegion : _currentReturnVisitData.Country });
        }

        private void bAddVisit_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ModifyCall.xaml", UriKind.Relative));
        }
    }
}