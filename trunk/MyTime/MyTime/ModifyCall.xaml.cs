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
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using MyTimeDatabaseLib;

namespace MyTime
{
    /// <summary>
    /// Class ModifyCall
    /// </summary>
    public partial class ModifyCall : PhoneApplicationPage
    {
        /// <summary>
        /// The _call id
        /// </summary>
        private int _callId = -1;

        /// <summary>
        /// The _RV item id
        /// </summary>
        private int _rvItemId = -1;

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
            var call = new RvPreviousVisitData();
            if (_callId >= 0) call = RvPreviousVisitsDataInterface.GetCall(_callId);
            call.RvItemId = _rvItemId;
            call.Books = string.IsNullOrEmpty(tbBooks.Text) ? 0 : int.Parse(tbBooks.Text);
            call.Brochures = string.IsNullOrEmpty(tbBrochures.Text) ? 0 : int.Parse(tbBrochures.Text);
            call.Magazines = string.IsNullOrEmpty(tbMags.Text) ? 0 : int.Parse(tbMags.Text);
            call.Notes = tbNotes.Text;
            call.Date = (dpDatePicker.Value ?? DateTime.Now);


            try {
                _callId = RvPreviousVisitsDataInterface.SaveCall(call, true);
                App.ToastMe(string.Format("Call {0}.", _callId >= 0 ? "Saved" : "Added"));
            } catch (Exception ee) {
                MessageBox.Show("Couldn't save call.\n\n\nException: " + ee.Message);
            }
        }

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
        /// Handles the Click event of the abmiDeleteCall control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void abmiDeleteCall_Click(object sender, EventArgs e)
        {
            if (_callId < 0 || MessageBox.Show("Are you sure you want to delete this call?", "FIELD SERVICE", MessageBoxButton.OKCancel) != MessageBoxResult.OK) return;
            RvPreviousVisitsDataInterface.DeleteCall(_callId);
            App.ToastMe("Call Deleted.");
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
                NavigationService.GoBack();
                return;
            }
            string rvItemId = NavigationContext.QueryString["rvid"];
            string callId = NavigationContext.QueryString.ContainsKey("id") ? NavigationContext.QueryString["id"] : string.Empty;
            if (string.IsNullOrEmpty(rvItemId)) {
                NavigationService.GoBack();
                return;
            }
            try {
                _rvItemId = int.Parse(rvItemId);
                _callId = string.IsNullOrEmpty(callId) ? -1 : int.Parse(callId);

                if (_callId >= 0) RetrieveCallInfo(_callId);
            } catch (Exception) {}
        }

        /// <summary>
        /// Retrieves the call info.
        /// </summary>
        /// <param name="callId">The call id.</param>
        private void RetrieveCallInfo(int callId)
        {
            try {
                RvPreviousVisitData call = RvPreviousVisitsDataInterface.GetCall(callId);

                tbBooks.Text = call.Books.ToString(CultureInfo.InvariantCulture);
                tbBrochures.Text = call.Brochures.ToString(CultureInfo.InvariantCulture);
                tbMags.Text = call.Magazines.ToString(CultureInfo.InvariantCulture);
                tbNotes.Text = call.Notes;
                dpDatePicker.Value = call.Date;
                _callId = call.ItemId;
            } catch (RvPreviousVisitNotFoundException e) {
                MessageBox.Show(string.Format("{0}\n\nPlease create a new call.", e.Message));
            }
        }
    }
}