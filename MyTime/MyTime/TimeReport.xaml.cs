// ***********************************************************************
// Assembly         : MyTime
// Author           : trevo_000
// Created          : 11-06-2012
//
// Last Modified By : trevo_000
// Last Modified On : 11-06-2012
// ***********************************************************************
// <copyright file="TimeReport.xaml.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using FieldService.ViewModels;
using Microsoft.Phone.Controls;

namespace FieldService
{
    /// <summary>
    /// Class TimeReport
    /// </summary>
    public partial class TimeReport : PhoneApplicationPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeReport" /> class.
        /// </summary>
        public TimeReport()
        {
            InitializeComponent();
            DataContext = App.ViewModel;
        }

        #region Events

        /// <summary>
        /// Handles the Loaded event of the PhoneApplicationPage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            myChart.InvalidateMeasure();
            myChart.InvalidateArrange();
            myChart.UpdateLayout();
        }

        /// <summary>
        /// Handles the SelectionChanged event of the lbEntries control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs" /> instance containing the event data.</param>
        private void lbEntries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbEntries.SelectedIndex < 0) return;
            NavigationService.Navigate(new Uri("/ManuallyEnterTime.xaml?id=" + ((TimeReportEntryViewModel) lbEntries.SelectedItem).ItemId.ToString(CultureInfo.InvariantCulture), UriKind.Relative));
            lbEntries.SelectedIndex = -1;
        }

        #endregion
    }

    /// <summary>
    /// Class TimeChartInfo
    /// </summary>
    public class TimeChartInfo
    {
        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>The header.</value>
        public string Header { get; set; }
        /// <summary>
        /// Gets or sets the time.
        /// </summary>
        /// <value>The time.</value>
        public double Time { get; set; }
    }

    // Class for storing activities
    /// <summary>
    /// Class ChartData
    /// </summary>
    public class ChartData : List<TimeChartInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartData" /> class.
        /// </summary>
        public ChartData()
        {
            if (App.ViewModel.icReport.Count > 1) {
                foreach (TimeReportSummaryViewModel v in App.ViewModel.icReport) {
                    Add(new TimeChartInfo {Header = new String(v.Month.Take(3).ToArray()), Time = (v.Minutes/60.0)});
                }
            } else {
                foreach (TimeReportEntryViewModel v in App.ViewModel.lbTimeEntries) {
                    DateTime d = DateTime.Parse(v.Date);
                    Add(new TimeChartInfo {Header = string.Format("{0}/{1}", d.Month, d.Day), Time = (v.Minutes/60.0)});
                }
            }
        }
    }
}