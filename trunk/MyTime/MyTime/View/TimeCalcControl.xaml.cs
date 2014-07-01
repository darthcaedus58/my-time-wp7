using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Channels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Telerik.Windows.Controls;

namespace FieldService.View
{
    public class TimeCalcFormClosedEventArgs : EventArgs
    {
        public DialogResult DialogResult { get; set; }
        public TimeSpan TimeSpan { get; set; }
        public TimeCalcFormClosedEventArgs(DialogResult r, TimeSpan t)
        {
            DialogResult = r;
            TimeSpan = t;
        }
    }
    public partial class TimeCalcControl : UserControl
    {
        public event FormClosedEventHandler FormClosed;
        public delegate void FormClosedEventHandler(object sender, TimeCalcFormClosedEventArgs e);
 
        public TimeCalcControl()
        {
            InitializeComponent();
            tpEnd.Value = DateTime.Now;

        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            var start = tpStart.Value ?? DateTime.MinValue;
            var end = tpEnd.Value ?? DateTime.MinValue;

            TimeSpan t = end - start;

            if (t.Minutes < 0) {
                MessageBox.Show("Start time must be before End Time.");
                return;
            }

            if (start == DateTime.MinValue || end == DateTime.MinValue)
                FormClosed(this, new TimeCalcFormClosedEventArgs(DialogResult.Cancel, TimeSpan.Zero));

            t = RoundTime(t);


            FormClosed(this, new TimeCalcFormClosedEventArgs(DialogResult.OK, t));
        }

        private static TimeSpan RoundTime(TimeSpan t)
        {
            var ts =
                TimeSpan.FromMinutes(App.Settings.roundTimeIncrement*
                                     Math.Ceiling(t.TotalMinutes/App.Settings.roundTimeIncrement));
            float m = float.Parse(t.TotalMinutes.ToString())%App.Settings.roundTimeIncrement;
            if (m <= (App.Settings.roundTimeIncrement/2.0))
                t = TimeSpan.FromMinutes(ts.TotalMinutes - App.Settings.roundTimeIncrement);
            else t = ts;
            return t;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            FormClosed(this, new TimeCalcFormClosedEventArgs(DialogResult.Cancel, TimeSpan.Zero));
        }
    }

    
}
