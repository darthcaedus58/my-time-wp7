using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace FieldService.View
{
        public partial class StreetBuildingList : PhoneApplicationPage
        {
                public StreetBuildingList()
                {
                        this.Language = XmlLanguage.GetLanguage(CultureInfo.CurrentUICulture.Name);
                        InitializeComponent();
                }
        }
}