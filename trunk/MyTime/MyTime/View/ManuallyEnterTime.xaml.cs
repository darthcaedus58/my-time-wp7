// ***********************************************************************
// Assembly         : MyTime
// Author           : trevo_000
// Created          : 11-03-2012
//
// Last Modified By : trevo_000
// Last Modified On : 11-07-2012
// ***********************************************************************
// <copyright file="ManuallyEnterTime.xaml.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using MyTimeDatabaseLib;

namespace FieldService
{
	/// <summary>
	/// Class ManuallyEnterTime
	/// </summary>
	public partial class ManuallyEnterTime : PhoneApplicationPage
	{
		/// <summary>
		/// The _item id
		/// </summary>
		private int _itemId = -1;

		/// <summary>
		/// Initializes a new instance of the <see cref="ManuallyEnterTime" /> class.
		/// </summary>
		public ManuallyEnterTime() { InitializeComponent(); }

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
			if (tspTime.Value == null) return;
			var t = (TimeSpan) tspTime.Value;

			var minutes = (int) t.TotalMinutes;

			var td = new TimeData {
				                      Date = (DateTime) dpDatePicker.Value,
				                      Minutes = minutes,
				                      Magazines = (int) tbMags.Value,
				                      Brochures = (int) tbBrochures.Value,
				                      Books = (int) tbBooks.Value,
				                      BibleStudies = (int) tbBibleStudies.Value,
				                      ReturnVisits = (int) tbReturnVisits.Value,
				                      Notes = tbNotes.Text
			                      };
			try {
				if (_itemId >= 0) {
					TimeDataInterface.UpdateTime(_itemId, td);
					App.ToastMe("Time Updated.");
				} else {
					TimeDataInterface.AddTime(td);
					App.ToastMe(string.Format("Time ({0} hrs & {1} min) added.", t.Hours, t.Minutes));
				}
			} catch (TimeDataItemNotFoundException) {
				TimeDataInterface.AddTime(td);
				App.ToastMe(string.Format("Time ({0} hrs & {1} min) added.", t.Hours, t.Minutes));
			} catch (Exception ee) {
				//TODO:Exception handler
				MessageBox.Show("Couldn't add time.\n\nException: " + ee.Message);
			}
		}

		private void abmiConvertToRbc_Click_1(object sender, EventArgs e)
		{
			if (_itemId > 0) TimeDataInterface.DeleteTime(_itemId);
			var rtd = new RBCTimeData() {
				                            Minutes = (int) ((TimeSpan) tspTime.Value).TotalMinutes,
				                            Date = (DateTime) dpDatePicker.Value,
				                            Notes = tbNotes.Text
			                            };
			RBCTimeDataInterface.AddOrUpdateTime(ref rtd);

			App.ToastMe("Time Converted to RBC.");
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
			if (_itemId < 0) return;
			TimeDataInterface.DeleteTime(_itemId);
			App.ToastMe("Time Deleted.");
			Thread.Sleep(500);
			NavigationService.GoBack();
		}

		#endregion

		/// <summary>
		/// Handles the KeyDown event of the TextBoxMasking control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="KeyEventArgs" /> instance containing the event data.</param>
		private void TextBoxMasking_KeyDown(object sender, KeyEventArgs e)
		{
			Key[] goodKeys = {
				                 Key.D0, Key.D1, Key.D2, Key.D3, Key.D4,
				                 Key.D5, Key.D6, Key.D7, Key.D8, Key.D9,
				                 Key.NumPad0, Key.NumPad1, Key.NumPad2, Key.NumPad3, Key.NumPad4,
				                 Key.NumPad5, Key.NumPad6, Key.NumPad7, Key.NumPad8, Key.NumPad9
			                 };
			if (!goodKeys.Contains(e.Key)) {
				e.Handled = true;
			}
		}

		/// <summary>
		/// Called when a page becomes the active page in a frame.
		/// </summary>
		/// <param name="e">An object that contains the event data.</param>
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			if (!NavigationContext.QueryString.ContainsKey("id") || _itemId > 0) return;

			try {
				int id = int.Parse(NavigationContext.QueryString["id"]);

				TimeData td = TimeDataInterface.GetTimeDataItem(id);

				if (td != null) {
					SetText(td);
				}
			} catch (Exception) {}
		}

		/// <summary>
		/// Sets the text.
		/// </summary>
		/// <param name="td">The td.</param>
		private void SetText(TimeData td)
		{
			tbBibleStudies.Value = td.BibleStudies;
			tbBooks.Value = td.Books;
			tbBrochures.Value = td.Brochures;
			tbMags.Value = td.Magazines;
			tbNotes.Text = td.Notes;
			tbReturnVisits.Value = td.ReturnVisits;
			dpDatePicker.Value = td.Date;
			tspTime.Value = new TimeSpan(0, 0, td.Minutes, 0, 0);
			_itemId = td.ItemId;
		}
	}
}