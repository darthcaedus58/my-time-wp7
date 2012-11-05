using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;

namespace MyTime
{
	public class ReturnVisitItemViewModel : INotifyPropertyChanged
	{
		private BitmapImage _image;
		/// <summary>
		/// Sample ViewModel property; this property is used in the view to display its value using a Binding.
		/// </summary>
		/// <returns></returns>
		public BitmapImage ImageSource
		{
			get
			{
				return _image;
			}

			set
			{
				if (value != _image) {
					_image = value;
					NotifyPropertyChanged("ImageSource");
				}
			}
		}

		private string _name;
		/// <summary>
		/// Sample ViewModel property; this property is used in the view to display its value using a Binding.
		/// </summary>
		/// <returns></returns>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				if (value != _name) {
					_name = value;
					NotifyPropertyChanged("Name");
				}
			}
		}

		private string _lineOne;
		/// <summary>
		/// Sample ViewModel property; this property is used in the view to display its value using a Binding.
		/// </summary>
		/// <returns></returns>
		public string LineOne
		{
			get
			{
				return _lineOne;
			}
			set
			{
				if (value != _lineOne) {
					_lineOne = value;
					NotifyPropertyChanged("LineOne");
				}
			}
		}

		private string _lineTwo;
		/// <summary>
		/// Sample ViewModel property; this property is used in the view to display its value using a Binding.
		/// </summary>
		/// <returns></returns>
		public string LineTwo
		{
			get
			{
				return _lineTwo;
			}
			set
			{
				if (value != _lineTwo) {
					_lineTwo = value;
					NotifyPropertyChanged("LineTwo");
				}
			}
		}

		private int _itemID;

		public int ItemId
		{
			get { return _itemID; }
			set
			{
				if (value != _itemID) {
					_itemID = value;
					NotifyPropertyChanged("ItemId");
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged(String propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (null != handler) {
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}

}