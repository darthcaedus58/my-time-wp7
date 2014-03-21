// ***********************************************************************
// Assembly         : MyTime
// Author           : trevo_000
// Created          : 11-03-2012
//
// Last Modified By : trevo_000
// Last Modified On : 11-07-2012
// ***********************************************************************
// <copyright file="ReturnVisitItemViewModel.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace FieldService.Model
{
    /// <summary>
    /// Class ReturnVisitItemViewModel
    /// </summary>
    public class ReturnVisitSummaryModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The _image
        /// </summary>
        private BitmapImage _image;
        /// <summary>
        /// The _item ID
        /// </summary>
        private int _itemID;
        /// <summary>
        /// The _line one
        /// </summary>
        private string _formattedAddress;
        /// <summary>
        /// The _line three
        /// </summary>
        private string _daysSinceVisit;

        /// <summary>
        /// The _nameOrDescription
        /// </summary>
        private string _nameOrDescription;

        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <value>The image source.</value>
        /// <returns></returns>
        public BitmapImage ImageSource
        {
            get { return _image; }

            set
            {
                if (value != _image) {
                    _image = value;
                    NotifyPropertyChanged("ImageSource");
                }
            }
        }

        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <value>The name.</value>
        /// <returns></returns>
        public string NameOrDescription
        {
            get { return _nameOrDescription; }
            set
            {
                if (value != _nameOrDescription) {
                    _nameOrDescription = value;
                    NotifyPropertyChanged("NameOrDescription");
                }
            }
        }

        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <value>The line one.</value>
        /// <returns></returns>
        public string FormattedAddress
        {
            get { return _formattedAddress; }
            set
            {
                if (value != _formattedAddress) {
                    _formattedAddress = value;
                    NotifyPropertyChanged("FormattedAddress");
                }
            }
        }
        public string DaysSinceVisit
        {
            get { return _daysSinceVisit; }
            set
            {
                if (_daysSinceVisit != value) {
                    _daysSinceVisit = value;
                    NotifyPropertyChanged("DaysSinceVisit");
                }
            }
        }

        /// <summary>
        /// Gets or sets the item id.
        /// </summary>
        /// <value>The item id.</value>
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

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Occurs when [property changed].
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        /// <summary>
        /// Notifies the property changed.
        /// </summary>
        /// <param name="propertyName">NameOrDescription of the property.</param>
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}