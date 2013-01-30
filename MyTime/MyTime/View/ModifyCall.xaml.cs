// ***********************************************************************
// Assembly         : MyTime
// Author           : trevo_000
// Created          : 11-03-2012
//
// Last Modified By : trevo_000
// Last Modified On : 11-07-2012
// ***********************************************************************
// <copyright file="ModifyCall.xaml.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using FieldService.ViewModels;
using Microsoft.Phone.Controls;
using MyTimeDatabaseLib;

namespace FieldService.View
{
	/// <summary>
	/// Class ModifyCall
	/// </summary>
	public partial class ModifyCall : PhoneApplicationPage
	{
		private PreviousVisitViewModel ViewModel { get { return ((PreviousVisitViewModel) DataContext); } }

		/// <summary>
		/// Initializes a new instance of the <see cref="ModifyCall" /> class.
		/// </summary>
		public ModifyCall() { InitializeComponent(); }

		#region Events

		/// <summary>
		/// Handles the Click event of the ApplicationBarIconButtonSave control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		private void ApplicationBarIconButtonSave_Click(object sender, EventArgs e)
		{
			tbNotes.GetBindingExpression(PhoneTextBox.TextProperty).UpdateSource();
			App.ToastMe(ViewModel.AddOrUpdateItem() ? "Call Saved." : "Call Saving Failed.");
		}

		/// <summary>
		/// Handles the Click event of the abmiDeleteCall control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		private void abmiDeleteCall_Click(object sender, EventArgs e)
		{
			if (ViewModel.PreviousVisitItemId < 0 || MessageBox.Show("Are you sure you want to delete this call?", "FIELD SERVICE", MessageBoxButton.OKCancel) != MessageBoxResult.OK) return;
			bool v = ViewModel.DeleteCall();
			if (!v) {
				App.ToastMe("Deleting the call failed.");
				return;
			}
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
			if (!NavigationContext.QueryString.ContainsKey("rvid")) {
				MessageBox.Show("You must save the RV before adding a call.");
				NavigationService.GoBack();
				return;
			}
			if (ViewModel.PreviousVisitItemId >= 0) return; //we have already loaded the call data and the user is just modifying it before saving it.
			string rvItemId = NavigationContext.QueryString["rvid"];
			if (string.IsNullOrEmpty(rvItemId)) {
				MessageBox.Show("You must save the RV before adding a call.");
				NavigationService.GoBack();
				return;
			}
			ViewModel.ReturnVisitItemId = int.Parse(rvItemId);

			if (!NavigationContext.QueryString.ContainsKey("id")) return;

			string callId = NavigationContext.QueryString["id"];
			ViewModel.PreviousVisitItemId = int.Parse(callId);
		}
	}
}