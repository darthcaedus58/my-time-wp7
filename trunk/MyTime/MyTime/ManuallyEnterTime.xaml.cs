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
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using MyTimeDatabaseLib;

namespace MyTime
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
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e) { }

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
                                      Magazines = string.IsNullOrEmpty(tbMags.Text) ? 0 : int.Parse(tbMags.Text),
                                      Brochures = string.IsNullOrEmpty(tbBrochures.Text) ? 0 : int.Parse(tbBrochures.Text),
                                      Books = string.IsNullOrEmpty(tbBooks.Text) ? 0 : int.Parse(tbBooks.Text),
                                      BibleStudies = string.IsNullOrEmpty(tbBibleStudies.Text) ? 0 : int.Parse(tbBibleStudies.Text),
                                      ReturnVisits = string.IsNullOrEmpty(tbReturnVisits.Text) ? 0 : int.Parse(tbReturnVisits.Text),
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

                if (td != null) SetText(td);
            } catch (Exception) {}
        }

        /// <summary>
        /// Sets the text.
        /// </summary>
        /// <param name="td">The td.</param>
        private void SetText(TimeData td)
        {
            tbBibleStudies.Text = td.BibleStudies.ToString(CultureInfo.InvariantCulture);
            tbBooks.Text = td.Books.ToString(CultureInfo.InvariantCulture);
            tbBrochures.Text = td.Brochures.ToString(CultureInfo.InvariantCulture);
            tbMags.Text = td.Magazines.ToString(CultureInfo.InvariantCulture);
            tbNotes.Text = td.Notes;
            tbReturnVisits.Text = td.ReturnVisits.ToString(CultureInfo.InvariantCulture);
            dpDatePicker.Value = td.Date;
            tspTime.Value = new TimeSpan(0, 0, td.Minutes, 0, 0);
            _itemId = td.ItemId;
        }
    }
}