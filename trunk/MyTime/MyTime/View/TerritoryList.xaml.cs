using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace FieldService.View
{
        public partial class TerritoryList : PhoneApplicationPage
        {
                public TerritoryList()
                {
                        InitializeComponent();
                }

                private void abibAddTerritory_OnClick(object sender, EventArgs e)
                {
                        NavigationService.Navigate(new Uri("/View/EditTerritoryCard.xaml", UriKind.Relative));
                }
        }
}