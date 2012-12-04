// ***********************************************************************
// Assembly         : MyTime
// Author           : trevo_000
// Created          : 11-03-2012
//
// Last Modified By : trevo_000
// Last Modified On : 11-11-2012
// ***********************************************************************
// <copyright file="MainViewModel.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Marketplace;
using MyTimeDatabaseLib;

namespace FieldService.ViewModels
{
    /// <summary>
    /// Class MainViewModel
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel" /> class.
        /// </summary>
        public MainViewModel()
        {
            lbRvPreviousItems = new ObservableCollection<PreviousVisitViewModel>();
            lbRvItems = new ObservableCollection<ReturnVisitItemViewModel>();
            lbMainMenuItems = new ObservableCollection<MainMenuViewModel>();
            icReport = new ObservableCollection<TimeReportSummaryViewModel>();
            lbTimeEntries = new ObservableCollection<TimeReportEntryViewModel>();

            llReturnVisitFullListCategory = new ObservableCollection<ReturnVistLLCategory>();
        }

        /// <summary>
        /// Gets the ll return visit full list category.
        /// </summary>
        /// <value>The ll return visit full list category.</value>
        public ObservableCollection<ReturnVistLLCategory> llReturnVisitFullListCategory { get; private set; }


        /// <summary>
        /// A collection for ReturnVisitItemViewModel objects.
        /// </summary>
        /// <value>The lb rv items.</value>
        public ObservableCollection<ReturnVisitItemViewModel> lbRvItems { get; private set; }

        /// <summary>
        /// Gets the lb main menu items.
        /// </summary>
        /// <value>The lb main menu items.</value>
        public ObservableCollection<MainMenuViewModel> lbMainMenuItems { get; private set; }

        /// <summary>
        /// Gets the lb rv previous items.
        /// </summary>
        /// <value>The lb rv previous items.</value>
        public ObservableCollection<PreviousVisitViewModel> lbRvPreviousItems { get; private set; }

        /// <summary>
        /// Gets the ic report.
        /// </summary>
        /// <value>The ic report.</value>
        public ObservableCollection<TimeReportSummaryViewModel> icReport { get; private set; }

        /// <summary>
        /// Gets the lb time entries.
        /// </summary>
        /// <value>The lb time entries.</value>
        public ObservableCollection<TimeReportEntryViewModel> lbTimeEntries { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is rv data loaded.
        /// </summary>
        /// <value><c>true</c> if this instance is rv data loaded; otherwise, <c>false</c>.</value>
        public bool IsRvDataLoaded { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is time report data loaded.
        /// </summary>
        /// <value><c>true</c> if this instance is time report data loaded; otherwise, <c>false</c>.</value>
        public bool IsTimeReportDataLoaded { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is main menu loaded.
        /// </summary>
        /// <value><c>true</c> if this instance is main menu loaded; otherwise, <c>false</c>.</value>
        public bool IsMainMenuLoaded { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is previous visits loaded.
        /// </summary>
        /// <value><c>true</c> if this instance is previous visits loaded; otherwise, <c>false</c>.</value>
        public bool IsPreviousVisitsLoaded { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is rv full list loaded.
        /// </summary>
        /// <value><c>true</c> if this instance is rv full list loaded; otherwise, <c>false</c>.</value>
        public bool IsRvFullListLoaded { get; private set; }

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Occurs when [property changed].
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        /// <summary>
        /// Updates the previous visits.
        /// </summary>
        /// <param name="visits">The visits.</param>
        public void UpdatePreviousVisits(RvPreviousVisitData[] visits)
        {
            if (IsPreviousVisitsLoaded) lbRvPreviousItems.Clear();
            IsPreviousVisitsLoaded = false;
            foreach (RvPreviousVisitData v in visits) {
                lbRvPreviousItems.Add(new PreviousVisitViewModel {
                                                                     LastVisitDate = v.Date.ToShortDateString(),
                                                                     ItemId = v.ItemId,
                                                                     Placements = string.Format("{0} Mg's, {1} Bk's, {2} Bro's", v.Magazines, v.Books, v.Brochures),
                                                                     Description = v.Notes
                                                                 }
                    );
            }
            IsPreviousVisitsLoaded = true;
        }

        /// <summary>
        /// Creates and adds a few ReturnVisitItemViewModel objects into the lbRvItems collection.
        /// </summary>
        /// <param name="so">The so.</param>
        public void LoadReturnVisitList(SortOrder so)
        {
            if (IsRvDataLoaded) lbRvItems.Clear();
            IsRvDataLoaded = false;
            // Sample data; replace with real data
            ReturnVisitData[] rvs = ReturnVisitsInterface.GetReturnVisits(so);

            var wb = new WriteableBitmap(100, 100);
            for (int i = 0; i < wb.Pixels.Length; i++) {
                wb.Pixels[i] = 0xFF3300;
            }
            var bmp = new BitmapImage();
            using (var ms = new MemoryStream()) {
                wb.SaveJpeg(ms, 100, 100, 0, 100);
                bmp.SetSource(ms);
            }

            foreach (ReturnVisitData r in rvs) {
                var bi = new BitmapImage();
                if (r.ImageSrc != null && r.ImageSrc.Length >= 0) {
                    var ris = new WriteableBitmap(450, 250);

                    //get image from database
                    for (int i = 0; i < r.ImageSrc.Length; i++) {
                        ris.Pixels[i] = r.ImageSrc[i];
                    }

                    //put the image in a WritableBitmap
                    using (var ms = new MemoryStream()) {
                        ris.SaveJpeg(ms, 450, 250, 0, 100);
                        bi.SetSource(ms);
                    }

                    //crop the image to 100x100 and centered
                    var img = new Image {
                                            Source = bi,
                                            Width = 450,
                                            Height = 250
                                        };
                    var wb2 = new WriteableBitmap(100, 100);
                    var t = new CompositeTransform {
                                                       ScaleX = 0.5,
                                                       ScaleY = 0.5,
                                                       TranslateX = -((450/2)/2 - 50),
                                                       TranslateY = -((250/2)/2 - 50)
                                                   };
                    wb2.Render(img, t);
                    wb2.Invalidate();
                    bi = new BitmapImage();
                    using (var ms = new MemoryStream()) {
                        wb2.SaveJpeg(ms, 100, 100, 0, 100);
                        bi.SetSource(ms);
                    }
                    //BitmapImage bi is now cropped
                } else {
                    bi = bmp; //Default image.
                }
                string lv = r.LastVisitDate == DateTime.MinValue ? "No visit recorded" : string.Format("{0} day(s) since last visit", (DateTime.Now - r.LastVisitDate).Days);
                lbRvItems.Add(new ReturnVisitItemViewModel {
                                                               ItemId = r.ItemId,
                                                               ImageSource = bi,
                                                               Name = string.IsNullOrEmpty(r.FullName) ? string.Format("{0} year old {1}", r.Age, r.Gender) : r.FullName,
                                                               LineOne = string.Format("{0} {1}", r.AddressOne, r.AddressTwo),
                                                               LineTwo = string.Format("{0}, {1} {2}", r.City, r.StateProvince, r.PostalCode),
                                                               LineThree = lv
                                                           });
            }
            IsRvDataLoaded = true;
        }

        /// <summary>
        /// Loads the main menu.
        /// </summary>
        public void LoadMainMenu()
        {
            if (IsMainMenuLoaded) lbMainMenuItems.Clear();
            IsMainMenuLoaded = false;
            lbMainMenuItems.Add(new MainMenuViewModel {
                                                          MenuText = "add time",
                                                          MenuItemName = "miAddTime",
                                                          IconUri = "/icons/clock.png",
                                                          MenuImageName = "AddTimeImage"
                                                      });

            lbMainMenuItems.Add(new MainMenuViewModel {
                                                          MenuText = "add return visit",
                                                          MenuItemName = "miAddRv",
                                                          IconUri = "/icons/add-user.png",
                                                          MenuImageName = "AddReturnVisitImage"
                                                      });
            lbMainMenuItems.Add(new MainMenuViewModel {MenuText = "send service report", MenuItemName = "miSendReport", IconUri = "/icons/message.png", MenuImageName = "SemdReportImage"});
            lbMainMenuItems.Add(new MainMenuViewModel {MenuText = string.Format("{0} report", DateTime.Today.ToString("MMMM").ToLower()), MenuItemName = "miThisMonthReport", IconUri = "/icons/calendar.png", MenuImageName = "ThisMonthImage"});
            lbMainMenuItems.Add(new MainMenuViewModel {MenuText = "service year report", MenuItemName = "miThisYearReport", IconUri = "/icons/calendar.png", MenuImageName = "ThisYearImage"});
            //lbMainMenuItems.Add(new MainMenuViewModel {MenuText = "custom report", MenuItemName = "miCustomReport", IconUri = "/icons/search.png", MenuImageName = "CustomReportImage"});

            lbMainMenuItems.Add(new MainMenuViewModel {MenuText = "watchtower library", MenuItemName = "miWtLib", IconUri = "/icons/webinar.png", MenuImageName = "OpenWtLibImage"});
#if DEBUG
            lbMainMenuItems.Add(new MainMenuViewModel {MenuText = "backup & restore", MenuImageName = "miBupReStr", IconUri = "/icons/cloud.png", MenuItemName = "CloudBackupImg"});
#else
            if((new LicenseInformation()).IsTrial())
                lbMainMenuItems.Add(new MainMenuViewModel {MenuText = "buy cloud backup", MenuItemName = "miBuyCloud", IconUri = "/icons/cloud.png", MenuImageName = "BuyCloudImg"});
            else 
                lbMainMenuItems.Add(new MainMenuViewModel {MenuText = "backup & restore", MenuImageName = "miBupReStr", IconUri = "/icons/cloud.png", MenuItemName = "CloudBackupImg"});
#endif
            lbMainMenuItems.Add(new MainMenuViewModel {MenuText = "settings", MenuItemName = "abmiManuallyEnter", MenuImageName = "SettingsImage", IconUri = "/icons/tasks.png"});
            IsMainMenuLoaded = true;
        }


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

        /// <summary>
        /// Loads the time report.
        /// </summary>
        /// <param name="entries">The entries.</param>
        public void LoadTimeReport(TimeData[] entries)
        {
            if (IsTimeReportDataLoaded) {
                IsTimeReportDataLoaded = false;
                icReport.Clear();
                lbTimeEntries.Clear();
            }
            
            int minutes = 0;
            if (entries.Length <= 0) return;

            int month = entries[0].Date.Month;
            var summary = new TimeReportSummaryViewModel();
            foreach (TimeData td in entries) {
                if (month != td.Date.Month) {
                    summary.Time = string.Format("{0:0.00} Hour(s)", (minutes/60.0));
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

                lbTimeEntries.Add(new TimeReportEntryViewModel {
                                                                   Date = td.Date.ToLongDateString(),
                                                                   Hours = string.Format("{0:0.00} Hour(s)", (td.Minutes/60.0)),
                                                                   ItemId = td.ItemId,
                                                                   Minutes = td.Minutes
                                                               });
            }
            summary.Time = string.Format("{0:0.00} Hour(s)", (minutes/60.0));
            summary.Minutes = minutes;
            icReport.Add(summary);
            IsTimeReportDataLoaded = true;
        }

        /// <summary>
        /// Loads the return visit full list.
        /// </summary>
        public void LoadReturnVisitFullList()
        {
            if (IsRvFullListLoaded) {
                llReturnVisitFullListCategory = new ObservableCollection<ReturnVistLLCategory>();
            }
            IsRvFullListLoaded = false;

            var wb = new WriteableBitmap(100, 100);
            for (int i = 0; i < wb.Pixels.Length; i++) {
                wb.Pixels[i] = 0xFF3300;
            }
            var bmp = new BitmapImage();
            using (var ms = new MemoryStream()) {
                wb.SaveJpeg(ms, 100, 100, 0, 100);
                bmp.SetSource(ms);
            }

            ReturnVisitData[] rVs = ReturnVisitsInterface.GetReturnVisits(SortOrder.CityAToZ, -1);
            if (rVs == null) return;
            if (rVs.Length <= 0) return;
            string lastCity = rVs[0].City;
            var cityCat = new ReturnVistLLCategory {Name = lastCity};
            foreach (ReturnVisitData r in rVs) {
                if (r.City != lastCity) {
                    llReturnVisitFullListCategory.Add(cityCat);
                    cityCat = new ReturnVistLLCategory {Name = r.City};
                }

                var bi = new BitmapImage();
                if (r.ImageSrc != null && r.ImageSrc.Length >= 0) {
                    var ris = new WriteableBitmap(450, 250);

                    //get image from database
                    for (int i = 0; i < r.ImageSrc.Length; i++) {
                        ris.Pixels[i] = r.ImageSrc[i];
                    }

                    //put the image in a WritableBitmap
                    using (var ms = new MemoryStream()) {
                        ris.SaveJpeg(ms, 450, 250, 0, 100);
                        bi.SetSource(ms);
                    }

                    //crop the image to 100x100 and centered
                    var img = new Image {
                                            Source = bi,
                                            Width = 450,
                                            Height = 250
                                        };
                    var wb2 = new WriteableBitmap(100, 100);
                    var t = new CompositeTransform {
                                                       ScaleX = 0.5,
                                                       ScaleY = 0.5,
                                                       TranslateX = -((450/2)/2 - 50),
                                                       TranslateY = -((250/2)/2 - 50)
                                                   };
                    wb2.Render(img, t);
                    wb2.Invalidate();
                    bi = new BitmapImage();
                    using (var ms = new MemoryStream()) {
                        wb2.SaveJpeg(ms, 100, 100, 0, 100);
                        bi.SetSource(ms);
                    }
                    //BitmapImage bi is now cropped
                } else {
                    bi = bmp; //Default image.
                }

                lastCity = r.City;
                cityCat.Items.Add(new ReturnVisitLLItemModel {
                                                                 Text = string.IsNullOrEmpty(r.FullName) ? string.Format("{0} year old {1}", r.Age, r.Gender) : r.FullName,
                                                                 Address1 = string.Format("{0} {1}", r.AddressOne, r.AddressTwo),
                                                                 Address2 = string.Format("{0}, {1} {2}", r.City, r.StateProvince, r.Country),
                                                                 ImageSource = bi,
                                                                 ItemId = r.ItemId
                                                             });
            }
            llReturnVisitFullListCategory.Add(cityCat);
            IsRvFullListLoaded = true;
        }
    }
}