using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using FieldService.BingMapsGeocodeService;
using FieldService.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Controls.Maps.Platform;
using Microsoft.Phone.Shell;
using System.ComponentModel;
using MyTimeDatabaseLib;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace FieldService.View
{
        public partial class MapCalls : PhoneApplicationPage
        {
                private ReturnVisitData[] _rvList;
                private int _index;

                public MapCalls()
                {
                        InitializeComponent();
                    ((MapCallsViewModel)DataContext).StartLocationService();

                        var bw = new BackgroundWorker();

                        bw.RunWorkerCompleted += (o, e) => {
                                _rvList = ReturnVisitsInterface.GetReturnVisits(SortOrder.DateNewestToOldest, -1);
                                for (_index = 0; _index < _rvList.Length; _index++){
                                        if (_rvList[_index].Latitude == 0 || _rvList[_index].Longitude == 0){
                                                MakeGeocodeRequest(GetGeocodeAddress(_rvList[_index]));
                                                break;
                                        }
                                    var p = new Pushpin
                                    {
                                        Location = new Location
                                        {
                                            Longitude = _rvList[_index].Longitude,
                                            Latitude = _rvList[_index].Latitude
                                        },
                                        Content = _rvList[_index]
                                    };
                                    p.DoubleTap += POnDoubleTap;
                                        mRvMaps.Children.Add(p);
                                }
                        };
                        bw.RunWorkerAsync();
                }

            private void POnDoubleTap(object sender, GestureEventArgs gestureEventArgs)
            {
                var p = sender as Pushpin;
                if (p == null) return;

                var rv = p.Content as ReturnVisitData;

                if (rv == null) return;

				NavigationService.Navigate(new Uri(string.Format("/View/EditReturnVisit.xaml?id={0}", rv.ItemId.ToString(CultureInfo.InvariantCulture)), UriKind.Relative));
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

                private Address GetGeocodeAddress(ReturnVisitData r)
                {
                        switch (GetCountryCode(r.Country)) {
                                case "gb":
                                        return new Address
                                        {
                                                AddressLine = r.AddressOne,
                                                Locality = r.City,
                                                CountryRegion = string.IsNullOrEmpty(r.Country) ? "England" : r.Country
                                        };
                                case "th":
                                        return new Address
                                        {
                                                AddressLine = string.Format("{0}{1}", r.AddressOne, string.IsNullOrEmpty(r.AddressTwo) ? string.Empty : string.Format(" {0}", r.AddressTwo)),
                                                Locality = r.City,
                                                AdminDistrict = r.StateProvince,
                                                CountryRegion = string.IsNullOrEmpty(r.Country) ? "Thailand" : r.Country
                                        };
                                case "it":
                                case "us":
                                default:
                                        return new Address
                                        {
                                                AddressLine = r.AddressOne,
                                                AdminDistrict = r.StateProvince,
                                                Locality = r.City,
                                                PostalCode = r.PostalCode,
                                                CountryRegion = string.IsNullOrEmpty(r.Country) ? "United States" : r.Country
                                        };

                        }
                }

                /// <summary>
                /// Makes the geocode request.
                /// </summary>
                /// <param name="a">A.</param>
                public void MakeGeocodeRequest(Address a)
                {
                    try
                    {
                        // Set a Bing Maps key before making a request
                        const string KEY = "AkRhgqPR6aujo-xib-KiR8Lt20wsn89GY4R9SP0RA6h4w7QT9mS3kKwYKKxjklfV";

                        // Set the credentials using a valid Bing Maps key
                        // Set the point to use to find a matching address
                        var geocodeRequest = new GeocodeRequest
                        {
                            Credentials = new Credentials {ApplicationId = KEY},
                            Address = a,
                            UserProfile = new UserProfile {DeviceType = DeviceType.Mobile}
                        };

                        // Make the reverse geocode request
                        var geocodeService = new GeocodeServiceClient("BasicHttpBinding_IGeocodeService");
                        geocodeService.GeocodeCompleted += geocodeService_GeocodeCompleted;
                        geocodeService.GeocodeAsync(geocodeRequest);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }

                /// <summary>
                /// Handles the GeocodeCompleted event of the geocodeService control.
                /// </summary>
                /// <param name="sender">The source of the event.</param>
                /// <param name="e">The <see cref="GeocodeCompletedEventArgs" /> instance containing the event data.</param>
                private void geocodeService_GeocodeCompleted(object sender, GeocodeCompletedEventArgs e)
                {
                        if (e.Result == null || e.Result.Results == null || e.Result.Results.Count == 0) return;
                        var p = new Pushpin
                        {
                            Location = e.Result.Results[0].Locations[0],
                            Content = _rvList[_index]
                        };
                    p.DoubleTap += POnDoubleTap;
                        mRvMaps.Children.Add(p);
                    _rvList[_index].Latitude = p.Location.Latitude;
                    _rvList[_index].Longitude = p.Location.Longitude;
                    ReturnVisitsInterface.AddOrUpdateRV(ref _rvList[_index]);
                    _index++;
                    if (_index < _rvList.Length)
                    {
                        MakeGeocodeRequest(GetGeocodeAddress(_rvList[_index]));
                    }
                }
        }
}