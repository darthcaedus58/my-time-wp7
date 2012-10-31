﻿using System;
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


namespace MyTime
{
	public class MainViewModel : INotifyPropertyChanged
	{
		public MainViewModel()
		{
			this.lbRvItems = new ObservableCollection<ItemViewModel>();
			lbMainMenuItems = new ObservableCollection<MainMenuViewModel>();
		}

		/// <summary>
		/// A collection for ItemViewModel objects.
		/// </summary>
		public ObservableCollection<ItemViewModel> lbRvItems { get; private set; }
		public ObservableCollection<MainMenuViewModel> lbMainMenuItems { get; private set; }

		public bool IsDataLoaded
		{
			get;
			private set;
		}



		/// <summary>
		/// Creates and adds a few ItemViewModel objects into the lbRvItems collection.
		/// </summary>
		public void LoadData()
		{
			// Sample data; replace with real data
			var rvs = ReturnVisitsInterface.GetReturnVisits(SortOrder.DateFirstToLast, 1);

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
				lbRvItems.Add(new ItemViewModel() { ItemId = r.ItemId, ImageSource = bi, Name = r.FullName, LineOne = string.Format("{0} {1}", r.AddressOne, r.AddressTwo), LineTwo = string.Format("{0}, {1} {2}", r.City, r.StateProvince, r.PostalCode) });
			}
			this.IsDataLoaded = true;
		}

		public void LoadMainMenu()
		{
			lbMainMenuItems.Add(new MainMenuViewModel() { MenuText = "Add Time", MenuItemName = "miAddTime", IconUri = "/icons/add.png", MenuImageName = "AddTimeImage" });
			lbMainMenuItems.Add(new MainMenuViewModel() { MenuText = "Add Return Visit", MenuItemName = "miAddRv", IconUri = "/icons/phone.png", MenuImageName = "AddReturnVisitImage" });
			lbMainMenuItems.Add(new MainMenuViewModel() { MenuText = "Find Nearest RV", MenuItemName = "miFindNearRv", IconUri = "/icons/search.png", MenuImageName = "FindNearestRv" });
			lbMainMenuItems.Add(new MainMenuViewModel() { MenuText = "Send Service Report", MenuItemName = "miSendReport", IconUri = "/icons/mail.png", MenuImageName = "SemdReportImage" });
			lbMainMenuItems.Add(new MainMenuViewModel() { MenuText = string.Format("{0} Report",DateTime.Today.ToString("MMMM")), MenuItemName = "miThisMonthReport", IconUri = "/icons/favs.png", MenuImageName = "ThisMonthImage" });
			lbMainMenuItems.Add(new MainMenuViewModel() { MenuText = string.Format("{0} Report",DateTime.Today.Year), MenuItemName = "miThisYearReport", IconUri = "/icons/favs.png", MenuImageName = "ThisYearImage" });
			lbMainMenuItems.Add(new MainMenuViewModel() { MenuText = "Custom Report", MenuItemName = "miCustomReport", IconUri = "/icons/search.png", MenuImageName = "CustomReportImage" });
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged(String propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (null != handler) {
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}