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

namespace MyTime
{
	public partial class MainPage : PhoneApplicationPage
	{
		private DateTime _timerBase;
		private TimeSpan _timer;
		private DispatcherTimer _dt;

		[System.ComponentModel.EditorBrowsable]
		public Object Icon { get; set; }
		private enum TimerState
		{
			Stopped,
			Paused,
			Running
		};

		private TimerState _timerState = TimerState.Stopped;


		// Constructor
		public MainPage()
		{
			InitializeComponent();

			// Set the data context of the listbox control to the sample data
			DataContext = App.ViewModel;
			this.Loaded += new RoutedEventHandler(MainPage_Loaded);


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

		public void MenuItem_ClickEvent(object sender, RoutedEventArgs e) { }

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
	}
}