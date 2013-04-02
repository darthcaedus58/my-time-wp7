// ***********************************************************************
// Assembly         : MyTime
// Author           : trevo_000
// Created          : 11-07-2012
//
// Last Modified By : trevo_000
// Last Modified On : 11-10-2012
// ***********************************************************************
// <copyright file="SettingsPage.xaml.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using MyTimeDatabaseLib;
using Microsoft.Phone.Marketplace;

namespace FieldService.View
{
	/// <summary>
	/// Description for Settings.
	/// </summary>
	public partial class SettingsPage : PhoneApplicationPage
	{
		private bool _isTrial;

		/// <summary>
		/// Initializes a new instance of the Settings class.
		/// </summary>
		public SettingsPage()
		{
			DataContext = App.ViewModel;
			InitializeComponent();
			Loaded += Settings_Loaded;
			_isTrial = (new LicenseInformation()).IsTrial();
		}

		#region Events

		/// <summary>
		/// Handles the Click event of the HyperlinkButtonHelpMeQuestion control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
		private void HyperlinkButtonHelpMeQuestion_Click(object sender, RoutedEventArgs e)
		{
			var emailcomposer = new EmailComposeTask {
														 Subject = "Field Service App Help",
														 To = "help@square-hiptobe.com",
														 Body = string.Format("Description of Problem:\n\nSteps to Reproduce:\n\nI hereby give you my permission to contact me regarding this issue.\n\nApplication Version: {0}\nCore Version: {1}", tbAppVersion.Text, tbCoreVersion.Text)
													 };
			emailcomposer.Show();
		}

		/// <summary>
		/// Handles the Click event of the HyperlinkButtonRateApp control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
		private void HyperlinkButtonRateApp_Click(object sender, RoutedEventArgs e)
		{
			var marketplaceReviewTask = new MarketplaceReviewTask();

			marketplaceReviewTask.Show();
		}

		/// <summary>
		/// Handles the Loaded event of the Settings control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
		private void Settings_Loaded(object sender, RoutedEventArgs e)
		{
			var grid = FindName("SettingsRoot") as Grid;
			if (null == grid) return;
			StackPanel sp = App.AppSettingsProvider.BuildXaml();
			grid.Children.Add(sp);

			tbAppVersion.Text = App.GetVersion();
			tbCoreVersion.Text = Main.GetVersion();
			//bBuyMePleasePleaseBuyMe.Visibility = _isTrial ? Visibility.Visible : Visibility.Collapsed;
		}

		/// <summary>
		/// PH_s the completed.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The e.</param>
		private void ph_Completed(object sender, PhoneNumberResult e) { }

		#endregion

		/// <summary>
		/// Called when a page is no longer the active page in a frame.
		/// </summary>
		/// <param name="e">An object that contains the event data.</param>
		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			App.AppSettingsProvider.SaveSettings();

			base.OnNavigatedFrom(e);
		}

		private void bBuyMePleasePleaseBuyMe_Click(object sender, RoutedEventArgs e)
		{
			var donate = new WebBrowserTask() {
												  URL = @"https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=tk%40square%2dhiptobe%2ecom&lc=US&item_name=Field%20Service%20App%20%28Square%2eHipToBe%29&no_note=0&currency_code=USD&bn=PP%2dDonationsBF%3abtn_donate_SM%2egif%3aNonHostedGuest"
											  };
			donate.Show();
		}

		private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
		{
			var web = new WebBrowserTask();
			web.URL = "http://code.google.com/p/my-time-wp7/issues/entry?template=Defect%20report%20from%20user";
			web.Show();
		}
	}
}