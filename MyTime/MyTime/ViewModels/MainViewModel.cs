using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using MyTime.ViewModels;
using MyTimeDatabaseLib;
using System.IO;
using System.Linq;


namespace MyTime
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            lbRvPreviousItems = new ObservableCollection<PreviousVisitViewModel>();
            lbRvItems = new ObservableCollection<ReturnVisitItemViewModel>();
            lbMainMenuItems = new ObservableCollection<MainMenuViewModel>();
            icReport = new ObservableCollection<TimeReportSummaryViewModel>();
            lbTimeEntries = new ObservableCollection<TimeReportEntryViewModel>();
        }

        /// <summary>
        /// A collection for ReturnVisitItemViewModel objects.
        /// </summary>
        public ObservableCollection<ReturnVisitItemViewModel> lbRvItems { get; private set; }
        public ObservableCollection<MainMenuViewModel> lbMainMenuItems { get; private set; }
        public ObservableCollection<PreviousVisitViewModel> lbRvPreviousItems { get; private set; }
        public ObservableCollection<TimeReportSummaryViewModel> icReport { get; private set; }
        public ObservableCollection<TimeReportEntryViewModel> lbTimeEntries { get; private set; }

        public bool IsRvDataLoaded
        {
            get;
            private set;
        }

        public bool IsTimeReportDataLoaded
        {
            get;
            private set;
        }

        public void UpdatePreviousVisits(MyTimeDatabaseLib.RvPreviousVisitData[] visits)
        {
            foreach (var v in visits) {
                string placements = string.Empty;
                lbRvPreviousItems.Add(new PreviousVisitViewModel() { 
                                                                    LastVisitDate = v.Date.ToShortDateString(), 
                                                                    ItemId = v.ItemId, 
                                                                    Placements = string.Format("{0} Mg's, {1} Bk's, {2} Bro's", v.Magazines, v.Books, v.Brochures),
                                                                    Description = v.Notes
                                                                   }
                                      );
            }
        }

        /// <summary>
        /// Creates and adds a few ReturnVisitItemViewModel objects into the lbRvItems collection.
        /// </summary>
        public void LoadReturnVisitList()
        {
            // Sample data; replace with real data
            var rvs = ReturnVisitsInterface.GetReturnVisits(SortOrder.DateOldestToNewest, 25);

            var wb = new System.Windows.Media.Imaging.WriteableBitmap(100, 100);
            for(int i = 0; i < wb.Pixels.Length; i++) {
                wb.Pixels[i] = (int)0xFF3300;
            }
            BitmapImage bmp = new BitmapImage();
            using (MemoryStream ms = new MemoryStream()) {
                wb.SaveJpeg(ms, 100, 100, 0, 100);
                bmp.SetSource(ms);
            }

            foreach (var r in rvs) {
                var bi = new BitmapImage();
                if (r.ImageSrc != null && r.ImageSrc.Length >= 0) {
                    var ris = new WriteableBitmap(100, 100);
                    for (int i = 0; i < ris.Pixels.Length; i++) {
                        ris.Pixels[i] = r.ImageSrc[i];
                    }
                        using (var ms = new MemoryStream()) {
                            ris.SaveJpeg(ms, 100, 100, 0, 100);
                            bi.SetSource(ms);
                        }
                } else {
                    bi = bmp;
                }
                lbRvItems.Add(new ReturnVisitItemViewModel() { ItemId = r.ItemId, ImageSource = bi, Name = string.IsNullOrEmpty(r.FullName) ? string.Format("{0} year old {1}",r.Age, r.Gender) : r.FullName, LineOne = string.Format("{0} {1}", r.AddressOne, r.AddressTwo), LineTwo = string.Format("{0}, {1} {2}", r.City, r.StateProvince, r.PostalCode) });
            }
            this.IsRvDataLoaded = true;
        }

        public void LoadMainMenu()
        {
            lbMainMenuItems.Add(new MainMenuViewModel() { MenuText = "add time", MenuItemName = "miAddTime", IconUri = "/icons/add.png", MenuImageName = "AddTimeImage" });
            lbMainMenuItems.Add(new MainMenuViewModel() { MenuText = "add return visit", MenuItemName = "miAddRv", IconUri = "/icons/phone.png", MenuImageName = "AddReturnVisitImage" });
            lbMainMenuItems.Add(new MainMenuViewModel() { MenuText = "find nearest rv", MenuItemName = "miFindNearRv", IconUri = "/icons/search.png", MenuImageName = "FindNearestRv" });
            lbMainMenuItems.Add(new MainMenuViewModel() { MenuText = "send service report", MenuItemName = "miSendReport", IconUri = "/icons/mail.png", MenuImageName = "SemdReportImage" });
            lbMainMenuItems.Add(new MainMenuViewModel() { MenuText = string.Format("{0} report",DateTime.Today.ToString("MMMM").ToLower()), MenuItemName = "miThisMonthReport", IconUri = "/icons/favs.png", MenuImageName = "ThisMonthImage" });
            lbMainMenuItems.Add(new MainMenuViewModel() { MenuText = string.Format("{0} report",DateTime.Today.Year), MenuItemName = "miThisYearReport", IconUri = "/icons/favs.png", MenuImageName = "ThisYearImage" });
            lbMainMenuItems.Add(new MainMenuViewModel() { MenuText = "custom report", MenuItemName = "miCustomReport", IconUri = "/icons/search.png", MenuImageName = "CustomReportImage" });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void LoadTimeReport(TimeData[] entries)
        {
            int minutes = 0;
            int month = 0;
            if (entries.Length <= 0) return;
            if (IsTimeReportDataLoaded) {
                IsTimeReportDataLoaded = false;
                icReport.Clear();
                lbTimeEntries.Clear();
            }
            month = entries[0].Date.Month;
            var summary = new TimeReportSummaryViewModel();
            foreach (var td in entries) {
                if (month != td.Date.Month) {
                    summary.Time = string.Format("{0:0.00} Hour(s)", ((double) minutes/60.0));
                    summary.Minutes = minutes;
                    icReport.Add(summary);
                    summary = new TimeReportSummaryViewModel();
                    month = td.Date.Month;
                    minutes = 0;
                }
                summary.Month = td.Date.ToString("MMMM").ToUpper();
                summary.Days++;
                minutes += td.Minutes;
                summary.Magazines += td.Magazines;
                summary.BibleStudies += td.BibleStudies;
                summary.Books += td.Books;
                summary.Brochures += td.Brochures;
                summary.ReturnVisits += td.ReturnVisits;

                lbTimeEntries.Add(new TimeReportEntryViewModel() {
                                                                     Date = td.Date.ToLongDateString(),
                                                                     Hours = string.Format("{0:0.00} Hour(s)", ((double)td.Minutes/60.0)),
                                                                     ItemId = td.ItemId,
                                                                     Minutes = td.Minutes
                                                                 });
            }
            summary.Time = string.Format("{0:0.00} Hour(s)", ((double)minutes / 60.0));
            summary.Minutes = minutes;
            icReport.Add(summary);
            IsTimeReportDataLoaded = true;
        }
    }
}