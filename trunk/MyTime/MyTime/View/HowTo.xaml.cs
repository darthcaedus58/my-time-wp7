
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace FieldService.View
{
        public partial class HowTo : PhoneApplicationPage
        {
                public HowTo()
                {
            this.Language = XmlLanguage.GetLanguage(CultureInfo.CurrentUICulture.Name);
                        InitializeComponent();
                }

                private void LeftArrow_Click(object sender, EventArgs e)
                {
                        this.slideView.MoveToPreviousItem();
                }

                private void RightArrow_Click(object sender, EventArgs e)
                {
                        this.slideView.MoveToNextItem();
                }

                private void CloseButton_Click(object sender, EventArgs e)
                {
                        NavigationService.GoBack();

                }
        }
}
