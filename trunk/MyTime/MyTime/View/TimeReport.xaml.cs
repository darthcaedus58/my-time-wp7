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
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Navigation;
using FieldService.Model;
using FieldService.ViewModels;
using Microsoft.Phone.Controls;
using MyTimeDatabaseLib;

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
			DataContext = App.ViewModel;
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
			if (App.ViewModel.icReport.Count > 1) {
				tbDisclaimer.Text = "Reminder: Service Year Begins in September.";
			}
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
			var selectedTimeEntry = lbEntries.SelectedItem as TimeReportEntryViewModel;
			if (selectedTimeEntry == null) return;
			NavigationService.Navigate(new Uri(selectedTimeEntry.EditLink, UriKind.Relative));
			lbEntries.SelectedIndex = -1;
		}

		private void miShareReport_Click(object sender, EventArgs e)
		{
			//
			var entries = new ChartData();

			string body = string.Empty;
			foreach (TimeReportSummaryModel entry in App.ViewModel.icReport) {
				body += string.Format("Report for {0}:\n\n", entry.Month);
				body += string.Format("Hours:\t\t{0:0.00}\n", (entry.Minutes/60.0));
				body += entry.Magazines > 0 ? string.Format("Magazines:\t{0}\n", entry.Magazines) : string.Empty;
				body += entry.Books > 0 ? string.Format("Books:\t\t{0}\n", entry.Books) : string.Empty;
				body += entry.Brochures > 0 ? string.Format("Brochures:\t{0}\n", entry.Brochures) : string.Empty;
				body += entry.ReturnVisits > 0 ? string.Format("Return Visits:\t{0}\n", entry.ReturnVisits) : string.Empty;
				body += entry.BibleStudies > 0 ? string.Format("Bible Studies:\t{0}\n", entry.BibleStudies) : string.Empty;
				body += entry.RBCHours > 0 ? string.Format("R/B/C Hours:\t{0}", entry.RBCHours) : string.Empty;
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
				RefreshTimeReport();
			} catch (Exception ee) {
				NavigationService.GoBack();
			}
		}

		private void RefreshTimeReport()
		{
			App.ViewModel.LoadTimeReport(Reporting.BuildTimeReport(_fromDate, _toDate, SortOrder.DateOldestToNewest));

			tbFromDate.Text = string.Format("FROM:\t\t{0}", _fromDate.ToShortDateString());
			tbToDate.Text = string.Format("TO:\t\t\t{0}", _toDate.ToShortDateString());

			myChart.Series.Clear();
			myChart.Series.Add(new LineSeries {
				                                  ItemsSource = new ChartData(),
				                                  DependentValuePath = "Time",
				                                  IndependentValuePath = "Header",
				                                  Title = "Time in Hours",
			                                  });
		}
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
				foreach (TimeReportSummaryModel v in App.ViewModel.icReport) {
					Add(new TimeChartInfo {Header = new String(v.Month.Take(3).ToArray()), Time = (v.Minutes/60.0)});
				}
			} else {
				foreach (TimeReportEntryViewModel v in App.ViewModel.lbTimeEntries) {
					Add(new TimeChartInfo {Header = string.Format("{0}/{1}", v.Date.Month, v.Date.Day), Time = (v.Minutes/60.0)});
				}
			}
		}
	}
}