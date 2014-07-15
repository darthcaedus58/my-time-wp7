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
using FieldService.Common;

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
            tspBreakTime.Value = new TimeSpan(0,0,0);

        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            var start = tpStart.Value ?? DateTime.MinValue;
            if (start == DateTime.MinValue) {
                MessageBox.Show("Start time must be before End Time.");
                return;
            }
            var end = tpEnd.Value ?? DateTime.MinValue;
            if(end == DateTime.MinValue) {
                MessageBox.Show("Start time must be before End Time.");
                return;
            }
            var breakTime = tspBreakTime.Value ?? new TimeSpan(0,0,0);

            TimeSpan t = (end - start) - breakTime;

            if (t.Minutes < 0) {
                MessageBox.Show("Start time must be before End Time.");
                return;
            }

            if (start == DateTime.MinValue || end == DateTime.MinValue)
                FormClosed(this, new TimeCalcFormClosedEventArgs(DialogResult.Cancel, TimeSpan.Zero));

            t = GeneralHelper.RoundTime(t, App.Settings.roundTimeIncrement);


            FormClosed(this, new TimeCalcFormClosedEventArgs(DialogResult.OK, t));
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            FormClosed(this, new TimeCalcFormClosedEventArgs(DialogResult.Cancel, TimeSpan.Zero));
        }
    }

    
}
