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

namespace MyTime
{
    /// <summary>
    /// Class ReturnVisitItemViewModel
    /// </summary>
    public class ReturnVisitItemViewModel : INotifyPropertyChanged
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
        private string _lineOne;
        /// <summary>
        /// The _line three
        /// </summary>
        private string _lineThree;
        /// <summary>
        /// The _line two
        /// </summary>
        private string _lineTwo;

        /// <summary>
        /// The _name
        /// </summary>
        private string _name;

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
        public string Name
        {
            get { return _name; }
            set
            {
                if (value != _name) {
                    _name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <value>The line one.</value>
        /// <returns></returns>
        public string LineOne
        {
            get { return _lineOne; }
            set
            {
                if (value != _lineOne) {
                    _lineOne = value;
                    NotifyPropertyChanged("LineOne");
                }
            }
        }

        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <value>The line two.</value>
        /// <returns></returns>
        public string LineTwo
        {
            get { return _lineTwo; }
            set
            {
                if (value != _lineTwo) {
                    _lineTwo = value;
                    NotifyPropertyChanged("LineTwo");
                }
            }
        }

        /// <summary>
        /// Gets or sets the line three.
        /// </summary>
        /// <value>The line three.</value>
        public string LineThree
        {
            get { return _lineThree; }
            set
            {
                if (_lineThree != value) {
                    _lineThree = value;
                    NotifyPropertyChanged("LineThree");
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