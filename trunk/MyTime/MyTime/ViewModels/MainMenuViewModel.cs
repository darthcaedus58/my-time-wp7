using System;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MyTime.ViewModels
{
	public class MainMenuViewModel : INotifyPropertyChanged
	{
		private string _icon;
		public string IconUri
		{
			get
			{
				return _icon;
			}
			set
			{
				if (value != _icon) {
					_icon = value;
					NotifyPropertyChanged("Icon");
				}
			}
		}

		private string _menuText;
		public string MenuText
		{
			get
			{
				return _menuText;
			}
			set
			{
				if (value != _menuText) {
					_menuText = value;
					NotifyPropertyChanged("MenuText");
				}
			}
		}

		private string _menuItemName;
		public string MenuItemName
		{
			get
			{
				return _menuItemName;
			}
			set
			{
				if (value != _menuItemName) {
					_menuItemName = value;
					NotifyPropertyChanged("MenuItemName");
				}
			}
		}

		private string _menuImageName;
		public string MenuImageName
		{
			get
			{
				return _menuImageName;
			}
			set
			{
				if (value != _menuImageName) {
					_menuImageName = value;
					NotifyPropertyChanged("MenuImageName");
				}
			}
		}

		public override string ToString() { return MenuText; }
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
