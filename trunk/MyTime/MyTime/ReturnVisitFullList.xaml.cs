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
using MyTime.ViewModels;

namespace MyTime
{
    public partial class ReturnVistFullList : PhoneApplicationPage
    {
        public ReturnVistFullList()
        {
            DataContext = App.ViewModel;
            App.ViewModel.LoadReturnVisitFullList();
            InitializeComponent();
            llsAllReturnVisits.ItemsSource = App.ViewModel.llReturnVisitFullListCategory;
        }

        private void llsAllReturnVisits_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (llsAllReturnVisits.SelectedItem is ReturnVisitLLItemModel) {
                NavigationService.Navigate(new Uri(string.Format("/AddNewRV.xaml?id={0}", ((ReturnVisitLLItemModel) llsAllReturnVisits.SelectedItem).ItemId), UriKind.Relative));
            }
        }
    }
}