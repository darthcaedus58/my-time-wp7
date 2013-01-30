// ***********************************************************************
// Assembly         : MyTime
// Author           : trevo_000
// Created          : 11-03-2012
//
// Last Modified By : trevo_000
// Last Modified On : 11-07-2012
// ***********************************************************************
// <copyright file="RegularTimePage.xaml.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using FieldService.ViewModels;
using Microsoft.Phone.Controls;

namespace FieldService
{
	/// <summary>
	/// Class RegularTimePage
	/// </summary>
	public partial class RegularTimePage : PhoneApplicationPage
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RegularTimePage" /> class.
		/// </summary>
		public RegularTimePage()
		{
			DataContext = new AddModifyTimeViewModel();
			InitializeComponent();
		}

		private AddModifyTimeViewModel ViewModel { get { return ((AddModifyTimeViewModel) DataContext); } }

		#region Events

		/// <summary>
		/// Handles the Loaded event of the PhoneApplicationPage control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
		private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
		{
			bool countCalls = bool.Parse(App.AppSettingsProvider["AddCallPlacements"].Value);
			if (countCalls) {
				tbReturnVisits.Visibility = Visibility.Collapsed;
			}
		}

		/// <summary>
		/// Handles the Click event of the abibSave control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		private void abibSave_Click(object sender, EventArgs e)
		{
			tbNotes.GetBindingExpression(TextBox.TextProperty).UpdateSource();
			App.ToastMe(ViewModel.AddOrUpdateTime() ? string.Format("Time: {0} Hours Saved.", ViewModel.TimeData.Hours) : "Failed to save time.");
		}

		private void abmiConvertToRbc_Click_1(object sender, EventArgs e)
		{
			bool v = ViewModel.ConvertToRBCTime();

			App.ToastMe(v ? "Time Converted to RBC." : "Conversion Failed.");
			if (!v) return;
			Thread.Sleep(500);
			NavigationService.GoBack();
		}

		/// <summary>
		/// Handles the Click event of the abmiDelete control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		private void abmiDelete_Click(object sender, EventArgs e)
		{
			if (ViewModel.TimeDataItemId < 0) return;
			bool v = ViewModel.DeleteTime();
			App.ToastMe(v ? "Time deleted." : "Time delete failed.");
			if (!v) return;
			Thread.Sleep(500);
			NavigationService.GoBack();
		}

		#endregion

		/// <summary>
		/// Called when a page becomes the active page in a frame.
		/// </summary>
		/// <param name="e">An object that contains the event data.</param>
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			if (!NavigationContext.QueryString.ContainsKey("id") || ViewModel.TimeDataItemId > 0) return;

			try {
				int id = int.Parse(NavigationContext.QueryString["id"]);

				ViewModel.TimeDataItemId = id;
			} catch (Exception) {}
		}
	}
}