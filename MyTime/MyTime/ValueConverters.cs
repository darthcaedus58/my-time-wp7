﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Data;

namespace FieldService
{
        public class DateTimeToShortTimeValueConverter : IValueConverter
        {
                public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
                {
                        return ((DateTime) value).ToShortTimeString();
                }

                public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
                {
                        return DateTime.ParseExact(
                                value.ToString(), Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortTimePattern, CultureInfo.CurrentCulture);
                }
        }
	public class DoubleToVisibilityValueConverter : IValueConverter
	{
		/// <summary>
		/// Modifies the source data before passing it to the target for display in the UI.
		/// </summary>
		/// <returns>
		/// The value to be passed to the target dependency property.
		/// </returns>
		/// <param name="value">The source data being passed to the target.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) { return (double) value > 0 ? Visibility.Visible : Visibility.Collapsed; }

		/// <summary>
		/// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
		/// </summary>
		/// <returns>
		/// The value to be passed to the source object.
		/// </returns>
		/// <param name="value">The target data being passed to the source.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { return (Visibility) value == Visibility.Collapsed ? (double)0.0 : (double)1.0; }
	}
	public class MinutesToTimeSpanValueConverter : IValueConverter
	{
		/// <summary>
		/// Modifies the source data before passing it to the target for display in the UI.
		/// </summary>
		/// <returns>
		/// The value to be passed to the target dependency property.
		/// </returns>
		/// <param name="value">The source data being passed to the target.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return new TimeSpan(0, (int)value, 0);
		}

		/// <summary>
		/// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
		/// </summary>
		/// <returns>
		/// The value to be passed to the source object.
		/// </returns>
		/// <param name="value">The target data being passed to the source.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (int)((TimeSpan)value).TotalMinutes;
		}
	}
}
