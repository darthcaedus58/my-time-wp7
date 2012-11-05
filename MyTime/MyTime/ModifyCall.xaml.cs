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
using MyTimeDatabaseLib;

namespace MyTime
{
    public partial class ModifyCall : PhoneApplicationPage
    {
        private int _rvItemId = -1;
        private  int _callId = -1;

        public ModifyCall()
        {
            InitializeComponent();
        }

        private void abmiDeleteCall_Click(object sender, EventArgs e)
        {

        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
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
            } catch { }
        }

        private void RetrieveCallInfo(int _callId)
        {
            throw new NotImplementedException();
        }

        private void TextBoxMasking_KeyDown(object sender, KeyEventArgs e)
        {
            Key[] GoodKeys = { Key.D0, Key.D1, Key.D2, Key.D3, Key.D4, 
                             Key.D5, Key.D6, Key.D7, Key.D8, Key.D9, 
                             Key.NumPad0, Key.NumPad1, Key.NumPad2, Key.NumPad3, Key.NumPad4, 
                             Key.NumPad5, Key.NumPad6, Key.NumPad7, Key.NumPad8, Key.NumPad9};
            if (!GoodKeys.Contains(e.Key)) {
                e.Handled = true;
                return;
            }
        }

        private void ApplicationBarIconButtonSave_Click(object sender, EventArgs e)
        {
            ///TODO:Add code for updating a call
            RvPreviousVisitData call = new RvPreviousVisitData()
            {
                RvItemId = _rvItemId,
                Books = int.Parse(tbBooks.Text),
                Brochures = int.Parse(tbBrochures.Text),
                Magazines = int.Parse(tbMags.Text),
                Notes = tbNotes.Text,
                Date = (DateTime)dpDatePicker.Value
            };
            try {
                RvPreviousVisitsDataInterface.SaveCall(call);
            } catch (Exception ee) {

            }
        }
    }
}