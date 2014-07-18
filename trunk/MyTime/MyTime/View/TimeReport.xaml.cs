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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
//using System.Windows.Controls.DataVisualization.Charting;
//xmlns:charting="clr-namespace:System.Windows.Controls.DataVisualization.Charting;assembly=System.Windows.Controls.DataVisualization.Toolkit" 
using System.Windows.Navigation;
using Windows.Foundation.Metadata;
using FieldService.Model;
using FieldService.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MyTimeDatabaseLib;
using Telerik.Charting;
using Telerik.Windows.Controls;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace FieldService.View
{
	/// <summary>
	/// Class TimeReport
	/// </summary>
	public partial class TimeReport : PhoneApplicationPage
	{
		private DateTime _fromDate;
		private DateTime _toDate;


	        /// <summary>
	        /// Initializes a new instance of the <see cref="TimeReport" /> class.
	        /// </summary>
	        public TimeReport()
	        {
	                DataContext = new TimeReportViewModel();

                        ((TimeReportViewModel) DataContext).TimeReportMajorStep = ((TimeReportViewModel)DataContext).icReport.Count > 1 ? 10 : 2;

                        InitializeComponent();
	        }

	        #region Events

		/// <summary>
		/// Handles the Loaded event of the PhoneApplicationPage control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
		private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
		{
                        tbDisclaimer.Visibility = ((TimeReportViewModel)DataContext).icReport.Count > 1 ? Visibility.Visible : Visibility.Collapsed;

			var menuItem = ApplicationBar.MenuItems[0] as ApplicationBarMenuItem;
			if (menuItem != null) {
				menuItem.Text = StringResources.ReportingPage_Share;
			}
		}


		/// <summary>
		/// Handles the SelectionChanged event of the lbEntries control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="SelectionChangedEventArgs" /> instance containing the event data.</param>
		private void lbEntries_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (lbEntries.SelectedIndex < 0) return;
			var selectedTimeEntry = lbEntries.SelectedItem as TimeReportEntryViewModel;
			if (selectedTimeEntry == null) return;
			NavigationService.Navigate(new Uri(selectedTimeEntry.EditLink, UriKind.Relative));
			lbEntries.SelectedIndex = -1;
		}

		private void miShareReport_Click(object sender, EventArgs e)
		{
			//
			//var entries = new ChartData();

			string body = string.Empty;
                        foreach (TimeReportSummaryModel entry in ((TimeReportViewModel)DataContext).icReport) {
				body += string.Format(StringResources.Reporting_ReportHeader, entry.Month);
                                body += string.Format(StringResources.MainPage_Report_Hours, (entry.Minutes/60.0));
                                body += entry.Magazines > 0
                                        ? string.Format(StringResources.MainPage_Report_Mags, entry.Magazines)
                                        : string.Empty;
                                body += entry.Books > 0
                                        ? string.Format(StringResources.MainPage_Report_Books, entry.Books)
                                        : string.Empty;
                                body += entry.Brochures > 0
                                        ? string.Format(StringResources.MainPage_Report_Brochures, entry.Brochures)
                                        : string.Empty;
                                body += entry.Tracts > 0
                                        ? string.Format(StringResources.MainPage_Report_Tracts, entry.Tracts)
                                        : string.Empty;
                                body += entry.ReturnVisits > 0
                                        ? string.Format(StringResources.MainPage_Report_RVs, entry.ReturnVisits)
                                        : string.Empty;
                                body += entry.BibleStudies > 0
                                        ? string.Format(StringResources.MainPage_Report_BibleStudies, entry.BibleStudies)
                                        : string.Empty;
                                body += entry.RBCHours > 0
                                        ? string.Format(StringResources.MainPage_Report_AuxHours, entry.RBCHours)
                                        : string.Empty;
				body += "\n\n";
			}

			Reporting.SendReport(body);
		}

		#endregion

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			try {
				string fromStr = string.Empty;
				string toStr = string.Empty;
				if (!NavigationContext.QueryString.TryGetValue("from", out fromStr) || !NavigationContext.QueryString.TryGetValue("to", out toStr))
					NavigationService.GoBack();
				_fromDate = DateTime.ParseExact(fromStr, "MM-dd-yyyy", CultureInfo.InvariantCulture);
				_toDate = DateTime.ParseExact(toStr, "MM-dd-yyyy", CultureInfo.InvariantCulture);
			    try {
			        string header = StringResources.ApplicationName;
			        NavigationContext.QueryString.TryGetValue("header", out header);
			        pMainPivot.Title = (header ?? string.Format("{0:d} - {1:d}",_fromDate,_toDate)).ToUpper();
			    } catch {}
				RefreshTimeReport();
			} catch (Exception ee) {
				NavigationService.GoBack();
			}
		}

		private void RefreshTimeReport()
		{
                        tbFromDate.Text = string.Format(StringResources.ReportingPage_Report_From, _fromDate.ToShortDateString());
                        tbToDate.Text = string.Format(StringResources.ReportingPage_Report_To, _toDate.ToShortDateString());

		        var bw = new BackgroundWorker();
		        bw.RunWorkerCompleted +=
		                (sender, args) =>
		                        ((TimeReportViewModel) this.DataContext).LoadTimeReport(Reporting.BuildTimeReport(_fromDate,
		                                _toDate, SortOrder.DateOldestToNewest));
                        bw.RunWorkerAsync();

		}

	    private void ChartTrackBallBehavior_OnTrackInfoUpdated(object sender, TrackBallInfoEventArgs e)
	    {
	            e.Header = "";
	            foreach (var info in e.Context.DataPointInfos)
	            {
	                    var dp = info.DataPoint as CategoricalDataPoint;
	                    info.DisplayHeader = dp.Category+":";
                            info.DisplayContent = string.Format(StringResources.TimeReport_HoursAndMinutes, ((int)dp.Value), (60 * (dp.Value - ((int)dp.Value))));
	            }
	    }

	    private void MyChart_OnTap(object sender, GestureEventArgs e)
	    {
	            //throw new NotImplementedException();
	    }

	    private void UIElement_OnTap(object sender, GestureEventArgs e)
	    {
	        var bw = new BackgroundWorker();
	        bw.DoWork += (o, args) => Thread.Sleep(1000);
	        bw.RunWorkerCompleted += (o, args) => RefreshTimeReport();

            bw.RunWorkerAsync();
	    }
	}





        //// Class for storing activities
        ///// <summary>
        ///// Class ChartData
        ///// </summary>
        //public class ChartData : List<TimeChartInfo>
        //{
        //        /// <summary>
        //        /// Initializes a new instance of the <see cref="ChartData" /> class.
        //        /// </summary>
        //        public ChartData()
        //        {
        //                if (((TimeReportViewModel)DataContext).icReport.Count > 1) {
        //                        foreach (TimeReportSummaryModel v in App.ViewModel.icReport) {
        //                                Add(new TimeChartInfo { Header = new String(v.Month.Take(3).ToArray()), Time = (v.Minutes / 60.0) });
        //                        }
        //                } else {
        //                        foreach (TimeReportEntryViewModel v in App.ViewModel.lbTimeEntries) {
        //                                Add(new TimeChartInfo { Header = string.Format("{0}/{1}", v.Date.Month, v.Date.Day), Time = (v.Minutes / 60.0) });
        //                        }
        //                }
        //        }
        //}

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
}