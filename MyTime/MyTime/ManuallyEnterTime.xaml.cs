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
    public partial class ManuallyEnterTime : PhoneApplicationPage
    {
        public ManuallyEnterTime()
        {
            InitializeComponent();
        }

        private void abibSave_Click(object sender, EventArgs e)
        {

        }

        private void abibCancel_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }


        private void TextBoxMasking_KeyDown(object sender, KeyEventArgs e)
        {
            Key[] GoodKeys = { Key.D0, Key.D1, Key.D2, Key.D3, Key.D4, 
                             Key.D5, Key.D6, Key.D7, Key.D8, Key.D9, 
                             Key.NumPad0, Key.NumPad1, Key.NumPad2, Key.NumPad3, Key.NumPad4, 
                             Key.NumPad5, Key.NumPad6, Key.NumPad7, Key.NumPad8, Key.NumPad9};
            if (!GoodKeys.Contains(e.Key)) {
                e.Handled = true;
                return;
            }
        }
    }
}