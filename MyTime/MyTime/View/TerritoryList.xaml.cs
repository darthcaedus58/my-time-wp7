using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Windows.Foundation.Metadata;
using FieldService.Model;
using FieldService.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Telerik.Windows.Controls;

namespace FieldService.View
{
        public partial class TerritoryList : PhoneApplicationPage
        {
            private TerritoryListPageViewModel ViewModel { get { return (TerritoryListPageViewModel) DataContext; } }
                public TerritoryList()
                {
                        InitializeComponent();
                }

                private void abibAddTerritory_OnClick(object sender, EventArgs e)
                {
                        NavigationService.Navigate(new Uri("/View/EditTerritoryCard.xaml", UriKind.Relative));
                }

                private void PageLoaded(object sender, RoutedEventArgs e)
                {
                    
                }
                protected override void OnNavigatedTo(NavigationEventArgs e)
                {
                    base.OnNavigatedTo(e);

                    ViewModel.LoadTerritoryList();
                }

                private void TerritoryList_SelectionChanged(object sender, SelectionChangedEventArgs e)
                {
                    var ls = sender as RadDataBoundListBox;
                    if (ls == null) return;
                    if (ls.SelectedItem == null) return;

                    var terr = ls.SelectedItem as TerritoryCardModel;
                    if (terr == null) return;

                    NavigationService.Navigate(
                        new Uri(string.Format("/View/StreetBuildingList.xaml?id={0}", terr.ItemId), UriKind.Relative));

                    ls.SelectedItem = null;
                }
        }
}