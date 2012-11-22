// ***********************************************************************
// Assembly         : MyTime
// Author           : trevo_000
// Created          : 11-03-2012
//
// Last Modified By : trevo_000
// Last Modified On : 11-11-2012
// ***********************************************************************
// <copyright file="AddNewRV.xaml.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using FieldService.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Controls.Maps.Platform;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using FieldService.BingMapsGeocodeService;
using MyTimeDatabaseLib;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace FieldService
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
            try {
                dlsAge.Text = App.AppSettingsProvider["dfltAgeValue"] == null ? "30" : App.AppSettingsProvider["dfltAgeValue"].Value;
            } catch { }
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
            SetInfoText();
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
                App.ToastMe("The Return Visit has been delete.");
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
            int rvItemId = -1;
            try {
                if (!ValidateRVData()) {
                    App.ToastMe("At a minimum you must a valid address to save.");
                    return;
                }

                var wb = new WriteableBitmap((int)mapInfo.Width, (int)mapInfo.Height);
                //var t = new TranslateTransform {X = -((mapInfo.Width/2) - 50), Y = -((mapInfo.Height/2) - 50)};
                wb.Render(mapInfo, new TranslateTransform());
                wb.Invalidate();

                newRv = new ReturnVisitData
                {
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
                if (_currentReturnVisitData != null && _currentReturnVisitData.ItemId >= 0) {
                    rvItemId = ReturnVisitsInterface.UpdateReturnVisit(_currentReturnVisitData.ItemId, newRv);
                } else {
                    rvItemId = ReturnVisitsInterface.AddNewReturnVisit(newRv);
                }
                App.ToastMe(String.Format("Return Visit {0}.", _currentReturnVisitData == null ? "Added" : "Saved"));
            } catch (ReturnVisitAlreadyExistsException ee) {
                if (_currentReturnVisitData != null && newRv != null) {
                    ReturnVisitsInterface.UpdateReturnVisit(_currentReturnVisitData.ItemId, newRv);
                    App.ToastMe("Return Visit Saved.");
                } else {
                    MessageBox.Show("Can't Add.\n\nException:\n" + ee.Message);
                }
            } catch (Exception ee) {
                MessageBox.Show("Can't Add.\n\nException:\n" + ee.Message);
            } finally {
                if (rvItemId >= 0)
                    _currentReturnVisitData = ReturnVisitsInterface.GetReturnVisit(rvItemId);
            }
        }

        /// <summary>
        /// Handles the Click event of the bAddVisit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void bAddVisit_Click(object sender, RoutedEventArgs e)
        {
            if (_currentReturnVisitData == null || _currentReturnVisitData.ItemId < 0) {
                App.ToastMe("Please save this contact record first.");
                return;
            }
            NavigationService.Navigate(new Uri(String.Format("/ModifyCall.xaml?rvid={0}", _currentReturnVisitData == null ? "-1" : _currentReturnVisitData.ItemId.ToString(CultureInfo.InvariantCulture)), UriKind.Relative));
        }

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
                var reminder = new Reminder(reminderName) {Title = txtTitle.Text, Content = txtContent.Text};
                // in contrast to alarms where setting the Title property is not supported

                if (dpDatePicker.Value != null) {
                    var remindNow = new DateTime(dpDatePicker.Value.Value.Year, dpDatePicker.Value.Value.Month, dpDatePicker.Value.Value.Day, tpStartTime.Value.Value.Hour, tpStartTime.Value.Value.Minute, tpStartTime.Value.Value.Second);

                    TimeSpan span = remindNow - DateTime.Now;

                    reminder.BeginTime = DateTime.Now.AddSeconds(span.TotalSeconds);
                }

                reminder.ExpirationTime = reminder.BeginTime.AddSeconds(5.0);
                reminder.RecurrenceType = RecurrenceInterval.None;

                ScheduledActionService.Add(reminder);

                App.ToastMe(String.Format("Reminder: '{0}' added.", txtTitle.Text));

                txtTitle.Text = String.Empty;
                txtContent.Text = String.Empty;
                dpDatePicker.Value = DateTime.Now;
                tpStartTime.Value = DateTime.Now;
            } catch (Exception ee) {
                App.ToastMe("Could Not Add Reminder.");
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
            if (e.Result == null || e.Result.Results == null || e.Result.Results.Count == 0) return;
            var p = new Pushpin {Location = e.Result.Results[0].Locations[0]};
            mapInfo.Children.Clear();
            mapInfo.Children.Add(p);
            mapInfo.SetView(p.Location, 15);
        }

        /// <summary>
        /// Handles the ReverseGeocodeCompleted event of the geocodeService control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ReverseGeocodeCompletedEventArgs" /> instance containing the event data.</param>
        private void geocodeService_ReverseGeocodeCompleted(object sender, ReverseGeocodeCompletedEventArgs e)
        {
            if (e.Result == null || e.Result.Results == null || e.Result.Results.Count == 0) return;
            GeocodeResult results = e.Result.Results[0];
            if (results == null) return;
            foreach (GeocodeResult r in e.Result.Results) {
                if (r.Confidence == Confidence.High) results = r;
            }

            lblCurrentAddress.Text = String.Format("{0}", results.Address.AddressLine);
            lblCurrentCityStateZip.Text = String.Format("{0}, {1} {2}", results.Address.Locality, results.Address.AdminDistrict, results.Address.PostalCode);
            _currentBingGeocodeLocation = results;
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
                                                             _currentReturnVisitData == null ? "-1" : _currentReturnVisitData.ItemId.ToString(CultureInfo.InvariantCulture),
                                                             call.ItemId),
                                               UriKind.Relative));
            } catch (Exception) {} finally {
                lbRvPrevItems.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Handles the Click event of the lblInfo_telephone control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void lblInfo_telephone_Click(object sender, RoutedEventArgs e)
        {
            var phone = new PhoneCallTask {
                                              PhoneNumber = tbPhoneNumber.Text
                                          };
            phone.Show();
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
        /// Handles the Tap event of the mapInfo control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void mapInfo_Tap(object sender, RoutedEventArgs e)
        {
            var bmt = new BingMapsTask();
            bmt.SearchTerm = lblInfo_AddressFull.Content.ToString().Replace("\n", " ");
            bmt.Center = mapInfo.Center;
            bmt.ZoomLevel = 18;
            bmt.Show();
        }

        /// <summary>
        /// Handles the Click event of the miCreateContact control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void miExportContact_Click(object sender, EventArgs e)
        {
            //
            //
            var newContact = new SaveContactTask();
            newContact.FirstName = tbFullName.Text;
            newContact.HomeAddressCity = tbCity.Text;
            newContact.HomeAddressCountry = tbCountry.Text;
            newContact.HomeAddressState = tbDistrict.Text;
            newContact.HomeAddressStreet = string.Format("{0} {1}", tbAddress1.Text, tbAddress2.Text);
            newContact.HomeAddressZipCode = tbZipCode.Text;
            newContact.HomePhone = tbPhoneNumber.Text;
            newContact.Notes = string.Format("Age: {0}\nGender: {1}\nPhysical Description: {2}\nOther Notes: {3}",
                                             dlsAge.Text,
                                             lpGender.SelectedItem,
                                             tbDescription.Text,
                                             tbOtherNotes.Text);

            if (MessageBox.Show("Do you want to include all visits?", "FIELD SERVICE", MessageBoxButton.OKCancel) == MessageBoxResult.OK) {
                newContact.Notes += GetVisitsListAsString();
            }

            newContact.Show();
        }

        /// <summary>
        /// Handles the Click event of the miShareContact control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void miShareContact_Click(object sender, EventArgs e)
        {
            string body = string.Format("Name: {0}\n" +
                                        "Phone Number: {1}\n" +
                                        "Address 1: {2}\n" +
                                        "Address 2: {3}\n" +
                                        "City/State/Zip: {4}, {5} {6}\n" +
                                        "Country: {7}\n" +
                                        "Loc Description: {8}\n\n" +
                                        "Age: {9}\n" +
                                        "Gender: {10}\n" +
                                        "Physical Description: {11}\n",
                                        tbFullName.Text,
                                        BeautifyPhoneNumber(tbPhoneNumber.Text),
                                        tbAddress1.Text,
                                        tbAddress2.Text,
                                        tbCity.Text, tbDistrict.Text, tbZipCode.Text,
                                        tbCountry.Text,
                                        tbOtherNotes.Text,
                                        dlsAge.Text,
                                        lpGender.SelectedItem,
                                        tbDescription.Text);

            if (MessageBox.Show("Include All Visits?", "Field Service", MessageBoxButton.OKCancel) == MessageBoxResult.OK) {
                body += GetVisitsListAsString();
            }
            body += "\n\n";

            var emailcomposer = new EmailComposeTask {Subject = "Return Visit", Body = body};
            emailcomposer.Show();
        }


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
        /// Gets the visits list as string.
        /// </summary>
        /// <returns>System.String.</returns>
        private string GetVisitsListAsString()
        {
            string body = string.Empty;
            body += "\n\nVisits:\n";
            foreach (object p in lbRvPrevItems.Items) {
                var pp = p as PreviousVisitViewModel;
                if (pp != null) {
                    RvPreviousVisitData pv = RvPreviousVisitsDataInterface.GetCall(pp.ItemId);
                    if (pv != null)
                        body += string.Format("Date: {0}\n" +
                                              "Placements: {1} Mg's, {2} Bk's, {3} Br's\n" +
                                              "Notes: {4}\n\n\n",
                                              pv.Date.ToShortDateString(),
                                              pv.Magazines, pv.Books, pv.Brochures,
                                              pv.Notes);
                }
            }
            return body;
        }

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
                int itemID = Int32.Parse(id);
                ReturnVisitData rv = ReturnVisitsInterface.GetReturnVisit(itemID);
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
            } catch (Exception) {}
        }

        /// <summary>
        /// Refreshes the visit list.
        /// </summary>
        private void RefreshVisitList()
        {
            if(App.ViewModel.IsPreviousVisitsLoaded) App.ViewModel.lbRvPreviousItems.Clear();
            if (_currentReturnVisitData == null) return;

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
                var geocodeRequest = new GeocodeRequest {Credentials = new Credentials {ApplicationId = KEY}, Address = a, UserProfile = new UserProfile {DeviceType = DeviceType.Mobile}};

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
            lblInfo_AddressFull.Content = String.Format(_address, tbAddress1.Text, tbAddress2.Text, tbCity.Text, tbDistrict.Text, tbZipCode.Text, "\n");
            lblInfo_FullName.Text = String.Format(_fullName, tbFullName.Text);
            lblInfo_Age.Text = String.Format("{0} year old {1}", dlsAge.Text, lpGender.SelectedItem);
            lblInfo_telephone.Content = BeautifyPhoneNumber(tbPhoneNumber.Text);
            if (!String.IsNullOrEmpty(tbAddress1.Text) && !String.IsNullOrEmpty(tbCity.Text) && !String.IsNullOrEmpty(tbDistrict.Text))
                MakeGeocodeRequest(new Address {AddressLine = tbAddress1.Text, AdminDistrict = tbDistrict.Text, Locality = tbCity.Text, PostalCode = tbZipCode.Text, CountryRegion = _currentBingGeocodeLocation != null ? _currentBingGeocodeLocation.Address.CountryRegion : _currentReturnVisitData.Country});
        }

        /// <summary>
        /// Beautifies the phone number.
        /// </summary>
        /// <param name="phNum">The ph num.</param>
        /// <returns>System.String.</returns>
        private string BeautifyPhoneNumber(string phNum)
        {
            bool beautifyPhoneNumber = true;
            string beautifyRegEx = @"^(\d{1})?(\d{3})(\d{3})(\d{4})$";
            string beautifyMask = @"$1($2) $3-$4";

            try {
                beautifyPhoneNumber = bool.Parse(App.AppSettingsProvider["beautifyPhoneNumber"].Value);
                beautifyRegEx = App.AppSettingsProvider["beautifyPhNumRegEx"].Value;
                beautifyMask = App.AppSettingsProvider["beautifyPhNumMask"].Value;
            } catch {
                return phNum;
            }

            if (!beautifyPhoneNumber) return phNum;

            string ss = String.Empty;
            foreach (char s in phNum) {
                if (Char.IsDigit(s)) ss += s;
            }
            if (ss.Length == 10 || ss.Length == 11) {
                return Regex.Replace(ss, beautifyRegEx, beautifyMask);
            }

            return phNum;
        }
    }
}