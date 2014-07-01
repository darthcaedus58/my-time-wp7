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
using Microsoft.Phone.Shell;
using Telerik.Windows.Controls;

namespace FieldService.View
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
			//DataContext = new AddModifyTimeViewModel();
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
		    bool countCalls = App.Settings.manuallyTrackRvs;
			if (countCalls) {
				tbReturnVisits.Visibility = Visibility.Collapsed;
			}
			var convert = ApplicationBar.MenuItems[0] as ApplicationBarMenuItem;
			var delete = ApplicationBar.MenuItems[1] as ApplicationBarMenuItem;
			var save = ApplicationBar.Buttons[0] as ApplicationBarIconButton;

			if (convert != null) {
				convert.Text = StringResources.AddTimePage_ConvertMenuItem;
			}
			if (delete != null) {
				delete.Text = StringResources.AddTimePage_DeleteMenuItem;
			}
			if (save != null) {
				save.Text = StringResources.AddTimePage_Save;
			}
		}

		/// <summary>
		/// Handles the Click event of the abibSave control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		private void abibSave_Click(object sender, EventArgs e)
		{
			if(!string.IsNullOrWhiteSpace(tbNotes.Text)) tbNotes.GetBindingExpression(TextBox.TextProperty).UpdateSource();

			int idExisting;
			if (ViewModel.IsDoubleDataEntry(out idExisting)) {
				var r = MessageBox.Show(StringResources.AddRBCTimePage_AskForDoubleEntry, "", MessageBoxButton.OKCancel);
				if (r == MessageBoxResult.OK) {
					App.ToastMe(ViewModel.AddOrUpdateTime(idExisting) ? string.Format(StringResources.AddTimePage_Messages_SaveCompleted, ViewModel.TimeData.Hours) : StringResources.AddTimePage_Messages_SaveFailed);
					return;
				}
			}
			App.ToastMe(ViewModel.AddOrUpdateTime() ? string.Format(StringResources.AddTimePage_Messages_SaveCompleted, ViewModel.TimeData.Hours) : StringResources.AddTimePage_Messages_SaveFailed);
		}

		private void abmiConvertToRbc_Click_1(object sender, EventArgs e)
		{
			bool v = ViewModel.ConvertToRBCTime();

			App.ToastMe(v ? StringResources.AddTimePage_Messages_RBCConvertSuccess : StringResources.AddTimePage_Messages_RBCConvertFailed);
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
			App.ToastMe(v ? StringResources.AddTimePage_Messages_TimeDeleteSuccess : StringResources.AddTimePage_Messages_TimeDeleteFailed);
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

	    private void TimeCalcButton_OnClick(object sender, RoutedEventArgs e)
	    {
	        TimeCalc.Visibility = Visibility.Visible;
	        ContentGrid.IsHitTestVisible = false;
	    }

	    private void TimeCalcControl_OnFormClosed(object sender, TimeCalcFormClosedEventArgs e)
	    {
	        TimeCalc.Visibility = Visibility.Collapsed;
	        ContentGrid.IsHitTestVisible = true;
	        if (e.DialogResult == DialogResult.OK) {
	            ViewModel.TimeDataTotalTime = e.TimeSpan;
	        }
	    }
	}
}