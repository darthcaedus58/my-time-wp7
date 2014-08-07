using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using FieldService.Annotations;
using FieldService.Model;
using FieldService.View;
using MyTimeDatabaseLib;

namespace FieldService.ViewModels
{
    class TimeReportViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the ic report.
        /// </summary>
        /// <value>The ic report.</value>
        public ObservableCollection<TimeReportSummaryModel> icReport { get; private set; }

        /// <summary>
        /// Gets the lb time entries.
        /// </summary>
        /// <value>The lb time entries.</value>
        public ObservableCollection<TimeReportEntryViewModel> lbTimeEntries { get; private set; }

        public ObservableCollection<TimeChartInfo> TimeReportChartData { get; private set; }

        private int _timeReportTotal = 0;
        private int _timeReportMajorStep;
        private int _timeReportPeriod;

        public double TimeReportGaugeValue
        {
            get { return _timeReportTotal / 60.0; }
        }

        public double TimeReportGaugeGoalValue
        {
            get { return App.Settings.TimeGoalPerMonth * TimeReportPeriod; }
        }

        public string TimeReportTotal
        {
            get { return string.Format(StringResources.ReportingPage_Report_RemainingText2, _timeReportTotal / 60, _timeReportTotal % 60 > 0 ? _timeReportTotal % 60 : 0); }
        }

        public string TimeReportGoalText
        {
            get
            {
                return string.Format(StringResources.ReportingPage_Report_GoalText2, TimeReportGaugeGoalValue,
                    TimeReportPeriod);
            }
        }

        public string TimeReportRemainingText
        {
            get
            {
                int remaining = ((int)TimeReportGaugeGoalValue*60) - _timeReportTotal;
                if (remaining < 0) remaining = 0;
                return string.Format(StringResources.ReportingPage_Report_RemainingText2,
                    remaining/60, remaining%60 > 0 ? remaining%60 : 0);
            }
        }

        public int TimeReportMajorStep
        {
            get { return _timeReportMajorStep; }
            set
            {
                _timeReportMajorStep = value;
                OnPropertyChanged("TimeReportMajorStep");
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is time report data loaded.
        /// </summary>
        /// <value><c>true</c> if this instance is time report data loaded; otherwise, <c>false</c>.</value>
        public bool IsTimeReportDataLoading { get; private set; }

        public int TimeReportPeriod
        {
            get { return _timeReportPeriod; }
            set
            {
                _timeReportPeriod = value;
                OnPropertyChanged("TimeReportPeriod");
                OnPropertyChanged("TimeReportGaugeGoalValue");
                OnPropertyChanged("TimeReportRemainingText");
            }
        }

        public TimeReportViewModel()
        {

            icReport = new ObservableCollection<TimeReportSummaryModel>();
            lbTimeEntries = new ObservableCollection<TimeReportEntryViewModel>();
            TimeReportChartData = new ObservableCollection<TimeChartInfo>();
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Loads the time report.
        /// </summary>
        /// <param name="entries">The entries.</param>
        public void LoadTimeReport(TimeData[] entries)
        {
            if (!IsTimeReportDataLoading) {
                IsTimeReportDataLoading = true;
                OnPropertyChanged("IsTimeReportDataLoading");
                icReport.Clear();
                lbTimeEntries.Clear();
                _timeReportTotal = 0;
            }


            int summaryMinTotal = 0;
            if (entries.Length <= 0) return;

            int month = entries[0].Date.Month;
            int year = entries[0].Date.Year;
            var summary = new TimeReportSummaryModel();
            var rtdEntries = new List<RBCTimeData>();
            //ListBox<int> rvList = new ListBox<int>();
            foreach (TimeData td in entries) {	//total regular time entries

                //build total number for the report.
                _timeReportTotal += td.Minutes;


                //summary data start
                if (month != td.Date.Month) {
                    summary.Time = string.Format(StringResources.TimeReport_HoursAndMinutes, (summaryMinTotal / 60), summaryMinTotal > 0 ? summaryMinTotal % 60 : 0);
                    summary.Minutes = summaryMinTotal;
                    summary.RBCHours = (RBCTimeDataInterface.GetMonthRBCTimeTotal(new DateTime(year, month, 1))) / 60.0;
                    RBCTimeData[] eee = RBCTimeDataInterface.GetRBCTimeEntries(new DateTime(year, month, 1), new DateTime(year, month, 1).AddMonths(1).AddDays(-1), SortOrder.DateNewestToOldest);
                    if (eee != null) rtdEntries.AddRange(eee);
                    icReport.Add(summary);
                    summary = new TimeReportSummaryModel();
                    month = td.Date.Month;
                    year = td.Date.Year;
                    summaryMinTotal = 0;
                }
                summary.Month = td.Date.ToString("MMMM").ToUpper();
                summary.Days++;
                summaryMinTotal += td.Minutes;
                summary.Magazines += td.Magazines;
                summary.BibleStudies += td.BibleStudies;
                summary.Books += td.Books;
                summary.Brochures += td.Brochures;
                summary.ReturnVisits += td.ReturnVisits;
                summary.Tracts += td.Tracts;
                //summary data end.

                //add the time data to the entries page.
                lbTimeEntries.Add(new TimeReportEntryViewModel {
                    Date = td.Date,
                    Hours = string.Format(StringResources.TimeReport_HoursAndMinutes, (td.Minutes / 60), td.Minutes > 0 ? td.Minutes % 60 : 0),
                    ItemId = td.ItemId,
                    Minutes = td.Minutes,
                    EditLink = string.Format("/View/RegularTime.xaml?id={0}", td.ItemId),
                    Notes = td.Notes,
                    MagazinesCount = td.Magazines,
                    BrochuresCount = td.Brochures,
                    BooksCount = td.Books,
                    TractsCount = td.Tracts,
                    RVsCount = td.ReturnVisits,
                    Type = TimeType.Regular
                });
            }

            RBCTimeData[] ee = RBCTimeDataInterface.GetRBCTimeEntries(new DateTime(year, month, 1), new DateTime(year, month, 1).AddMonths(1).AddDays(-1), SortOrder.DateNewestToOldest);
            if (ee != null) rtdEntries.AddRange(ee);
            summary.RBCHours = (RBCTimeDataInterface.GetMonthRBCTimeTotal(new DateTime(year, month, 1)) / 60.0);
            summary.Time = string.Format(StringResources.TimeReport_HoursAndMinutes, (summaryMinTotal / 60), summaryMinTotal > 0 ? summaryMinTotal % 60 : 0);
            summary.Minutes = summaryMinTotal;
            icReport.Add(summary);

            foreach (RBCTimeData e in rtdEntries) {
                //add time 
                _timeReportTotal += e.Minutes;
                lbTimeEntries.Add(new TimeReportEntryViewModel {
                    Date = e.Date,
                    Hours = string.Format(StringResources.TimeReport_AuxHoursAndMinutes, (int)e.Hours, e.Minutes > 0 ? e.Minutes % 60 : 0),
                    ItemId = e.ItemID,
                    Minutes = e.Minutes,
                    EditLink = string.Format("/View/RBCTime.xaml?id={0}", e.ItemID),
                    Notes = e.Notes,
                    MagazinesCount = -1,
                    BrochuresCount = -1,
                    BooksCount = -1,
                    RVsCount = -1,
                    TractsCount = -1,
                    Type = TimeType.Auxiliary
                });
            }
            TimeReportEntryViewModel[] lte = lbTimeEntries.OrderBy(s => s.Date.Date).ToArray();
            lbTimeEntries.Clear();
            foreach (TimeReportEntryViewModel l in lte) lbTimeEntries.Add(l);

            TimeReportChartData = new ObservableCollection<TimeChartInfo>();
            if (icReport.Count > 1) {
                foreach (TimeReportSummaryModel v in icReport) {
                    TimeReportChartData.Add(new TimeChartInfo { Header = new String(v.Month.Take(3).ToArray()), Time = (v.Minutes / 60.0) });
                }
            } else {
                foreach (TimeReportEntryViewModel v in lbTimeEntries) {
                    TimeReportChartData.Add(new TimeChartInfo { Header = string.Format("{0}/{1}", v.Date.Month, v.Date.Day), Time = (v.Minutes / 60.0) });
                }
            }
            OnPropertyChanged("TimeReportTotal");
            OnPropertyChanged("TimeReportGaugeValue");
            OnPropertyChanged("TimeReportGaugeGoalValue");
            OnPropertyChanged("TimeReportGoalText");
            OnPropertyChanged("TimeReportRemainingText");
            IsTimeReportDataLoading = false;
            OnPropertyChanged("IsTimeReportDataLoading");
            OnPropertyChanged("TimeReportChartData");
        }
    }
}
