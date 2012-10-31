using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using System.IO.IsolatedStorage;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.ComponentModel;
using System.Collections.ObjectModel;
using MyTimeDatabaseLib;

namespace MyTime
{
	public partial class MainPage : PhoneApplicationPage
	{
		private DateTime _timerBase;
		private TimeSpan _timer;
		private DispatcherTimer _dt;
		[System.Xml.Serialization.XmlElement(ElementName = "startTime", DataType = "DateTime")]
		public System.DateTime startTime { get; set; }


		[System.ComponentModel.EditorBrowsable]
		public Object Icon { get; set; }
		private enum TimerState:byte
		{
			Stopped = 3,
			Paused = 0,
			Running = 1
		};

		private TimerState _timerState = TimerState.Stopped;


		// Constructor
		public MainPage()
		{
			InitializeComponent();

			// Set the data context of the listbox control to the sample data
			DataContext = App.ViewModel;
			this.Loaded += new RoutedEventHandler(MainPage_Loaded);
			if (IsolatedStorageFile.GetUserStoreForApplication().FileExists("restart.bin")) GetRestartTime();
		}

		private void GetRestartTime()
		{
			try {
				using (var restartTime = IsolatedStorageFile.GetUserStoreForApplication().OpenFile("restart.bin", System.IO.FileMode.Open)) {
					var isRunning = (TimerState)restartTime.ReadByte();
					
					_dt = new DispatcherTimer()
					{
						Interval = new TimeSpan(0, 0, 0, 1, 0)
					};
					_dt.Tick += new EventHandler(dt_Tick);
					if (isRunning == TimerState.Running) {
						byte[] time = new byte[sizeof(long)];
						restartTime.Read(time, 0, sizeof(long));
						_timerBase = new DateTime(BitConverter.ToInt64(time, 0));
						_timer = DateTime.Now.Subtract(_timerBase);
						_timerState = TimerState.Running;
						_dt.Start();
					} else if (isRunning == TimerState.Paused) {
						int hours, minutes, seconds;
						byte[] ints = new byte[sizeof(int)];
						restartTime.Read(ints, 0, sizeof(int));
						hours = BitConverter.ToInt32(ints, 0);
						restartTime.Read(ints, 0, sizeof(int));
						minutes = BitConverter.ToInt32(ints, 0);
						restartTime.Read(ints, 0, sizeof(int));
						seconds = BitConverter.ToInt32(ints, 0);
						_timer = new TimeSpan(hours, minutes, seconds);
						_timerBase = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour - _timer.Hours, DateTime.Now.Minute - _timer.Minutes, DateTime.Now.Second - _timer.Seconds);
						_timerState = TimerState.Paused;
					} else {
						_timer = new TimeSpan(0, 0, 0);
					}
					lblTimer.Text = string.Format("{0:0,0}:{1:0,0}:{2:0,0}", _timer.Hours, _timer.Minutes, _timer.Seconds);
				}
			} catch {
				IsolatedStorageFile.GetUserStoreForApplication().DeleteFile("restart.bin");
			}

		}

		private void SetRestartTime()
		{
			IsolatedStorageFile.GetUserStoreForApplication().DeleteFile("restart.bin");
			try {
				using (var restart = IsolatedStorageFile.GetUserStoreForApplication().CreateFile("restart.bin")) {
					if (_timerState == TimerState.Running) {
						restart.WriteByte((byte)TimerState.Running);
						byte[] time = BitConverter.GetBytes(_timerBase.Ticks);
						restart.Write(time, 0, sizeof(long));
					} else if (_timerState == TimerState.Paused) {
						restart.WriteByte((byte)TimerState.Paused);
						byte[] hours = BitConverter.GetBytes(_timer.Hours);
						byte[] minutes = BitConverter.GetBytes(_timer.Minutes);
						byte[] seconds = BitConverter.GetBytes(_timer.Seconds);
						restart.Write(hours, 0, sizeof(int));
						restart.Write(minutes, 0, sizeof(int));
						restart.Write(seconds, 0, sizeof(int));
					} else {
						restart.WriteByte((byte)TimerState.Stopped);
					}
				}
			} catch { 
				IsolatedStorageFile.GetUserStoreForApplication().DeleteFile("restart.bin");
			}
		}

		private void ClearRestartTime()
		{
			IsolatedStorageFile.GetUserStoreForApplication().DeleteFile("restart.bin");
		}

		// Load data for the ViewModel lbRvItems
		private void MainPage_Loaded(object sender, RoutedEventArgs e)
		{
			if (!App.ViewModel.IsDataLoaded) {
				App.ViewModel.LoadData();
				App.ViewModel.LoadMainMenu();
			}
		}

		private void abibStart_Click(object sender, EventArgs e) 
		{ 
			switch(_timerState) {
				case TimerState.Running:
					break;
				case TimerState.Paused:
					_timerBase = DateTime.Now.Subtract(_timer);
					_dt.Start();
					break;
				default:		// TimerState.Stopped
					_timerBase = DateTime.Now;
					_dt = new DispatcherTimer() {
					                            	Interval = new TimeSpan(0, 0, 0, 1, 0)
					                            };
					_dt.Tick += new EventHandler(dt_Tick);
					_dt.Start();
					_timer = new TimeSpan(0, 0, 1);
					break;
			}
			_timerState = TimerState.Running;
			SetRestartTime();
			lblTimer.Text = string.Format("{0:0,0}:{1:0,0}:{2:0,0}", _timer.Hours, _timer.Minutes, _timer.Seconds);
		}

		private void abibPause_Click(object sender, EventArgs e)
		{
			switch(_timerState) {
				case TimerState.Paused:
				case TimerState.Stopped:
					abibStart_Click(sender, e);
					break;
				default: // TimerState.Running
					_dt.Stop();
					_timerState = TimerState.Paused;
					SetRestartTime();
					break;
			}
		}

		private void abibStop_Click(object sender, EventArgs e) 
		{ 
			_dt.Stop();
			_timer = new TimeSpan();
			_timerBase = new DateTime();
			lblTimer.Text = "00:00:00";
			_timerState = TimerState.Stopped;
			ClearRestartTime();
		}

		private void dt_Tick(object sender, EventArgs e)
		{
			if (_timerState != TimerState.Running) {
				_dt.Stop();
				return;
			}
			_timer = DateTime.Now.Subtract(_timerBase);

			lblTimer.Text = string.Format("{0:0,0}:{1:0,0}:{2:0,0}", _timer.Hours, _timer.Minutes, _timer.Seconds);
		}

		private void abmiManuallyEnter_Click(object sender, EventArgs e) { NavigationService.Navigate(new Uri("/ManuallyEnterTime.xaml", UriKind.Relative)); }

		public void MenuItem_ClickEvent(object sender, RoutedEventArgs e) 
		{ 
			switch(((MenuItem)sender).Header.ToString()) {
				case "Add Time":
					NavigationService.Navigate(new Uri("/ManuallyEnterTime.xaml", UriKind.Relative));
					break;
				case "Add Return Visit":
					NavigationService.Navigate(new Uri("/AddNewRV.xaml", UriKind.Relative));
					break;
				default:
					break;
			}
		}

		public void MenuImage_TapEvent(object sender, System.Windows.Input.GestureEventArgs e) { }

		private void imgAddReturnVisit_Tap(object sender, System.Windows.Input.GestureEventArgs e) { NavigationService.Navigate(new Uri("/AddNewRV.xaml", UriKind.Relative)); }

		private void TextBox_Tap(object sender, System.Windows.Input.GestureEventArgs e)
		{
			if(tbSearchRvs.Text.Equals("Search...", StringComparison.InvariantCultureIgnoreCase))
			{
				tbSearchRvs.Text = string.Empty;
			}
		}

		private void tbSearchRvs_LostFocus(object sender, RoutedEventArgs e)
		{
			if(string.IsNullOrEmpty(tbSearchRvs.Text))
			{
				tbSearchRvs.Text = "Search...";
			}
		}

		private void StackPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
		{

		}

		private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (((ListBox)sender).SelectedIndex < 0) return;
			try {
				var rv = (ItemViewModel)((ListBox)sender).SelectedItem;
				((ListBox)sender).SelectedIndex = -1;
				NavigationService.Navigate(new Uri(string.Format("/AddNewRV.xaml?id={0}", rv.ItemId.ToString()), UriKind.Relative));
			} catch { }
		}
	}
}