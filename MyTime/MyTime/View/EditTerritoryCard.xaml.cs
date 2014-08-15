using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using FieldService.Model;
using FieldService.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Telerik.Windows.Controls;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace FieldService.View
{
        public partial class EditTerritoryCard : PhoneApplicationPage
        {
            private EditTerritoryCardViewModel ViewModel { get { return ((EditTerritoryCardViewModel) this.DataContext); } }
                public EditTerritoryCard()
                {
            this.Language = XmlLanguage.GetLanguage(CultureInfo.CurrentUICulture.Name);
                        InitializeComponent();
                }

                private void bTakePhoto_OnTap (object sender, GestureEventArgs e)
                {
                        var cc = new CameraCaptureTask();
                    cc.Completed += (o, result) =>
                    {
                        if (result.TaskResult == TaskResult.Cancel) return;
                        var bi = new BitmapImage();
                        bi.SetSource(result.ChosenPhoto);
                        biTerrImage.Source = bi;

                        var wb = new WriteableBitmap(biTerrImage, null);
                        wb.Invalidate();

                        var bmp = new BitmapImage();
                        using (var ms = new MemoryStream()) {
                            wb.SaveJpeg(ms, 300, 300, 0, 100);
                            bmp.SetSource(ms);
                        }
                        ViewModel.TerritoryCardImage = bmp;
                        biTerrImage.SetBinding(Image.SourceProperty,
                            new Binding() {Source = ViewModel.TerritoryCardImage});
                    };
                        cc.Show();
                }

                private void abibSaveTerrCard_OnClick(object sender, EventArgs e)
                {
                    UpdateViewModel();

                    App.ToastMe(ViewModel.SaveOrUpdate() ? "Territory Card Saved" : "Couldn't save card.");
                }

            private void UpdateViewModel()
            {
                if (FocusManager.GetFocusedElement() is RadTextBox) {
                    var tb = FocusManager.GetFocusedElement() as RadTextBox;
                    if (tb != null) {
                        tb.GetBindingExpression(RadTextBox.TextProperty).UpdateSource();
                    }
                }
            }
        }
}