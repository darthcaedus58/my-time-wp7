// ***********************************************************************
// Assembly         : MyTime
// Author           : trevo_000
// Created          : 11-03-2012
//
// Last Modified By : trevo_000
// Last Modified On : 11-05-2012
// ***********************************************************************
// <copyright file="AddNewRV.xaml.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Coding4Fun.Phone.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Controls.Maps.Platform;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using MyTime.BingMapsGeocodeService;
using MyTimeDatabaseLib;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace MyTime
{
    /// <summary>
    /// Class AddNewRV
    /// </summary>
    public partial class AddNewRV : PhoneApplicationPage
    {
        /// <summary>
        /// The _address
        /// </summary>
        private readonly string _address;

        /// <summary>
        /// The _age
        /// </summary>
        private readonly string _age;

        /// <summary>
        /// The _full name
        /// </summary>
        private readonly string _fullName;

        /// <summary>
        /// The _current bing geocode location
        /// </summary>
        private GeocodeResult _currentBingGeocodeLocation;

        /// <summary>
        /// The _current return visit data
        /// </summary>
        private ReturnVisitData _currentReturnVisitData;

        /// <summary>
        /// The GCW
        /// </summary>
        private GeoCoordinateWatcher _gcw;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddNewRV" /> class.
        /// </summary>
        public AddNewRV()
        {
            InitializeComponent();

            DataContext = App.ViewModel;

            pbGetAddress.IsIndeterminate = true;
            _address = lblInfo_AddressFull.Content.ToString();
            _fullName = lblInfo_FullName.Text;
            _age = lblInfo_Age.Text;
            dlsAge.Text = App.AppSettings["dfltAgeValue"] == null ? "30" : App.AppSettings["dfltAgeValue"].Value;
            string[] genders = {"Male", "Female"};
            lpGender.ItemsSource = genders;
            lpGender.SelectedIndex = 0;
            SetInfoText();
        }

        #region Events

        /// <summary>
        /// Handles the Loaded event of the PhoneApplicationPage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            _gcw = new GeoCoordinateWatcher();
            mapInfo.SetView(new GeoCoordinate(40, -95), 4);
            _gcw.PositionChanged += currentLocation_PositionChanged;
            _gcw.Start(true);
            RefreshVisitList();
        }

        /// <summary>
        /// Handles the LoadedPivotItem event of the Pivot control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PivotItemEventArgs" /> instance containing the event data.</param>
        private void Pivot_LoadedPivotItem(object sender, PivotItemEventArgs e)
        {
            Focus();
            if (e.Item.Header.ToString() == "reminders") {
                ApplicationBar.Mode = ApplicationBarMode.Minimized;
            } else {
                ApplicationBar.Mode = ApplicationBarMode.Default;
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the Pivot control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs" /> instance containing the event data.</param>
        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e) { SetInfoText(); }

        /// <summary>
        /// Handles the Click event of the appbar_DeleteRv control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void appbar_DeleteRv_Click(object sender, EventArgs e)
        {
            if (_currentReturnVisitData != null && MessageBox.Show("Are you sure you want to delete this return visit?", "FIELD SERVICE", MessageBoxButton.OKCancel) == MessageBoxResult.OK) {
                ReturnVisitsInterface.DeleteReturnVisit(_currentReturnVisitData.ItemId);
                App.ToastMe("The Return Visit has been delete.", "Field Service");
                //MessageBox.Show("");
                NavigationService.GoBack();
            }
        }

        /// <summary>
        /// Handles the Click event of the appbar_save control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void appbar_save_Click(object sender, EventArgs e)
        {
            ReturnVisitData newRv = null;
            try {
                if (!ValidateRVData()) {
                    App.ToastMe("At a minimum you must a valid address filled out to save an rv.");
                    return;
                }
                
                var wb = new WriteableBitmap((int)mapInfo.Width, (int)mapInfo.Height);
                //var t = new TranslateTransform {X = -((mapInfo.Width/2) - 50), Y = -((mapInfo.Height/2) - 50)};
                wb.Render(mapInfo, new TranslateTransform());
                wb.Invalidate();

                newRv = new ReturnVisitData {
                                                AddressOne = tbAddress1.Text,
                                                AddressTwo = tbAddress2.Text,
                                                City = tbCity.Text,
                                                Country = _currentBingGeocodeLocation.Address.CountryRegion,
                                                StateProvince = tbDistrict.Text,
                                                PostalCode = tbZipCode.Text,
                                                Age = dlsAge.Text,
                                                Gender = lpGender.SelectedItem.ToString(),
                                                FullName = tbFullName.Text,
                                                DateCreated = DateTime.Now,
                                                OtherNotes = tbOtherNotes.Text,
                                                PhysicalDescription = tbDescription.Text,
                                                ImageSrc = wb.Pixels,
                                                PhoneNumber = tbPhoneNumber.Text
                                            };

                ReturnVisitsInterface.AddNewReturnVisit(newRv);
                App.ToastMe(String.Format("Return Visit {0}.", _currentReturnVisitData == null ? "Added" : "Saved"));
                _currentReturnVisitData = newRv;
            } catch (ReturnVisitAlreadyExistsException ee) {
                if (_currentReturnVisitData != null && newRv != null) {
                    ReturnVisitsInterface.UpdateReturnVisit(_currentReturnVisitData.ItemId, newRv);
                    App.ToastMe("Return Visit Saved.");
                    _currentReturnVisitData = newRv;
                } else {
                    MessageBox.Show("Can't Add.\n\nException:\n" + ee.Message);
                }
            } catch (Exception ee) {
                MessageBox.Show("Can't Add.\n\nException:\n" + ee.Message);
            }
        }

        /// <summary>
        /// Handles the Click event of the bAddVisit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void bAddVisit_Click(object sender, RoutedEventArgs e) { NavigationService.Navigate(new Uri(String.Format("/ModifyCall.xaml?rvid={0}", _currentReturnVisitData == null ? "-1" : _currentReturnVisitData.ItemId.ToString()), UriKind.Relative)); }

        /// <summary>
        /// Handles the Click event of the btnDone control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            try {
                string reminderName = Guid.NewGuid().ToString();
                // use guid for reminder name, since reminder names must be unique
                var reminder = new Reminder(reminderName);
                // in contrast to alarms where setting the Title property is not supported
                reminder.Title = txtTitle.Text;
                reminder.Content = txtContent.Text;

                var remindNow = new DateTime(dpDatePicker.Value.Value.Year, dpDatePicker.Value.Value.Month, dpDatePicker.Value.Value.Day, tpStartTime.Value.Value.Hour, tpStartTime.Value.Value.Minute, tpStartTime.Value.Value.Second);

                TimeSpan span = remindNow - DateTime.Now;

                reminder.BeginTime = DateTime.Now.AddSeconds(span.TotalSeconds);

                reminder.ExpirationTime = reminder.BeginTime.AddSeconds(5.0);
                reminder.RecurrenceType = RecurrenceInterval.None;

                ScheduledActionService.Add(reminder);

                App.ToastMe(String.Format("Reminder: '{0}' added.", txtTitle.Text));

                txtTitle.Text = String.Empty;
                txtContent.Text = String.Empty;
                dpDatePicker.Value = DateTime.Now;
                tpStartTime.Value = DateTime.Now;
            } catch {
                MessageBox.Show("Could Not Add Reminder.");
            } finally {
                RefreshReminderList();
            }
        }

        /// <summary>
        /// Currents the location_ position changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void currentLocation_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            var p = new Pushpin {Location = e.Position.Location};
            mapCurrentAddress.Children.Clear();
            mapCurrentAddress.Children.Add(p);
            mapCurrentAddress.SetView(e.Position.Location, 14);
            MakeReverseGeocodeRequest(e.Position.Location);
        }

        /// <summary>
        /// Handles the GeocodeCompleted event of the geocodeService control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="GeocodeCompletedEventArgs" /> instance containing the event data.</param>
        private void geocodeService_GeocodeCompleted(object sender, GeocodeCompletedEventArgs e)
        {
            if (e.Result.Results == null || e.Result.Results.Count == 0) return;
            var p = new Pushpin {Location = e.Result.Results[0].Locations[0]};
            mapInfo.Children.Clear();
            mapInfo.Children.Add(p);
            mapInfo.SetView(p.Location, 11);
        }

        /// <summary>
        /// Handles the ReverseGeocodeCompleted event of the geocodeService control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ReverseGeocodeCompletedEventArgs" /> instance containing the event data.</param>
        private void geocodeService_ReverseGeocodeCompleted(object sender, ReverseGeocodeCompletedEventArgs e)
        {
            if (e.Result.Results == null || e.Result.Results.Count == 0) return;
            GeocodeResult Results = e.Result.Results[0];
            foreach (GeocodeResult r in e.Result.Results) {
                if (r.Confidence == Confidence.High) Results = r;
            }

            lblCurrentAddress.Text = String.Format("{0}", Results.Address.AddressLine);
            lblCurrentCityStateZip.Text = String.Format("{0}, {1} {2}", Results.Address.Locality, Results.Address.AdminDistrict, Results.Address.PostalCode);
            _currentBingGeocodeLocation = Results;
        }

        /// <summary>
        /// Handles the SelectionChanged event of the lbRvPrevItems control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs" /> instance containing the event data.</param>
        private void lbRvPrevItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try {
                var call = (PreviousVisitViewModel) lbRvPrevItems.SelectedItem;
                NavigationService.Navigate(new Uri(
                                               String.Format("/ModifyCall.xaml?rvid={0}&id={1}",
                                                             _currentReturnVisitData == null ? "-1" : _currentReturnVisitData.ItemId.ToString(),
                                                             call.ItemId),
                                               UriKind.Relative));
            } catch {} finally {
                lbRvPrevItems.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Handles the Tap event of the mapCurrentAddress control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="GestureEventArgs" /> instance containing the event data.</param>
        private void mapCurrentAddress_Tap(object sender, GestureEventArgs e)
        {
            tbAddress1.Text = _currentBingGeocodeLocation.Address.AddressLine;
            tbCity.Text = _currentBingGeocodeLocation.Address.Locality;
            tbZipCode.Text = _currentBingGeocodeLocation.Address.PostalCode;
            tbDistrict.Text = _currentBingGeocodeLocation.Address.AdminDistrict;
        }

        /// <summary>
        /// Handles the Click event of the miAddPicture control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void miAddPicture_Click(object sender, EventArgs e) { }

        /// <summary>
        /// Handles the Click event of the miGetDirection control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void miGetDirection_Click(object sender, EventArgs e) { }


        /// <summary>
        /// Handles the LostFocus event of the tbAddress1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void tbAddress1_LostFocus(object sender, RoutedEventArgs e) { SetInfoText(); }

        /// <summary>
        /// Handles the LostFocus event of the tbAddress2 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void tbAddress2_LostFocus(object sender, RoutedEventArgs e) { SetInfoText(); }

        /// <summary>
        /// Handles the TextChanged event of the tbCity control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs" /> instance containing the event data.</param>
        private void tbCity_TextChanged(object sender, TextChangedEventArgs e) { SetInfoText(); }

        /// <summary>
        /// Handles the LostFocus event of the tbFullName control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void tbFullName_LostFocus(object sender, RoutedEventArgs e) { SetInfoText(); }

        /// <summary>
        /// Handles the LostFocus event of the tbZipCode control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void tbZipCode_LostFocus(object sender, RoutedEventArgs e) { SetInfoText(); }

        /// <summary>
        /// Handles the LostFocus event of the textBlock1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void textBlock1_LostFocus(object sender, RoutedEventArgs e) { }

        #endregion

        /// <summary>
        /// Called when a page becomes the active page in a frame.
        /// </summary>
        /// <param name="e">An object that contains the event data.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            RefreshReminderList();
            if (!NavigationContext.QueryString.ContainsKey("id")) {
                //appbar_delete.IsEnabled = false;
                return;
            }
            string id = NavigationContext.QueryString["id"];
            if (String.IsNullOrEmpty(id)) {
                //appbar_delete.IsEnabled = false;
                return;
            }
            try {
                int ItemID = Int32.Parse(id);
                ReturnVisitData rv = ReturnVisitsInterface.GetReturnVisit(ItemID);
                _currentReturnVisitData = rv;
                tbAddress1.Text = rv.AddressOne;
                tbAddress2.Text = rv.AddressTwo;
                tbCity.Text = rv.City;
                tbDistrict.Text = rv.StateProvince;
                tbFullName.Text = rv.FullName;
                tbOtherNotes.Text = rv.OtherNotes;
                tbZipCode.Text = rv.PostalCode;
                tbPhoneNumber.Text = rv.PhoneNumber;
                lpGender.SelectedItem = rv.Gender;
                dlsAge.Text = rv.Age;
                SetInfoText();
                //appbar_delete.IsEnabled = true;
            } catch {}
        }

        /// <summary>
        /// Refreshes the visit list.
        /// </summary>
        private void RefreshVisitList()
        {
            if (_currentReturnVisitData == null) return;
            App.ViewModel.lbRvPreviousItems.Clear();

            RvPreviousVisitData[] visits = RvPreviousVisitsDataInterface.GetPreviousVisits(_currentReturnVisitData.ItemId, SortOrder.DateNewestToOldest);
            App.ViewModel.UpdatePreviousVisits(visits);
        }

        /// <summary>
        /// Validates the RV data.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        private bool ValidateRVData()
        {
            if (String.IsNullOrEmpty(tbAddress1.Text) ||
                String.IsNullOrEmpty(tbCity.Text) ||
                String.IsNullOrEmpty(tbDistrict.Text)) return false;
            return true;
        }

        /// <summary>
        /// Handles the Click event of the btnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnCancel_Click(object sender, RoutedEventArgs e) { }

        /// <summary>
        /// Refreshes the reminder list.
        /// </summary>
        private void RefreshReminderList()
        {
            IEnumerable<Reminder> reminders = ScheduledActionService.GetActions<Reminder>();
            foreach (Reminder r in reminders) if (r.BeginTime < DateTime.Now) ScheduledActionService.Remove(r.Name);
            lbReminders.ItemsSource = ScheduledActionService.GetActions<Reminder>();
        }

        /// <summary>
        /// Makes the reverse geocode request.
        /// </summary>
        /// <param name="l">The l.</param>
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

        /// <summary>
        /// Makes the geocode request.
        /// </summary>
        /// <param name="a">A.</param>
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

        /// <summary>
        /// Sets the info text.
        /// </summary>
        private void SetInfoText()
        {
            lblInfo_AddressFull.Content = String.Format(_address, tbAddress1.Text, tbAddress2.Text, tbCity.Text, tbDistrict.Text, tbZipCode.Text,"\n");
            lblInfo_FullName.Text = String.Format(_fullName, tbFullName.Text);
            lblInfo_Age.Text = String.Format("{0} year old {1}", dlsAge.Text, lpGender.SelectedItem);
            lblInfo_telephone.Content = BeautifyPhoneNumber(tbPhoneNumber.Text);
            if (!String.IsNullOrEmpty(tbAddress1.Text) && !String.IsNullOrEmpty(tbCity.Text) && !String.IsNullOrEmpty(tbDistrict.Text))
                MakeGeocodeRequest(new Address {AddressLine = tbAddress1.Text, AdminDistrict = tbDistrict.Text, Locality = tbCity.Text, PostalCode = tbZipCode.Text, CountryRegion = _currentBingGeocodeLocation != null ? _currentBingGeocodeLocation.Address.CountryRegion : _currentReturnVisitData.Country});
        }

        private string BeautifyPhoneNumber(string phNum)
        {
            bool beautifyPhoneNumber = true;
            var beautifyRegEx = @"^(\d{1})?(\d{3})(\d{3})(\d{4})$";
            var beautifyMask = @"$1($2) $3-$4";

            try {
                beautifyPhoneNumber = bool.Parse(App.AppSettings["beautifyPhoneNumber"].Value);
                beautifyRegEx = App.AppSettings["beautifyPhNumRegEx"].Value;
                beautifyMask = App.AppSettings["beautifyPhNumMask"].Value;
            } catch { return phNum; }

            if (!beautifyPhoneNumber) return phNum;

            var ss = String.Empty;
            foreach (var s in phNum) {
                if (Char.IsDigit(s)) ss += s;
            }
            if (ss.Length == 10 || ss.Length == 11) {
                return Regex.Replace(ss, beautifyRegEx, beautifyMask);
            }

            return phNum;
        }

        private void lblInfo_telephone_Click(object sender, RoutedEventArgs e)
        {
            var phone = new PhoneCallTask() {
                                                PhoneNumber = tbPhoneNumber.Text
                                            };
            phone.Show();
        }

        private void mapInfo_Tap(object sender, RoutedEventArgs e)
        {
            var bmt = new BingMapsTask();
            bmt.SearchTerm = lblInfo_AddressFull.Content.ToString().Replace("\n", " ");
            bmt.Center = mapInfo.Center;
            bmt.ZoomLevel = 18;
            bmt.Show();
        }
    }
}