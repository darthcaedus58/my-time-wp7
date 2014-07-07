using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace FieldService
{
	public partial class HeaderControl : UserControl
	{
		public static DependencyProperty HeaderTextProperty = DependencyProperty.Register("HeaderText", typeof (string), typeof (HeaderControl), new PropertyMetadata(new PropertyChangedCallback(HeaderTextPropertyChanged)));

		public static DependencyProperty IconSourceProperty = DependencyProperty.Register("IconSource", typeof (string), typeof (HeaderControl), new PropertyMetadata(new PropertyChangedCallback(IconSourcePropertyChanged)));

		public static new DependencyProperty ForegroundProperty = DependencyProperty.Register("Foreground", typeof (Brush), typeof (HeaderControl), new PropertyMetadata(App.Current.Resources["AppForegroundBrush"] as Brush, new PropertyChangedCallback(ForegroundPropertyChanged)));

        public static new DependencyProperty BackgroundProperty = DependencyProperty.Register("Background", typeof (Brush), typeof (HeaderControl), new PropertyMetadata(App.Current.Resources["AppBackgroundBrush"] as Brush, new PropertyChangedCallback(BackgroundPropertyChanged)));

	    public string HeaderText
		{
			get { return (string) GetValue(HeaderTextProperty); }
			set { SetValue(HeaderTextProperty, value); }
		}

		public string IconSource
		{
			get { return (string) GetValue(IconSourceProperty); }
			set { SetValue(IconSourceProperty, value); }
		}

	    public Brush Background
	    {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value);}
	    }

        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

		public HeaderControl()
		{
			InitializeComponent();
            //Foreground = App.Current.Resources["AppForegroundBrush"] as Brush;
            //Background = App.Current.Resources["AppBackgroundBrush"] as Brush;
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
                c.imgBrush.ImageSource = new BitmapImage(new Uri(e.NewValue.ToString(), UriKind.Relative));
			}
		}

        private static void BackgroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            var c = d as HeaderControl;
            if (c != null) {
                //c.Background = (Brush)e.NewValue;
                c.LayoutRoot.Background = (Brush) e.NewValue;
                c.rectOpacityMask.Fill = (Brush) e.NewValue;
            }
        }

        private static void ForegroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as HeaderControl;
            if (c != null) {
                //c.rectOpacityMask.Fill = (Brush) e.NewValue;
                //c.rectOpacityMask.OpacityMask.Opacity = 1;
                c.HeaderTextBlock.Foreground = (Brush)e.NewValue;
            }
        }
	}
}
