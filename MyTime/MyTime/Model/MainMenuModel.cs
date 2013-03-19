// ***********************************************************************
// Assembly         : MyTime
// Author           : trevo_000
// Created          : 11-03-2012
//
// Last Modified By : trevo_000
// Last Modified On : 11-03-2012
// ***********************************************************************
// <copyright file="MainMenuViewModel.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.ComponentModel;

namespace FieldService.Model
{
	/// <summary>
	/// Class MainMenuViewModel
	/// </summary>
	public class MainMenuModel : INotifyPropertyChanged
	{
		/// <summary>
		/// The _icon
		/// </summary>
		private string _icon;

		/// <summary>
		/// The _menu text
		/// </summary>
		private string _menuText;

		private string _navigateToPage;

		/// <summary>
		/// Gets or sets the icon URI.
		/// </summary>
		/// <value>The icon URI.</value>
		public string IconUri
		{
			get { return _icon; }
			set
			{
				if (value != _icon) {
					_icon = value;
					NotifyPropertyChanged("Icon");
				}
			}
		}

		/// <summary>
		/// Gets or sets the menu text.
		/// </summary>
		/// <value>The menu text.</value>
		public string MenuText
		{
			get { return _menuText; }
			set
			{
				if (value != _menuText) {
					_menuText = value;
					NotifyPropertyChanged("MenuText");
				}
			}
		}

		public string NavigateToPage
		{
			get { return _navigateToPage; } 
			set
			{
				if (value != _navigateToPage) {
					_navigateToPage = value;
					NotifyPropertyChanged("NavigateToPage");
				}
			}
		}

		#region INotifyPropertyChanged Members

		/// <summary>
		/// Occurs when [property changed].
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <returns>A <see cref="System.String" /> that represents this instance.</returns>
		public override string ToString() { return MenuText; }

		/// <summary>
		/// Notifies the property changed.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		private void NotifyPropertyChanged(String propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (null != handler) {
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}