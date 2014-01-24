﻿using System;
using System.Threading;
using System.Windows;
using FieldService.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace FieldService.View
{
	public partial class RBCTimePage : PhoneApplicationPage
	{
		private AddModifyRBCTimeViewModel ViewModel { get { return ((AddModifyRBCTimeViewModel) DataContext); } }
		public RBCTimePage()
		{
			DataContext = new AddModifyRBCTimeViewModel();
			InitializeComponent();
		}

		protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			if (!NavigationContext.QueryString.ContainsKey("id") || ViewModel.RBCTimeData.ItemID > 0) return;

			try {
				int id = int.Parse(NavigationContext.QueryString["id"]);

				ViewModel.RBCTimeDataItemId = id;
			} catch { }
		}

		#region Events

		private void abibSave_Click_1(object sender, EventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(tbNotes.Text)) tbNotes.GetBindingExpression(PhoneTextBox.TextProperty).UpdateSource(); //if the text box has focus its data source will not be updated.

			int idExisting; 
			if (ViewModel.IsDoubleDateEntry(out idExisting)) {
				var r = MessageBox.Show(StringResources.AddRBCTimePage_AskForDoubleEntry, "", MessageBoxButton.OKCancel);
				if (r == MessageBoxResult.OK) {
					App.ToastMe(ViewModel.AddOrUpdateTime(idExisting) ? string.Format(StringResources.AddRBCTimePage_SaveConfirmation, ViewModel.RBCTimeData.Hours) : StringResources.AddRBCTimePage_SaveFailed);
					return;
				}
			}
			App.ToastMe(ViewModel.AddOrUpdateTime() ? string.Format(StringResources.AddRBCTimePage_SaveConfirmation, ViewModel.RBCTimeData.Hours) : StringResources.AddRBCTimePage_SaveFailed);
		}

		private void abmiConvertToRegTime_Click_1(object sender, EventArgs e)
		{
			bool v = ViewModel.ConvertToRegularTime();
			App.ToastMe(v ? "Converted to regular time." : "Conversion failed.");
			if (!v) return;
			Thread.Sleep(500);
			NavigationService.GoBack();
		}

		private void abmiDelete_Click_1(object sender, EventArgs e)
		{
			if (ViewModel.RBCTimeDataItemId < 0) return; //You can't delete what is not saved. durr.
			bool v = ViewModel.DeleteTime();
			App.ToastMe(v ? "RBC time deleted." : "RBC time delete failed.");
			if (!v) return;
			Thread.Sleep(500);
			NavigationService.GoBack();
		}

		#endregion

		private void PhoneApplicationPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			//
			var convert = ApplicationBar.MenuItems[0] as ApplicationBarMenuItem;
			var delete = ApplicationBar.MenuItems[1] as ApplicationBarMenuItem;
			var save = ApplicationBar.Buttons[0] as ApplicationBarIconButton;

			if (convert != null) {
				convert.Text = StringResources.AddRBCTimePage_ConvertMenuItem;
			}
			if(delete != null) {
				delete.Text = StringResources.AddRBCTimePage_Delete;
			}
			if(save != null) {
				save.Text = StringResources.AddRBCTimePage_Save;
			}
		}
	}
}