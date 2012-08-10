using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;
using System.Device.Location;
using MyTime.BingMapsGeocodeService;

namespace MyTime
{
	public partial class AddNewRV : PhoneApplicationPage
	{
		public class States
		{
			public string Text {get; set;}
		}

		System.Device.Location.GeoCoordinateWatcher gcw;

		string _address, _fullName, _cityStateZip;
		public AddNewRV()
		{
			InitializeComponent();
			pbGetAddress.IsIndeterminate = true;
			_address = lblInfo_Address.Text;
			_fullName = lblInfo_FullName.Text;
			_cityStateZip = lblInfo_CityStateZip.Text;
			SetInfoText();
		}

		private void textBlock1_LostFocus(object sender, RoutedEventArgs e)
		{

		}

		private void mapInfo_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
		{
			Pushpin p = new Pushpin();
			p.Location = e.Position.Location;
			mapInfo.Children.Clear();
			mapInfo.Children.Add(p);
			mapInfo.SetView(e.Position.Location, 12);
		}

		private void MakeReverseGeocodeRequest()
		{
			string Results = "";
			try {
				// Set a Bing Maps key before making a request
				string key = "AkRhgqPR6aujo-xib-KiR8Lt20wsn89GY4R9SP0RA6h4w7QT9mS3kKwYKKxjklfV";

				var reverseGeocodeRequest = new BingMapsGeocodeService.ReverseGeocodeRequest();

				// Set the credentials using a valid Bing Maps key
				reverseGeocodeRequest.Credentials = new GeocodeService.Credentials();
				reverseGeocodeRequest.Credentials.ApplicationId = key;

				// Set the point to use to find a matching address
				BingMapsGeocodeService.GeocodeLocation.Location point = new GeocodeService.Location();
				point.Latitude = 47.608;
				point.Longitude = -122.337;

				reverseGeocodeRequest.Location = point;

				// Make the reverse geocode request
				GeocodeService.GeocodeServiceClient geocodeService =
				new GeocodeService.GeocodeServiceClient("BasicHttpBinding_IGeocodeService");
				GeocodeService.GeocodeResponse geocodeResponse = geocodeService.ReverseGeocode(reverseGeocodeRequest);

				Results = geocodeResponse.Results[0].DisplayName;

			} catch (Exception ex) {
				Results = "An exception occurred: " + ex.Message;

			}

		}


		private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
		{
			gcw = new System.Device.Location.GeoCoordinateWatcher();
			mapInfo.SetView(new GeoCoordinate(40, -95), 4);
			gcw.PositionChanged += mapInfo_PositionChanged;
			gcw.Start(true);
			List<States> states = new List<States>();
			states.Add(new States() { Text = "Alabama" });
			states.Add(new States() { Text = "Alaska" });
			states.Add(new States() { Text = "American Samoa" });
			states.Add(new States() { Text = "Arizona" });
			states.Add(new States() { Text = "Arkansas" });
			states.Add(new States() { Text = "California" });
			states.Add(new States() { Text = "Colorado" });
			states.Add(new States() { Text = "Connecticut" });
			states.Add(new States() { Text = "Delaware" });
			states.Add(new States() { Text = "District of Columbia" });
			states.Add(new States() { Text = "Florida" });
			states.Add(new States() { Text = "Georgia" });
			states.Add(new States() { Text = "Guam" });
			states.Add(new States() { Text = "Hawaii" });
			states.Add(new States() { Text = "Idaho" });
			states.Add(new States() { Text = "Illinois" });
			states.Add(new States() { Text = "Indiana" });
			states.Add(new States() { Text = "Iowa" });
			states.Add(new States() { Text = "Kansas" });
			states.Add(new States() { Text = "Kentucky" });
			states.Add(new States() { Text = "Louisiana" });
			states.Add(new States() { Text = "Maine" });
			states.Add(new States() { Text = "Maryland" });
			states.Add(new States() { Text = "Massachusetts" });
			states.Add(new States() { Text = "Michigan" });
			states.Add(new States() { Text = "Minnesota" });
			states.Add(new States() { Text = "Mississippi" });
			states.Add(new States() { Text = "Missouri" });
			states.Add(new States() { Text = "Montana" });
			states.Add(new States() { Text = "Nebraska" });
			states.Add(new States() { Text = "Nevada" });
			states.Add(new States() { Text = "New Hampshire" });
			states.Add(new States() { Text = "New Jersey" });
			states.Add(new States() { Text = "New Mexico" });
			states.Add(new States() { Text = "New York" });
			states.Add(new States() { Text = "North Carolina" });
			states.Add(new States() { Text = "North Dakota" });
			states.Add(new States() { Text = "Northern Marianas Islands" });
			states.Add(new States() { Text = "Ohio" });
			states.Add(new States() { Text = "Oklahoma" });
			states.Add(new States() { Text = "Oregon" });
			states.Add(new States() { Text = "Pennsylvania" });
			states.Add(new States() { Text = "Puerto Rico" });
			states.Add(new States() { Text = "Rhode Island" });
			states.Add(new States() { Text = "South Carolina" });
			states.Add(new States() { Text = "South Dakota" });
			states.Add(new States() { Text = "Tennessee" });
			states.Add(new States() { Text = "Texas" });
			states.Add(new States() { Text = "Utah" });
			states.Add(new States() { Text = "Vermont" });
			states.Add(new States() { Text = "Virginia" });
			states.Add(new States() { Text = "Virgin Islands" });
			states.Add(new States() { Text = "Washington" });
			states.Add(new States() { Text = "West Virginia" });
			states.Add(new States() { Text = "Wisconsin" });
			states.Add(new States() { Text = "Wyoming" });
			lbiState.ItemsSource = states;
		}

		private void SetInfoText()
		{
			lblInfo_Address.Text = string.Format(_address, tbAddress1.Text, tbAddress2.Text);
			lblInfo_FullName.Text = string.Format(_fullName, tbFullName.Text);
			lblInfo_CityStateZip.Text = string.Format(_cityStateZip, tbCity.Text, lbiState.SelectedIndex >= 0 ? ((States)lbiState.Items[lbiState.SelectedIndex]).Text : string.Empty, tbZipCode.Text);
		}

		private void tbFullName_LostFocus(object sender, RoutedEventArgs e)
		{
			SetInfoText();
		}

		private void tbAddress1_LostFocus(object sender, RoutedEventArgs e)
		{
			SetInfoText();
		}

		private void tbAddress2_LostFocus(object sender, RoutedEventArgs e)
		{
			SetInfoText();
		}

		private void tbCity_TextChanged(object sender, TextChangedEventArgs e)
		{
			SetInfoText();
		}

		private void lbiState_LostFocus(object sender, RoutedEventArgs e)
		{
			SetInfoText();
		}

		private void tbZipCode_LostFocus(object sender, RoutedEventArgs e)
		{
			SetInfoText();
		}

		private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SetInfoText();
		}

		private void appbar_save_Click(object sender, EventArgs e)
		{

		}

		private void miAddPicture_Click(object sender, EventArgs e)
		{

		}

		private void appbar_cancel_Click(object sender, EventArgs e)
		{
			NavigationService.GoBack();
		}

		private void miGetDirection_Click(object sender, EventArgs e)
		{

		}
	}
}