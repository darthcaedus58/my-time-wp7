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
    }
}