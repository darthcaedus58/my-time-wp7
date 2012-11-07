using System;
using System.Collections;
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
using System.Windows.Controls.DataVisualization.Charting;

namespace MyTime
{
    public partial class TimeReport : PhoneApplicationPage
    {
        public TimeReport()
        {
            InitializeComponent();
            DataContext = App.ViewModel;
           
        }

        private void lbEntries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbEntries.SelectedIndex < 0) return;
            NavigationService.Navigate(new Uri("/ManuallyEnterTime.xaml?id=" + ((TimeReportEntryViewModel) lbEntries.SelectedItem).ItemId.ToString(), UriKind.Relative));
            lbEntries.SelectedIndex = -1;
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            myChart.InvalidateMeasure();
            myChart.InvalidateArrange();
            myChart.UpdateLayout();
        }
    }

    public class TimeChartInfo
    {
        public string Header { get; set; }
        public double Time { get; set; }
    }

    // Class for storing activities
    public class ChartData : List<TimeChartInfo>
    {
        public ChartData()
        {
            if (App.ViewModel.icReport.Count > 1) {
                
                foreach (var v in App.ViewModel.icReport) {
                    Add(new TimeChartInfo() { Header = new String(v.Month.Take(3).ToArray()), Time = ((double)v.Minutes / 60.0) });
                }
            } else {
                foreach (var v in App.ViewModel.lbTimeEntries) {
                    var d = DateTime.Parse(v.Date);
                    Add(new TimeChartInfo() { Header = string.Format("{0}/{1}",d.Month, d.Day), Time = ((double)v.Minutes / 60.0) });
                }
            }
        }
    }
}