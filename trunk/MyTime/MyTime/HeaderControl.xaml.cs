using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace FieldService
{
	public partial class HeaderControl : UserControl
	{
		public string IconSource { get; set; }
		public string HeaderText { get; set; }

		public HeaderControl()
		{
			DataContext = this;
			InitializeComponent();
		}
	}
}
