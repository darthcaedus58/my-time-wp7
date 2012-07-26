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
			lbRvItems.Add(new ItemViewModel() { Name = "runtime one", LineOne = "Maecenas praesent accumsan bibendum", LineTwo = "Facilisi faucibus habitant inceptos interdum lobortis nascetur pharetra placerat pulvinar sagittis senectus sociosqu" });
			lbRvItems.Add(new ItemViewModel() { Name = "runtime two", LineOne = "Dictumst eleifend facilisi faucibus", LineTwo = "Suscipit torquent ultrices vehicula volutpat maecenas praesent accumsan bibendum dictumst eleifend facilisi faucibus" });
			lbRvItems.Add(new ItemViewModel() { Name = "runtime three", LineOne = "Habitant inceptos interdum lobortis", LineTwo = "Habitant inceptos interdum lobortis nascetur pharetra placerat pulvinar sagittis senectus sociosqu suscipit torquent" });
			lbRvItems.Add(new ItemViewModel() { Name = "runtime four", LineOne = "Nascetur pharetra placerat pulvinar", LineTwo = "Ultrices vehicula volutpat maecenas praesent accumsan bibendum dictumst eleifend facilisi faucibus habitant inceptos" });
			lbRvItems.Add(new ItemViewModel() { Name = "runtime five", LineOne = "Maecenas praesent accumsan bibendum", LineTwo = "Maecenas praesent accumsan bibendum dictumst eleifend facilisi faucibus habitant inceptos interdum lobortis nascetur" });
			lbRvItems.Add(new ItemViewModel() { Name = "runtime six", LineOne = "Dictumst eleifend facilisi faucibus", LineTwo = "Pharetra placerat pulvinar sagittis senectus sociosqu suscipit torquent ultrices vehicula volutpat maecenas praesent" });
			lbRvItems.Add(new ItemViewModel() { Name = "runtime seven", LineOne = "Habitant inceptos interdum lobortis", LineTwo = "Accumsan bibendum dictumst eleifend facilisi faucibus habitant inceptos interdum lobortis nascetur pharetra placerat" });
			lbRvItems.Add(new ItemViewModel() { Name = "runtime eight", LineOne = "Nascetur pharetra placerat pulvinar", LineTwo = "Pulvinar sagittis senectus sociosqu suscipit torquent ultrices vehicula volutpat maecenas praesent accumsan bibendum" });
			lbRvItems.Add(new ItemViewModel() { Name = "runtime nine", LineOne = "Maecenas praesent accumsan bibendum", LineTwo = "Facilisi faucibus habitant inceptos interdum lobortis nascetur pharetra placerat pulvinar sagittis senectus sociosqu" });
			lbRvItems.Add(new ItemViewModel() { Name = "runtime ten", LineOne = "Dictumst eleifend facilisi faucibus", LineTwo = "Suscipit torquent ultrices vehicula volutpat maecenas praesent accumsan bibendum dictumst eleifend facilisi faucibus" });
			lbRvItems.Add(new ItemViewModel() { Name = "runtime eleven", LineOne = "Habitant inceptos interdum lobortis", LineTwo = "Habitant inceptos interdum lobortis nascetur pharetra placerat pulvinar sagittis senectus sociosqu suscipit torquent" });
			lbRvItems.Add(new ItemViewModel() { Name = "runtime twelve", LineOne = "Nascetur pharetra placerat pulvinar", LineTwo = "Ultrices vehicula volutpat maecenas praesent accumsan bibendum dictumst eleifend facilisi faucibus habitant inceptos" });
			lbRvItems.Add(new ItemViewModel() { Name = "runtime thirteen", LineOne = "Maecenas praesent accumsan bibendum", LineTwo = "Maecenas praesent accumsan bibendum dictumst eleifend facilisi faucibus habitant inceptos interdum lobortis nascetur" });
			lbRvItems.Add(new ItemViewModel() { Name = "runtime fourteen", LineOne = "Dictumst eleifend facilisi faucibus", LineTwo = "Pharetra placerat pulvinar sagittis senectus sociosqu suscipit torquent ultrices vehicula volutpat maecenas praesent" });
			lbRvItems.Add(new ItemViewModel() { Name = "runtime fifteen", LineOne = "Habitant inceptos interdum lobortis", LineTwo = "Accumsan bibendum dictumst eleifend facilisi faucibus habitant inceptos interdum lobortis nascetur pharetra placerat" });
			lbRvItems.Add(new ItemViewModel() { Name = "runtime sixteen", LineOne = "Nascetur pharetra placerat pulvinar", LineTwo = "Pulvinar sagittis senectus sociosqu suscipit torquent ultrices vehicula volutpat maecenas praesent accumsan bibendum" });

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