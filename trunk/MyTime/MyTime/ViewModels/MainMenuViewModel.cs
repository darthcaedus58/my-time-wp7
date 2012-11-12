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

namespace FieldService.ViewModels
{
    /// <summary>
    /// Class MainMenuViewModel
    /// </summary>
    public class MainMenuViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The _icon
        /// </summary>
        private string _icon;
        /// <summary>
        /// The _menu image name
        /// </summary>
        private string _menuImageName;
        /// <summary>
        /// The _menu item name
        /// </summary>
        private string _menuItemName;

        /// <summary>
        /// The _menu text
        /// </summary>
        private string _menuText;

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

        /// <summary>
        /// Gets or sets the name of the menu item.
        /// </summary>
        /// <value>The name of the menu item.</value>
        public string MenuItemName
        {
            get { return _menuItemName; }
            set
            {
                if (value != _menuItemName) {
                    _menuItemName = value;
                    NotifyPropertyChanged("MenuItemName");
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the menu image.
        /// </summary>
        /// <value>The name of the menu image.</value>
        public string MenuImageName
        {
            get { return _menuImageName; }
            set
            {
                if (value != _menuImageName) {
                    _menuImageName = value;
                    NotifyPropertyChanged("MenuImageName");
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