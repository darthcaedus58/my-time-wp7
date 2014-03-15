// ***********************************************************************
// Assembly         : MyTime
// Author           : trevo_000
// Created          : 11-03-2012
//
// Last Modified By : tevo_000
// Last Modified On : 11-11-2012
// ***********************************************************************
// <copyright file="MainViewModel.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using FieldService.Model;
using FieldService.View;
using Microsoft.Phone.Marketplace;
using MyTimeDatabaseLib;

namespace FieldService.ViewModels
{
        /// <summary>
        /// Class MainViewModel
        /// </summary>
        public class MainViewModel : INotifyPropertyChanged
        {
                private ReturnVisitViewModel _returnVisitData;
                private List<ReturnVisitViewModel> _rvs;

                /// <summary>
                /// Initializes a new instance of the <see cref="MainViewModel" /> class.
                /// </summary>
                public MainViewModel()
                {
                        IsRvDataChanged = true;
                        lbRvItems = new ObservableCollection<ReturnVisitViewModel>();
                        lbMainMenuItems = new ObservableCollection<MainMenuModel>();
                        if (!IsolatedStorageFile.GetUserStoreForApplication().FileExists("mainpage.xml")) {
                                using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication()) {
                                        using (var file = new IsolatedStorageFileStream("mainpage.xml", FileMode.CreateNew, iso)) {
                                                byte[] b = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"utf-8\" ?><items><magazines>0</magazines><brochures>0</brochures><books>0</books><rvs>0</rvs><bs>0</bs><tracts>0</tracts><notes> </notes></items>");
                                                file.Write(b, 0, b.Length);
                                        }
                                }
                        }
                }

                /// <summary>
                /// A collection for ReturnVisitItemViewModel objects.
                /// </summary>
                /// <value>The lb rv items.</value>
                public ObservableCollection<ReturnVisitViewModel> lbRvItems { get; private set; }

                /// <summary>
                /// Gets the lb main menu items.
                /// </summary>
                /// <value>The lb main menu items.</value>
                public ObservableCollection<MainMenuModel> lbMainMenuItems { get; private set; }

                public ReturnVisitViewModel ReturnVisitData
                {
                        get { return _returnVisitData ?? (_returnVisitData = new ReturnVisitViewModel()); }
                        set
                        {
                                if (_returnVisitData == value) return;
                                _returnVisitData = value;
                                NotifyPropertyChanged("ReturnVisitData");
                        }
                }

                /// <summary>
                /// Gets a value indicating whether this instance is rv data loaded.
                /// </summary>
                /// <value><c>true</c> if this instance is rv data loaded; otherwise, <c>false</c>.</value>
                private bool _isRvDataChanged = true;
                public bool IsRvDataChanged
                {
                        get { return _isRvDataChanged; }
                        set
                        {
                                if (_isRvDataChanged != value) {
                                        _isRvDataChanged = value;
                                        NotifyPropertyChanged("IsRvDataChanged");
                                }
                        }
                }

                /// <summary>
                /// Gets a value indicating whether this instance is main menu loaded.
                /// </summary>
                /// <value><c>true</c> if this instance is main menu loaded; otherwise, <c>false</c>.</value>
                public bool IsMainMenuLoaded { get; private set; }

                public double MainPageMagazines { get { return GetMainPageDouble("magazines"); } set { SetMainPageDouble(value, "magazines"); } }

                public double MainPageBrochures { get { return GetMainPageDouble("brochures"); } set { SetMainPageDouble(value, "brochures"); } }

                public double MainPageBooks { get { return GetMainPageDouble("books"); } set { SetMainPageDouble(value, "books"); } }

                public double MainPageTracts { get { return GetMainPageDouble("tracts"); } set { SetMainPageDouble(value, "tracts"); } }

                public double MainPageReturnVisits { get { return GetMainPageDouble("rvs"); } set { SetMainPageDouble(value, "rvs"); } }

                public double MainPageBibleStudies { get { return GetMainPageDouble("bs"); } set { SetMainPageDouble(value, "bs"); } }

                public string MainPageNotes { get { return GetMainPageString("notes"); } set { SetMainPageString(value, "notes"); } }

                #region INotifyPropertyChanged Members

                /// <summary>
                /// Occurs when [property changed].
                /// </summary>
                public event PropertyChangedEventHandler PropertyChanged;

                #endregion

                private static string GetMainPageString(string elementName)
                {
                        using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication()) {
                                using (var file = new IsolatedStorageFileStream("mainpage.xml", FileMode.Open, iso)) {
                                        var bb = new byte[file.Length];
                                        file.Read(bb, 0, bb.Length);
                                        using (var oStream = new MemoryStream(bb)) {
                                                XDocument xDoc = XDocument.Load(oStream);
                                                try {
                                                        return xDoc.Element("items").Element(elementName).Value;
                                                } catch (NullReferenceException) {
                                                        return string.Empty;
                                                }
                                        }
                                }
                        }
                }

                private static double GetMainPageDouble(string elementName)
                {
                        using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication()) {
                                using (var file = new IsolatedStorageFileStream("mainpage.xml", FileMode.Open, iso)) {
                                        var bb = new byte[file.Length];
                                        file.Read(bb, 0, bb.Length);
                                        using (var oStream = new MemoryStream(bb)) {
                                                XDocument xDoc = XDocument.Load(oStream);
                                                try {
                                                        return Convert.ToDouble(xDoc.Element("items").Element(elementName).Value);
                                                } catch (NullReferenceException) {
                                                        return 0;
                                                }
                                        }
                                }
                        }
                }

                private static void SetMainPageString(string value, string elementName)
                {
                        using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication()) {
                                XDocument xDoc;
                                using (var file = new IsolatedStorageFileStream("mainpage.xml", FileMode.Open, iso)) {
                                        var bb = new byte[file.Length];
                                        file.Read(bb, 0, bb.Length);
                                        using (var oStream = new MemoryStream(bb)) {
                                                xDoc = XDocument.Load(oStream);
                                                if (!xDoc.Element("items").Elements(elementName).Any())
                                                        xDoc.Element("items").Add(new XElement(elementName, value));
                                                xDoc.Element("items").Element(elementName).Value = value;
                                        }
                                }
                                using (var file = new IsolatedStorageFileStream("mainpage.xml", FileMode.Create, iso)) {
                                        byte[] bb = Encoding.UTF8.GetBytes(xDoc.ToString());
                                        file.Write(bb, 0, bb.Length);
                                }
                        }
                }

                private static void SetMainPageDouble(double value, string elementName)
                {
                        using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication()) {
                                XDocument xDoc;
                                using (var file = new IsolatedStorageFileStream("mainpage.xml", FileMode.Open, iso)) {
                                        var bb = new byte[file.Length];
                                        file.Read(bb, 0, bb.Length);
                                        using (var oStream = new MemoryStream(bb)) {
                                                xDoc = XDocument.Load(oStream);
                                                if (!xDoc.Element("items").Elements(elementName).Any())
                                                        xDoc.Element("items").Add(new XElement(elementName, value));
                                                xDoc.Element("items").Element(elementName).Value = value.ToString();
                                        }
                                }
                                using (var file = new IsolatedStorageFileStream("mainpage.xml", FileMode.Create, iso)) {
                                        byte[] bb = Encoding.UTF8.GetBytes(xDoc.ToString());
                                        file.Write(bb, 0, bb.Length);
                                }
                        }
                }

                /// <summary>
                /// Creates and adds a few ReturnVisitItemViewModel objects into the lbRvItems collection.
                /// </summary>
                /// <param name="so">The sort order.</param>
                public void LoadReturnVisitList(SortOrder so)
                {
                        if (IsRvDataChanged) {
                                lbRvItems.Clear();
                        } else {
                                return;
                        }

                        var bw = new BackgroundWorker();
                        bw.DoWork += (obt, e) => {
                                _rvs = new List<ReturnVisitViewModel>();
                                var rvList = ReturnVisitsInterface.GetReturnVisitsByLastVisitDate(8);
                                foreach (var r in rvList) {
                                        //if (!ReturnVisitsInterface.IdExists(r)) continue;
                                        //if (_rvs.Count >= 8) break;
                                        _rvs.Add(new ReturnVisitViewModel() { ItemId = r });
                                }

                        };
                        bw.RunWorkerCompleted += (obj, e) => {
                                foreach (var rv in _rvs) {
                                        lbRvItems.Add(rv);
                                }
                                IsRvDataChanged = false;
                        };
                        bw.RunWorkerAsync();
                }


                /// <summary>
                /// Loads the main menu.
                /// </summary>
                public void LoadMainMenu()
                {
                        if (IsMainMenuLoaded) lbMainMenuItems.Clear();
                        IsMainMenuLoaded = false;
                        lbMainMenuItems.Add(new MainMenuModel
                        {
                                MenuText = StringResources.MainPage_MainMenu_AddTime,
                                IconUri = "/icons/clock.png",
                                NavigateToPage = "/View/RegularTime.xaml"
                        });
                        lbMainMenuItems.Add(new MainMenuModel
                        {
                                MenuText = StringResources.MainPage_MainMenu_AddAuxTime,
                                IconUri = "/icons/Tools.png",
                                NavigateToPage = "/View/RBCTime.xaml"
                        });

                        lbMainMenuItems.Add(new MainMenuModel
                        {
                                MenuText = StringResources.MainPage_MainMenu_AddRv,
                                IconUri = "/icons/add-user.png",
                                NavigateToPage = "/View/EditReturnVisit.xaml"
                        });
                        lbMainMenuItems.Add(new MainMenuModel
                        {
                                MenuText = StringResources.MainPage_MainMenu_SendReport,
                                IconUri = "/icons/message.png",
                                NavigateToPage = ""
                        });
                        lbMainMenuItems.Add(new MainMenuModel
                        {
                                MenuText = string.Format(StringResources.MainPage_MainMenu_MonthReport,
                                                         DateTime.Today.ToString("MMMM").ToLower()),
                                IconUri = "/icons/Graph1.png",
                                NavigateToPage = ""
                        });
                        lbMainMenuItems.Add(new MainMenuModel
                        {
                                MenuText = StringResources.MainPage_MainMenu_ServiceYearReport,
                                IconUri = "/icons/Graph2.png",
                                NavigateToPage = ""
                        });
                        lbMainMenuItems.Add(new MainMenuModel
                        {
                                MenuText = StringResources.MainPage_MainMenu_CustomReport,
                                IconUri = "/icons/ChartCustomization.png",
                                NavigateToPage = "/View/CustomReportsPage.xaml"
                        });

                        //lbMainMenuItems.Add(
                        //        new MainMenuModel
                        //        {
                        //                MenuText = StringResources.MainPage_MainMenu_HouseToHouseRecords,
                        //                IconUri = "/icons/Houses.png",
                        //                NavigateToPage = "/View/TerritoryList.xaml"
                        //        });

                        lbMainMenuItems.Add(new MainMenuModel
                        {
                                MenuText = StringResources.MainPage_MainMenu_WtLib,
                                IconUri = "/icons/books.png",
                                NavigateToPage = ""
                        });
#if DEBUG
			lbMainMenuItems.Add(new MainMenuViewModel {MenuText = "backup & restore", MenuImageName = "miBupReStr", IconUri = "/icons/Cloud-Refresh.png", MenuItemName = "CloudBackupImg"});
#else
                        if ((new LicenseInformation()).IsTrial())
                                lbMainMenuItems.Add(new MainMenuModel
                                {
                                        MenuText = "buy cloud backup",
                                        IconUri = "/icons/Cloud-Refresh.png",
                                        NavigateToPage = ""
                                });
                        else
                                lbMainMenuItems.Add(new MainMenuModel
                                {
                                        MenuText = StringResources.MainPage_MainMenu_BackupRestore,
                                        IconUri = "/icons/Cloud-Refresh.png",
                                        NavigateToPage = "/View/BackupAndRestorePage.xaml"
                                });
#endif
                        lbMainMenuItems.Add(new MainMenuModel
                        {
                                MenuText = StringResources.MainPage_MainMenu_Settings,
                                IconUri = "/icons/settings.png",
                                NavigateToPage = "/View/SettingsPage.xaml"
                        });
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
        }
}