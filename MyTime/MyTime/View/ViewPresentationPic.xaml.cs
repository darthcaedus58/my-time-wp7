using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FieldService.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps.Overlays;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace FieldService.View
{
    public partial class ViewPresentationPic : PhoneApplicationPage
    {
        public ViewPresentationPic()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try {
                using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                using (var isoFileStream = isoStore.OpenFile("Presentation.jpg", FileMode.Open, FileAccess.Read)) {
                    var bi = new BitmapImage();
                    bi.SetSource(isoFileStream);
                    imgPresentationPic.Source = bi;
                }  
            }
            catch {
            }
        }

        private void ApplicationBarIconButton_OnClick(object sender, EventArgs e)
        {
            var cc = new CameraCaptureTask();
            cc.Completed += (o, result) =>
            {
                var bmp = new BitmapImage();
                if (result.ChosenPhoto == null) return;
                bmp.SetSource(result.ChosenPhoto);
                imgPresentationPic.Source = bmp;

                using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication()) {
                    var wb = new WriteableBitmap(bmp);
                    using (var isoFileStream = isoStore.CreateFile("Presentation.jpg"))
                        Extensions.SaveJpeg(wb, isoFileStream, bmp.PixelWidth, wb.PixelHeight, 0, 100);
                }
            };
            cc.Show();
        }
    }
}