using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
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
    public partial class ManuallyEnterTime : PhoneApplicationPage
    {
        private int _itemId = -1;

        public ManuallyEnterTime()
        {
            InitializeComponent();
        }

        private void abibSave_Click(object sender, EventArgs e)
        {
            var t = (TimeSpan) tspTime.Value;

            var minutes = (int)t.TotalMinutes;

            var td = new TimeData() {
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
            } catch(TimeDataItemNotFoundException) {
                TimeDataInterface.AddTime(td);
                App.ToastMe(string.Format("Time ({0} hrs & {1} min) added.", t.Hours, t.Minutes));
            }catch (Exception ee) {
                //TODO:Exception handler
                MessageBox.Show("Couldn't add time.\n\nException: " + ee.Message);
            }

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

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (!NavigationContext.QueryString.ContainsKey("id") || _itemId > 0) return;

            try {
                int id = int.Parse(NavigationContext.QueryString["id"]);

                TimeData td = TimeDataInterface.GetTimeDataItem(id);

                if (td != null) SetText(td);
            } catch { }
        }

        private void SetText(TimeData td)
        {
            tbBibleStudies.Text = td.BibleStudies.ToString();
            tbBooks.Text = td.Books.ToString();
            tbBrochures.Text = td.Brochures.ToString();
            tbMags.Text = td.Magazines.ToString();
            tbNotes.Text = td.Notes;
            tbReturnVisits.Text = td.ReturnVisits.ToString();
            dpDatePicker.Value = td.Date;
            tspTime.Value = new TimeSpan(0,0,td.Minutes,0,0);
            _itemId = td.ItemId;
        }

        private void abmiDelete_Click(object sender, EventArgs e)
        {
            if (_itemId < 0) return;
            TimeDataInterface.DeleteTime(_itemId);
            App.ToastMe("Time Deleted.");
            Thread.Sleep(500);
            NavigationService.GoBack();
        }
    }
}