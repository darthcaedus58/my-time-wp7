// ***********************************************************************
// Assembly         : MyTime
// Author           : trevo_000
// Created          : 11-03-2012
//
// Last Modified By : trevo_000
// Last Modified On : 11-05-2012
// ***********************************************************************
// <copyright file="PreviousVisitViewModel.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.ComponentModel;

namespace FieldService.ViewModels
{
    /// <summary>
    /// Class PreviousVisitViewModel
    /// </summary>
    public class PreviousVisitViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The _desc
        /// </summary>
        private string _desc;
        /// <summary>
        /// The _item ID
        /// </summary>
        private int _itemID;


        /// <summary>
        /// The _last visit date
        /// </summary>
        private string _lastVisitDate;

        /// <summary>
        /// The _placements
        /// </summary>
        private string _placements;

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

        /// <summary>
        /// Gets or sets the last visit date.
        /// </summary>
        /// <value>The last visit date.</value>
        public string LastVisitDate
        {
            get { return _lastVisitDate; }
            set
            {
                if (value != _lastVisitDate) {
                    _lastVisitDate = value;
                    NotifyPropertyChanged("LastVisitDate");
                }
            }
        }

        /// <summary>
        /// Gets or sets the placements.
        /// </summary>
        /// <value>The placements.</value>
        public string Placements
        {
            get { return _placements; }
            set
            {
                if (value != _placements) {
                    _placements = value;
                    NotifyPropertyChanged("Placements");
                }
            }
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description
        {
            get { return _desc; }
            set
            {
                if (value != _desc) {
                    _desc = value;
                    NotifyPropertyChanged("Description");
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