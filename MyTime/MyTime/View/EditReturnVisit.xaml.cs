using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Device.Location;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Windows.Foundation.Metadata;
using FieldService.BingMapsGeocodeService;
using FieldService.Model;
using FieldService.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Controls.Maps.Platform;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using MyTimeDatabaseLib;
using Telerik.Windows.Controls;

namespace FieldService
{
        public partial class EditReturnVisit : PhoneApplicationPage
        {
                private Address _currentAddress;
                private GeoCoordinateWatcher _gcw;

                public EditReturnVisit()
                {
                        DataContext = App.ViewModel;
                        InitializeComponent();
                }

                #region Events

                private void Button_Click_1(object sender, RoutedEventArgs e)
                {
                        if (_currentAddress != null)
                                switch (_currentAddress.CountryRegion.ToLower()) {
                                        case "united kingdom":
                                                App.ViewModel.ReturnVisitData.Address1 = _currentAddress.AddressLine;
                                                App.ViewModel.ReturnVisitData.Address2 = string.Empty;
                                                App.ViewModel.ReturnVisitData.City = _currentAddress.Locality;
                                                App.ViewModel.ReturnVisitData.StateProvinceDistrict = string.Empty;
                                                App.ViewModel.ReturnVisitData.PostalCode = _currentAddress.PostalCode;
                                                App.ViewModel.ReturnVisitData.Country = _currentAddress.CountryRegion;
                                                break;
                                        case "united states":
                                        case "canada":
                                        case "italy":
                                        default:
                                                App.ViewModel.ReturnVisitData.Address1 = _currentAddress.AddressLine;
                                                App.ViewModel.ReturnVisitData.Address2 = string.Empty;
                                                App.ViewModel.ReturnVisitData.City = _currentAddress.Locality;
                                                App.ViewModel.ReturnVisitData.StateProvinceDistrict = _currentAddress.AdminDistrict;
                                                App.ViewModel.ReturnVisitData.PostalCode = _currentAddress.PostalCode;
                                                App.ViewModel.ReturnVisitData.Country = _currentAddress.CountryRegion;
                                                break;
                                }
                }

                private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
                {
                        var bwPageLoaded = new BackgroundWorker();
                        bwPageLoaded.RunWorkerCompleted += bwPageLoaded_RunWorkerCompleted;
                        bwPageLoaded.RunWorkerAsync();


                        var delete = ApplicationBar.MenuItems[0] as ApplicationBarMenuItem;
                        var email = ApplicationBar.MenuItems[1] as ApplicationBarMenuItem;
                        var export = ApplicationBar.MenuItems[2] as ApplicationBarMenuItem;

                        var save = ApplicationBar.Buttons[0] as ApplicationBarIconButton;
                        var add = ApplicationBar.Buttons[1] as ApplicationBarIconButton;
                        var clean = ApplicationBar.Buttons[2] as ApplicationBarIconButton;

                        if (delete != null) {
                                delete.Text = StringResources.RVPage_Menu_Delete;
                        }
                        if (email != null) {
                                email.Text = StringResources.RVPage_Menu_Email;
                        }
                        if (export != null) {
                                export.Text = StringResources.RVPage_Menu_Export;
                        }
                        if (save != null) {
                                save.Text = StringResources.RVPage_Menu_Save;
                        }
                        if (add != null) {
                                add.Text = StringResources.RVPage_Menu_AddVisit;
                        }
                        if (clean != null) {
                                clean.Text = StringResources.RVPage_Menu_Reset;
                        }
                }

                protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
                {
                        base.OnNavigatedTo(e);

                        if (!NavigationContext.QueryString.ContainsKey("id")) return;
                        var bw = new BackgroundWorker();
                        bw.RunWorkerCompleted += (s, x) => {
                                int id;
                                if (!int.TryParse(NavigationContext.QueryString["id"], out id)) return;

                                App.ViewModel.ReturnVisitData.ItemId = id;
                                MakeGeocodeRequest(App.ViewModel.ReturnVisitData.GeocodeAddress);
                                if (id == App.ViewModel.ReturnVisitData.ItemId) {
                                        App.ViewModel.ReturnVisitData.LoadPreviousVisits(id);
                                }


                                RefreshRemindersList();
                        };
                        bw.RunWorkerAsync();
                }

                private void RefreshRemindersList()
                {

                        var reminders = ScheduledActionService.GetActions<Microsoft.Phone.Scheduler.Reminder>();
                        foreach (var r in reminders) if (r.BeginTime < DateTime.Now) ScheduledActionService.Remove(r.Name);
                        var rr = ScheduledActionService.GetActions<Microsoft.Phone.Scheduler.Reminder>().ToList();

                        var s = string.Format("Field Service Reminder ({0})", App.ViewModel.ReturnVisitData.FullName);
                        var reminderList = new List<Microsoft.Phone.Scheduler.Reminder>();
                        foreach (var r in rr) {
                                if (r.Title == s) reminderList.Add(r);
                        }
                        lbReminders.ItemsSource = reminderList;
                }

                protected override void OnBackKeyPress(CancelEventArgs e)
                {
                        base.OnBackKeyPress(e);
                        CleanView();
                }


                private void Pivot_LoadedPivotItem_1(object sender, PivotItemEventArgs e)
                {
                        ApplicationBar.Mode = ApplicationBarMode.Default;
                        string hdr = e.Item.Header.ToString();
                        switch (hdr) {
                                case "info":
                                        break;
                                case "personal":
                                        break;
                                case "address":
                                        break;
                                case "visits":
                                        break;
                                case "reminders":
                                        ApplicationBar.Mode = ApplicationBarMode.Minimized;
                                        break;
                        }
                        if (App.ViewModel.ReturnVisitData.IsAddressValid) {
                                MakeGeocodeRequest(App.ViewModel.ReturnVisitData.GeocodeAddress);
                        }
                }

                private void _gcw_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
                {
                        if (e.Position == null || e.Position.Location == null) return;
                        MakeReverseGeocodeRequest(e.Position.Location);
                }

                private void bwPageLoaded_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
                {
                        _gcw = new GeoCoordinateWatcher(GeoPositionAccuracy.High) { MovementThreshold = 50 };
                        _gcw.PositionChanged += _gcw_PositionChanged;
                        _gcw.Start(true);
                }

                /// <summary>
                /// Handles the GeocodeCompleted event of the geocodeService control.
                /// </summary>
                /// <param name="sender">The source of the event.</param>
                /// <param name="e">The <see cref="GeocodeCompletedEventArgs" /> instance containing the event data.</param>
                private void geocodeService_GeocodeCompleted(object sender, GeocodeCompletedEventArgs e)
                {
                        if (e.Result == null || e.Result.Results == null || e.Result.Results.Count == 0) return;
                        var p = new Pushpin { Location = e.Result.Results[0].Locations[0] };
                        mapInfo.Children.Clear();
                        mapInfo.Children.Add(p);
                        mapInfo.SetView(p.Location, 15);
                }


                private void lookupAddress_ReverseGeocodeCompleted(object sender, ReverseGeocodeCompletedEventArgs e)
                {
                        //throw new NotImplementedException();
                        if (e.Result == null || e.Result.Results == null || !e.Result.Results.Any()) return;
                        Address found = null;
                        foreach (GeocodeResult l in e.Result.Results) {
                                if (l.Confidence == Confidence.High) {
                                        found = l.Address;
                                        _gcw.Stop();
                                        break;
                                }
                        }

                        if (found == null) {
                                foreach (GeocodeResult l in e.Result.Results) {
                                        if (l.Confidence == Confidence.Medium) {
                                                found = l.Address;
                                                break;
                                        }
                                }
                        }
                        try {
                                if (found == null) found = e.Result.Results.FirstOrDefault().Address;
                        } catch {
                                return;
                        }

                        SetCurrentAddress(found);
                }

                #endregion

                private void CleanView()
                {
                        App.ViewModel.ReturnVisitData.ItemId = -1;
                        mapInfo.Children.Clear();
                        mapInfo.SetView(new GeoCoordinate(0, 0), 0);
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
                                var reverseGeocodeRequest = new ReverseGeocodeRequest { Credentials = new Credentials { ApplicationId = KEY } };

                                Location point = l;

                                reverseGeocodeRequest.Location = point;
                                reverseGeocodeRequest.UserProfile = new UserProfile { DeviceType = DeviceType.Mobile };

                                // Make the reverse geocode request
                                var geocodeService = new GeocodeServiceClient("BasicHttpBinding_IGeocodeService");
                                geocodeService.ReverseGeocodeCompleted += lookupAddress_ReverseGeocodeCompleted;
                                geocodeService.ReverseGeocodeAsync(reverseGeocodeRequest);
                        } catch (Exception ex) {
                                MessageBox.Show(ex.Message);
                        }
                }

                private void SetCurrentAddress(Address a)
                {
                        _currentAddress = a;
                        var country = ReturnVisitViewModel.GetCountryCode(a.CountryRegion.ToLower());
                        switch (country) {
                                case "it":
                                        tbCurrentAddress.Text = a.AddressLine;
                                        tbCurrentCityStateZip.Text = string.Format("{2} {0} {1}", a.Locality, a.AdminDistrict, a.PostalCode);
                                        tbCurrentCountry.Text = a.CountryRegion;
                                        break;
                                case "gb":
                                        tbCurrentAddress.Text = a.AddressLine;
                                        tbCurrentCityStateZip.Text = string.Format("{0} {1}", a.Locality, a.PostalCode);
                                        tbCurrentCountry.Text = a.CountryRegion;
                                        break;
                                case "th":
                                        tbCurrentAddress.Text = a.AddressLine;
                                        tbCurrentCityStateZip.Text = string.Format("{0} {1}", a.Locality, a.AdminDistrict);
                                        tbCurrentCountry.Text = a.CountryRegion;
                                        break;
                                case "us":
                                default:
                                        tbCurrentAddress.Text = a.AddressLine;
                                        tbCurrentCityStateZip.Text = string.Format("{0}, {1} {2}", a.Locality, a.AdminDistrict, a.PostalCode);
                                        tbCurrentCountry.Text = a.CountryRegion;
                                        break;
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
                                var geocodeRequest = new GeocodeRequest { Credentials = new Credentials { ApplicationId = KEY }, Address = a, UserProfile = new UserProfile { DeviceType = DeviceType.Mobile } };

                                // Make the reverse geocode request
                                var geocodeService = new GeocodeServiceClient("BasicHttpBinding_IGeocodeService");
                                geocodeService.GeocodeCompleted += geocodeService_GeocodeCompleted;
                                geocodeService.GeocodeAsync(geocodeRequest);
                        } catch (Exception ex) {
                                MessageBox.Show(ex.Message);
                        }
                }

                private void ResetCleanApplicationBarButton_Click(object sender, EventArgs e)
                {
                        CleanView();
                }

                private void PreviousVisitListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
                {
                        var lb = sender as ListBox;
                        if (lb == null) return;

                        if (lb.SelectedIndex < 0) return;

                        var previousVisit = lb.SelectedItem as PreviousVisitViewModel;
                        if (previousVisit == null) {
                                lb.SelectedIndex = -1;
                                return;
                        }

                        NavigationService.Navigate(new Uri(string.Format("/View/ViewPreviousVisit.xaml?id={0}&rvid={1}", previousVisit.PreviousVisitItemId, App.ViewModel.ReturnVisitData.ItemId), UriKind.Relative));
                        lb.SelectedIndex = -1;
                }

                private void appbar_save_Click_1(object sender, EventArgs e)
                {
                        UpdateViewModel();
                        if (!App.ViewModel.ReturnVisitData.IsAddressValid) {
                                App.ToastMe("Please fill in address before saving.");
                                return;
                        }
                        if (App.ViewModel.ReturnVisitData.SaveOrUpdate()) {
                                App.ToastMe("Return Visit Saved.");
                                NavigationContext.QueryString["id"] = App.ViewModel.ReturnVisitData.ItemId.ToString();
                        } else {
                                App.ToastMe("Return Visit Saving Failed.");
                        }
                }

                private void UpdateViewModel()
                {
                        if (FocusManager.GetFocusedElement() is RadTextBox) {
                                var tb = FocusManager.GetFocusedElement() as RadTextBox;
                                if (tb != null) {
                                        tb.GetBindingExpression(RadTextBox.TextProperty).UpdateSource();
                                }
                        }
                        var wb = new WriteableBitmap(450, 250);
                        //var t = new TranslateTransform {X = -((mapInfo.Width/2) - 50), Y = -((mapInfo.Height/2) - 50)};
                        wb.Render(mapInfo, new TranslateTransform());
                        wb.Invalidate();

                        App.ViewModel.ReturnVisitData.IntImage = wb.Pixels;
                }

                private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
                {
                        if (App.ViewModel.ReturnVisitData.ItemId < 0) {
                                App.ToastMe("Save RV before adding call.");
                                return;
                        }
                        NavigationService.Navigate(new Uri(string.Format("/View/PreviousCall.xaml?rvid={0}", App.ViewModel.ReturnVisitData.ItemId), UriKind.Relative));
                }

                private void appbar_delete_Click_1(object sender, EventArgs e)
                {
                        if (App.ViewModel.ReturnVisitData.ItemId < 0) return;
                        if (MessageBox.Show("Are you sure you want to delete the return visit?", "Field Service", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel) return;
                        try {
                                var deleteCalls = bool.Parse(App.AppSettingsProvider["deleteCallsAndRV"].Value);
                                App.ToastMe(App.ViewModel.ReturnVisitData.Delete(deleteCalls) ? "RV Deleted." : "RV Delete Failed.");
                        } catch {
                                App.ToastMe(App.ViewModel.ReturnVisitData.Delete(MessageBox.Show("Do you want to delete the return visit's calls?") == MessageBoxResult.OK) ? "RV Deleted." : "RV Delete Failed.");
                        }
                }

                private void miShareContact_Click_1(object sender, EventArgs e)
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
                                                                                App.ViewModel.ReturnVisitData.FullName,
                                                                                App.ViewModel.ReturnVisitData.PhoneNumber,
                                                                                App.ViewModel.ReturnVisitData.Address1,
                                                                                App.ViewModel.ReturnVisitData.Address2,
                                                                                App.ViewModel.ReturnVisitData.City,
                                                                                App.ViewModel.ReturnVisitData.StateProvinceDistrict,
                                                                                App.ViewModel.ReturnVisitData.PostalCode,
                                                                                App.ViewModel.ReturnVisitData.Country,
                                                                                App.ViewModel.ReturnVisitData.LocationNotes,
                                                                                App.ViewModel.ReturnVisitData.Age,
                                                                                App.ViewModel.ReturnVisitData.Gender,
                                                                                App.ViewModel.ReturnVisitData.PhysicalDescription);

                        if (MessageBox.Show("Include All Visits?", "Field Service", MessageBoxButton.OKCancel) == MessageBoxResult.OK) {
                                body += GetVisitsListAsString();
                        }
                        body += "\n\n";

                        var emailcomposer = new EmailComposeTask { Subject = "Return Visit", Body = body };
                        emailcomposer.Show();
                }

                /// <summary>
                /// Gets the visits list as string.
                /// </summary>
                /// <returns>System.String.</returns>
                private string GetVisitsListAsString()
                {
                        string body = string.Empty;
                        body += "\n\nVisits:\n";
                        return App.ViewModel.ReturnVisitData.PreviousVisits.Where(pp => pp != null).Select(pp => RvPreviousVisitsDataInterface.GetCall(pp.PreviousVisitItemId)).Where(pv => pv != null).Aggregate(body, (current, pv) => current + string.Format("Date: {0}\n" + "Placements: {1} Mg's, {2} Bk's, {3} Br's\n" + "Notes: {4}\n\n\n", pv.Date.ToShortDateString(), pv.Magazines, pv.Books, pv.Brochures, pv.Notes));
                }

                private void miCreateContact_Click(object sender, EventArgs e)
                {
                        var newContact = new SaveContactTask();
                        newContact.FirstName = App.ViewModel.ReturnVisitData.FullName;
                        newContact.HomeAddressCity = App.ViewModel.ReturnVisitData.City;
                        newContact.HomeAddressCountry = App.ViewModel.ReturnVisitData.Country;
                        newContact.HomeAddressState = App.ViewModel.ReturnVisitData.StateProvinceDistrict;
                        newContact.HomeAddressStreet = string.Format("{0} {1}", App.ViewModel.ReturnVisitData.Address1, App.ViewModel.ReturnVisitData.Address2);
                        newContact.HomeAddressZipCode = App.ViewModel.ReturnVisitData.PostalCode;
                        newContact.HomePhone = App.ViewModel.ReturnVisitData.PhoneNumber;
                        newContact.Notes = string.Format("Age: {0}\nGender: {1}\nPhysical Description: {2}\nOther Notes: {3}",
                                                                                         App.ViewModel.ReturnVisitData.Age,
                                                                                         App.ViewModel.ReturnVisitData.Gender,
                                                                                         App.ViewModel.ReturnVisitData.PhysicalDescription,
                                                                                         App.ViewModel.ReturnVisitData.LocationNotes);

                        if (MessageBox.Show("Do you want to include all visits?", "FIELD SERVICE", MessageBoxButton.OKCancel) == MessageBoxResult.OK) {
                                newContact.Notes += GetVisitsListAsString();
                        }

                        newContact.Show();
                }

                private void ShowBingMapHyperlinkButton_Click(object sender, RoutedEventArgs e)
                {
                        var bmt = new BingMapsTask();
                        if (App.ViewModel.ReturnVisitData.IsAddressValid) {
                                bmt.SearchTerm = App.ViewModel.ReturnVisitData.FormattedAddress.Replace("\n", " ");
                                bmt.ZoomLevel = 18;
                        }
                        bmt.Center = mapInfo.Center;
                        bmt.Show();
                }

                private void PhoneNumberHyperlinkButton_Click(object sender, RoutedEventArgs e)
                {
                        var phone = new PhoneCallTask
                        {
                                PhoneNumber = App.ViewModel.ReturnVisitData.PhoneNumber
                        };
                        phone.Show();
                }

                private void AddReminderButton_Click(object sender, RoutedEventArgs e)
                {
                        var reminderDtTm = new DateTime(dpReminderDate.Value.Value.Year, dpReminderDate.Value.Value.Month, dpReminderDate.Value.Value.Day,
                                                        tpReminderTime.Value.Value.Hour, tpReminderTime.Value.Value.Minute, tpReminderTime.Value.Value.Second);
                        if (DateTime.Now > reminderDtTm) {
                                App.ToastMe("Reminder must be past right now.");
                                return;
                        }

                        var s = string.Format("Field Service Reminder ({0})", App.ViewModel.ReturnVisitData.FullName);


                        var r = new Microsoft.Phone.Scheduler.Reminder(Guid.NewGuid().ToString())
                        {
                                Content = tbReminderNotes.Text,
                                BeginTime = reminderDtTm,
                                Title = s,
                                NavigationUri = NavigationService.CurrentSource
                        };
                        ScheduledActionService.Add(r);
                        tbReminderNotes.Text = "";
                        App.ToastMe("Reminder Added.");
                        RefreshRemindersList();
                }
        }
}