using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace FieldService
{
	public partial class HeaderControl : UserControl
	{
		public static DependencyProperty HeaderTextProperty = DependencyProperty.Register("HeaderText", typeof (string), typeof (HeaderControl), new PropertyMetadata(new PropertyChangedCallback(HeaderTextPropertyChanged)));

		public static DependencyProperty IconSourceProperty = DependencyProperty.Register("IconSource", typeof (ImageSource), typeof (HeaderControl), new PropertyMetadata(new PropertyChangedCallback(IconSourcePropertyChanged)));

		public string HeaderText
		{
			get { return (string) GetValue(HeaderTextProperty); }
			set { SetValue(HeaderTextProperty, value); }
		}

		public ImageSource IconSource
		{
			get { return (ImageSource) GetValue(IconSourceProperty); }
			set { SetValue(IconSourceProperty, value); }
		}

		public HeaderControl()
		{
			//DataContext = this;
			InitializeComponent();
		}

		private void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
		{
		}

		private static void HeaderTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			//throw new NotImplementedException();
			var c = d as HeaderControl;
			if(c != null) {
				c.HeaderTextBlock.Text = (string) e.NewValue;
			}
		}

		private static void IconSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			//throw new NotImplementedException();

			var c = d as HeaderControl;
			if(c!= null) {
				c.IconImage.Source = (ImageSource) e.NewValue;
			}
		}
	}
}
