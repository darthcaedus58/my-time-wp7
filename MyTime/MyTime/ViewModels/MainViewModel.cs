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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using FieldService.Model;
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

		/// <summary>
		/// Initializes a new instance of the <see cref="MainViewModel" /> class.
		/// </summary>
		public MainViewModel()
		{
			lbRvItems = new ObservableCollection<ReturnVisitViewModel>();
			lbMainMenuItems = new ObservableCollection<MainMenuModel>();
			icReport = new ObservableCollection<TimeReportSummaryModel>();
			lbTimeEntries = new ObservableCollection<TimeReportEntryViewModel>();

			if (!IsolatedStorageFile.GetUserStoreForApplication().FileExists("mainpage.xml")) {
				using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication()) {
					using (var file = new IsolatedStorageFileStream("mainpage.xml", FileMode.CreateNew, iso)) {
						byte[] b = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"utf-8\" ?><items><magazines>0</magazines><brochures>0</brochures><books>0</books><rvs>0</rvs><bs>0</bs><notes> </notes></items>");
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

		public ReturnVisitViewModel ReturnVisitData
		{
			get { return  _returnVisitData ?? (_returnVisitData = new ReturnVisitViewModel()); } 
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

		public double MainPageMagazines { get { return GetMainPageDouble("magazines"); } set { SetMainPageValue(value, "magazines"); } }

		public double MainPageBrochures { get { return GetMainPageDouble("brochures"); } set { SetMainPageValue(value, "brochures"); } }

		public double MainPageBooks { get { return GetMainPageDouble("books"); } set { SetMainPageValue(value, "books"); } }

		public double MainPageReturnVisits { get { return GetMainPageDouble("rvs"); } set { SetMainPageValue(value, "rvs"); } }

		public double MainPageBibleStudies { get { return GetMainPageDouble("bs"); } set { SetMainPageValue(value, "bs"); } }

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
						return xDoc.Element("items").Element(elementName).Value;
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
						return Convert.ToDouble(xDoc.Element("items").Element(elementName).Value);
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
						xDoc.Element("items").Element(elementName).Value = value;
					}
				}
				using (var file = new IsolatedStorageFileStream("mainpage.xml", FileMode.Create, iso)) {
					byte[] bb = Encoding.UTF8.GetBytes(xDoc.ToString());
					file.Write(bb, 0, bb.Length);
				}
			}
		}

		private static void SetMainPageValue(double value, string elementName)
		{
			using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication()) {
				XDocument xDoc;
				using (var file = new IsolatedStorageFileStream("mainpage.xml", FileMode.Open, iso)) {
					var bb = new byte[file.Length];
					file.Read(bb, 0, bb.Length);
					using (var oStream = new MemoryStream(bb)) {
						xDoc = XDocument.Load(oStream);
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
		/// <param name="so">The so.</param>
		public void LoadReturnVisitList(SortOrder so)
		{
			if (IsRvDataLoaded) {
				lbRvItems.Clear();
				IsRvDataLoaded = false;
			}

			var rvs = ReturnVisitsInterface.GetReturnVisits(SortOrder.DateOldestToNewest, -1);
			var sorted = new List<ReturnVisitViewModel>();
			foreach (var rv in rvs) {
				sorted.Add(new ReturnVisitViewModel() {ItemId = rv.ItemId});
			}
			foreach(var rv in sorted.OrderByDescending(d => d.DaysSinceLastVisit).Take(8)) {
				lbRvItems.Add(rv);
			}
			IsRvDataLoaded = true;


			#region Old Code

			//if (IsRvDataLoaded) lbRvItems.Clear();
			//IsRvDataLoaded = false;
			//// Sample data; replace with real data
			//ReturnVisitData[] rvs = ReturnVisitsInterface.GetReturnVisits(so);

			//var wb = new WriteableBitmap(100, 100);
			//for (int i = 0; i < wb.Pixels.Length; i++) {
			//	wb.Pixels[i] = 0xFF3300;
			//}
			//var bmp = new BitmapImage();
			//using (var ms = new MemoryStream()) {
			//	wb.SaveJpeg(ms, 100, 100, 0, 100);
			//	bmp.SetSource(ms);
			//}

			//foreach (ReturnVisitData r in rvs) {
			//	var bi = new BitmapImage();
			//	if (r.ImageSrc != null && r.ImageSrc.Length >= 0) {
			//		var ris = new WriteableBitmap(450, 250);

			//		//get image from database
			//		for (int i = 0; i < r.ImageSrc.Length; i++) {
			//			ris.Pixels[i] = r.ImageSrc[i];
			//		}

			//		//put the image in a WritableBitmap
			//		using (var ms = new MemoryStream()) {
			//			ris.SaveJpeg(ms, 450, 250, 0, 100);
			//			bi.SetSource(ms);
			//		}

			//		//crop the image to 100x100 and centered
			//		var img = new Image {
			//								Source = bi,
			//								Width = 450,
			//								Height = 250
			//							};
			//		var wb2 = new WriteableBitmap(100, 100);
			//		var t = new CompositeTransform {
			//										   ScaleX = 0.5,
			//										   ScaleY = 0.5,
			//										   TranslateX = -((450/2)/2 - 50),
			//										   TranslateY = -((250/2)/2 - 50)
			//									   };
			//		wb2.Render(img, t);
			//		wb2.Invalidate();
			//		bi = new BitmapImage();
			//		using (var ms = new MemoryStream()) {
			//			wb2.SaveJpeg(ms, 100, 100, 0, 100);
			//			bi.SetSource(ms);
			//		}
			//		//BitmapImage bi is now cropped
			//	} else {
			//		bi = bmp; //Default image.
			//	}

			//	string lv = r.LastVisitDate == DateTime.MinValue ? "No visit recorded" : string.Format("{0} day(s) since last visit", (DateTime.Now - r.LastVisitDate).Days);
			//	lbRvItems.Add(new ReturnVisitItemModel {
			//											   ItemId = r.ItemId,
			//											   ImageSource = bi,
			//											   Name = string.IsNullOrEmpty(r.FullName) ? string.Format("{0} year old {1}", r.Age, r.Gender) : r.FullName,
			//											   LineOne = string.Format("{0} {1}", r.AddressOne, r.AddressTwo),
			//											   LineTwo = string.Format("{0}, {1} {2}", r.City, r.StateProvince, r.PostalCode),
			//											   LineThree = lv
			//										   });
			//}
			//IsRvDataLoaded = true; 

			#endregion
		}

		/// <summary>
		/// Loads the main menu.
		/// </summary>
		public void LoadMainMenu()
		{
			if (IsMainMenuLoaded) lbMainMenuItems.Clear();
			IsMainMenuLoaded = false;
			lbMainMenuItems.Add(new MainMenuModel {
				                                      MenuText = "add time",
				                                      IconUri = "/icons/clock.png",
													  NavigateToPage = "/View/RegularTime.xaml"
			                                      });
			lbMainMenuItems.Add(new MainMenuModel {
				                                      MenuText = "add rbc time",
				                                      IconUri = "/icons/Tools.png",
													  NavigateToPage = "/View/RBCTime.xaml"
			                                      });

			lbMainMenuItems.Add(new MainMenuModel {
				                                      MenuText = "add return visit",
				                                      IconUri = "/icons/add-user.png",
													  NavigateToPage = "/View/EditReturnVisit.xaml"
			                                      });
			lbMainMenuItems.Add(new MainMenuModel {
				                                      MenuText = "send service report",
													  IconUri = "/icons/message.png",
													  NavigateToPage = ""
			                                      });
			lbMainMenuItems.Add(new MainMenuModel {
				                                      MenuText = string.Format("{0} report",
				                                                               DateTime.Today.ToString("MMMM").ToLower()),
													  IconUri = "/icons/Graph1.png",
													  NavigateToPage = ""
			                                      });
			lbMainMenuItems.Add(new MainMenuModel {
				                                      MenuText = "service year report",
													  IconUri = "/icons/Graph2.png",
													  NavigateToPage = ""
			                                      });
			//lbMainMenuItems.Add(new MainMenuViewModel {MenuText = "custom report", IconUri = "/icons/search.png"});

			lbMainMenuItems.Add(new MainMenuModel {
				                                      MenuText = "watchtower library",
													  IconUri = "/icons/books.png",
													  NavigateToPage = ""
			                                      });
#if DEBUG
			lbMainMenuItems.Add(new MainMenuViewModel {MenuText = "backup & restore", MenuImageName = "miBupReStr", IconUri = "/icons/Cloud-Refresh.png", MenuItemName = "CloudBackupImg"});
#else
			if ((new LicenseInformation()).IsTrial())
				lbMainMenuItems.Add(new MainMenuModel {
					                                      MenuText = "buy cloud backup",
					                                      IconUri = "/icons/Cloud-Refresh.png",
														  NavigateToPage = ""
				                                      });
			else
				lbMainMenuItems.Add(new MainMenuModel {
					                                      MenuText = "backup & restore",
					                                      IconUri = "/icons/Cloud-Refresh.png",
														  NavigateToPage = "/View/BackupAndRestorePage.xaml"
				                                      });
#endif
			lbMainMenuItems.Add(new MainMenuModel {
				                                      MenuText = "settings",
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
			int year = entries[0].Date.Year;
			var summary = new TimeReportSummaryModel();
			var rtdEntries = new List<RBCTimeData>();
			//ListBox<int> rvList = new ListBox<int>();
			foreach (TimeData td in entries) {
				if (month != td.Date.Month) {
					summary.Time = string.Format("{0:0.00} Hour(s)", (minutes/60.0));
					summary.Minutes = minutes;
					summary.RBCHours = (RBCTimeDataInterface.GetMonthRBCTimeTotal(new DateTime(year, month, 1)))/60.0;
					RBCTimeData[] eee = RBCTimeDataInterface.GetRBCTimeEntries(new DateTime(year, month, 1), new DateTime(year, month, 1).AddMonths(1).AddDays(-1), SortOrder.DateNewestToOldest);
					if (eee != null) rtdEntries.AddRange(eee);
					icReport.Add(summary);
					summary = new TimeReportSummaryModel();
					month = td.Date.Month;
					year = td.Date.Year;
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
					                                               Date = td.Date,
					                                               Hours = string.Format("{0:0.00} Hour(s)", (td.Minutes/60.0)),
					                                               ItemId = td.ItemId,
					                                               Minutes = td.Minutes,
					                                               EditLink = string.Format("/View/RegularTime.xaml?id={0}", td.ItemId)
				                                               });
			}
			RBCTimeData[] ee = RBCTimeDataInterface.GetRBCTimeEntries(new DateTime(year, month, 1), new DateTime(year, month, 1).AddMonths(1).AddDays(-1), SortOrder.DateNewestToOldest);
			if (ee != null) rtdEntries.AddRange(ee);
			summary.RBCHours = (RBCTimeDataInterface.GetMonthRBCTimeTotal(new DateTime(year, month, 1))/60.0);
			summary.Time = string.Format("{0:0.00} Hour(s)", (minutes/60.0));
			summary.Minutes = minutes;
			icReport.Add(summary);

			foreach (RBCTimeData e in rtdEntries)
				lbTimeEntries.Add(new TimeReportEntryViewModel {
					                                               Date = e.Date,
					                                               Hours = string.Format("{0:0.00} R/B/C Hour(s)", e.Hours),
					                                               ItemId = e.ItemID,
					                                               Minutes = e.Minutes,
					                                               EditLink = string.Format("/View/RBCTime.xaml?id={0}", e.ItemID)
				                                               });
			TimeReportEntryViewModel[] lte = lbTimeEntries.OrderBy(s => s.Date.Date).ToArray();
			lbTimeEntries.Clear();
			foreach (TimeReportEntryViewModel l in lte) lbTimeEntries.Add(l);

			IsTimeReportDataLoaded = true;
		}
	}
}