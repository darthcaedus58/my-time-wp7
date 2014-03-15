using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using FieldService.Model;
using FieldService.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace FieldService.View
{
        public partial class EditTerritoryCard : PhoneApplicationPage
        {
                public EditTerritoryCard()
                {
                        InitializeComponent();
                }

                private void bTakePhoto_OnTap (object sender, GestureEventArgs e)
                {
                        var cc = new CameraCaptureTask();
                        cc.Completed += (o, result) => {
                                var bmp = new BitmapImage();
                                bmp.SetSource(result.ChosenPhoto);
                                ((EditTerritoryCardViewModel) DataContext).TerritoryCardImage = bmp;
                        };
                        cc.Show();
                }

                private void abibSaveTerrCard_OnClick(object sender, EventArgs e)
                {
                        throw new NotImplementedException();
                }
        }
}