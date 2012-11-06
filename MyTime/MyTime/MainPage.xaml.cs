using System;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.Xml.Serialization;
using Microsoft.Phone.Controls;
using MyTimeDatabaseLib;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace MyTime
{
    public partial class MainPage : PhoneApplicationPage
    {
        private DispatcherTimer _dt;
        private TimeSpan _timer;
        private DateTime _timerBase;

        private TimerState _timerState = TimerState.Stopped;


        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            Loaded += MainPage_Loaded;
            if (IsolatedStorageFile.GetUserStoreForApplication().FileExists("restart.bin")) GetRestartTime();
        }

        [XmlElement(ElementName = "startTime", DataType = "DateTime")]
        public DateTime startTime { get; set; }


        [EditorBrowsable]
        public Object Icon { get; set; }

        #region Events

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ListBox) sender).SelectedIndex < 0) return;
            try {
                var rv = (ReturnVisitItemViewModel) ((ListBox) sender).SelectedItem;
                ((ListBox) sender).SelectedIndex = -1;
                NavigationService.Navigate(new Uri(string.Format("/AddNewRV.xaml?id={0}", rv.ItemId.ToString()), UriKind.Relative));
            } catch {}
        }

        // Load data for the ViewModel lbRvItems
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.ViewModel.IsRvDataLoaded) {
                App.ViewModel.lbRvItems.Clear();
                App.ViewModel.lbMainMenuItems.Clear();
            }
            App.ViewModel.LoadReturnVisitList();
            App.ViewModel.LoadMainMenu();
        }

        public void MenuImage_TapEvent(object sender, GestureEventArgs e)
        {
            string v = ((Image) sender).Tag.ToString().ToLower();
            NavigateMainMenu(v);
        }

        public void MenuItem_ClickEvent(object sender, RoutedEventArgs e)
        {
            string v = ((MenuItem) sender).Header.ToString().ToLower();
            NavigateMainMenu(v);
        }

        private void TextBoxMasking_KeyDown(object sender, KeyEventArgs e)
        {
            Key[] GoodKeys = {
                                 Key.D0, Key.D1, Key.D2, Key.D3, Key.D4,
                                 Key.D5, Key.D6, Key.D7, Key.D8, Key.D9,
                                 Key.NumPad0, Key.NumPad1, Key.NumPad2, Key.NumPad3, Key.NumPad4,
                                 Key.NumPad5, Key.NumPad6, Key.NumPad7, Key.NumPad8, Key.NumPad9
                             };
            if (!GoodKeys.Contains(e.Key)) {
                e.Handled = true;
                return;
            }
        }

        private void TextBox_Tap(object sender, GestureEventArgs e)
        {
            if (tbSearchRvs.Text.Equals("Search...", StringComparison.InvariantCultureIgnoreCase)) {
                tbSearchRvs.Text = string.Empty;
            }
        }

        private void abibPause_Click(object sender, EventArgs e)
        {
            switch (_timerState) {
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

        private void abibStart_Click(object sender, EventArgs e)
        {
            switch (_timerState) {
                case TimerState.Running:
                    break;
                case TimerState.Paused:
                    _timerBase = DateTime.Now.Subtract(_timer);
                    _dt.Start();
                    break;
                default: // TimerState.Stopped
                    _timerBase = DateTime.Now;
                    _dt = new DispatcherTimer {
                                                  Interval = new TimeSpan(0, 0, 0, 1, 0)
                                              };
                    _dt.Tick += dt_Tick;
                    _dt.Start();
                    _timer = new TimeSpan(0, 0, 1);
                    break;
            }
            _timerState = TimerState.Running;
            SetRestartTime();
            lblTimer.Text = string.Format("{0:0,0}:{1:0,0}:{2:0,0}", _timer.Hours, _timer.Minutes, _timer.Seconds);
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

        private void abmiManuallyEnter_Click(object sender, EventArgs e) { NavigationService.Navigate(new Uri("/ManuallyEnterTime.xaml", UriKind.Relative)); }

        private void dt_Tick(object sender, EventArgs e)
        {
            if (_timerState != TimerState.Running) {
                _dt.Stop();
                return;
            }
            _timer = DateTime.Now.Subtract(_timerBase);

            lblTimer.Text = string.Format("{0:0,0}:{1:0,0}:{2:0,0}", _timer.Hours, _timer.Minutes, _timer.Seconds);
        }

        private void imgAddReturnVisit_Tap(object sender, GestureEventArgs e) { NavigationService.Navigate(new Uri("/AddNewRV.xaml", UriKind.Relative)); }

        public void ss_DailyTextRetrieved(DailyText dt)
        {
            lblDailyText.Text = dt.Scripture;
            lblDTSummary.Text = dt.SummaryText;
        }

        private void tbSearchRvs_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbSearchRvs.Text)) {
                tbSearchRvs.Text = "Search...";
            }
        }

        #endregion

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            lblDailyTextDate.Text = DateTime.Now.ToLongDateString();

            var ss = new DailyTextScraper();

            ss.DailyTextRetrieved += ss_DailyTextRetrieved;

            ss.StartDailyTextRetrieval(DateTime.Now);
        }

        private void GetRestartTime()
        {
            try {
                using (IsolatedStorageFileStream restartTime = IsolatedStorageFile.GetUserStoreForApplication().OpenFile("restart.bin", FileMode.Open)) {
                    var isRunning = (TimerState) restartTime.ReadByte();

                    _dt = new DispatcherTimer {
                                                  Interval = new TimeSpan(0, 0, 0, 1, 0)
                                              };
                    _dt.Tick += dt_Tick;
                    if (isRunning == TimerState.Running) {
                        var time = new byte[sizeof (long)];
                        restartTime.Read(time, 0, sizeof (long));
                        _timerBase = new DateTime(BitConverter.ToInt64(time, 0));
                        _timer = DateTime.Now.Subtract(_timerBase);
                        _timerState = TimerState.Running;
                        _dt.Start();
                    } else if (isRunning == TimerState.Paused) {
                        int hours, minutes, seconds;
                        var ints = new byte[sizeof (int)];
                        restartTime.Read(ints, 0, sizeof (int));
                        hours = BitConverter.ToInt32(ints, 0);
                        restartTime.Read(ints, 0, sizeof (int));
                        minutes = BitConverter.ToInt32(ints, 0);
                        restartTime.Read(ints, 0, sizeof (int));
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
                using (IsolatedStorageFileStream restart = IsolatedStorageFile.GetUserStoreForApplication().CreateFile("restart.bin")) {
                    if (_timerState == TimerState.Running) {
                        restart.WriteByte((byte) TimerState.Running);
                        byte[] time = BitConverter.GetBytes(_timerBase.Ticks);
                        restart.Write(time, 0, sizeof (long));
                    } else if (_timerState == TimerState.Paused) {
                        restart.WriteByte((byte) TimerState.Paused);
                        byte[] hours = BitConverter.GetBytes(_timer.Hours);
                        byte[] minutes = BitConverter.GetBytes(_timer.Minutes);
                        byte[] seconds = BitConverter.GetBytes(_timer.Seconds);
                        restart.Write(hours, 0, sizeof (int));
                        restart.Write(minutes, 0, sizeof (int));
                        restart.Write(seconds, 0, sizeof (int));
                    } else {
                        restart.WriteByte((byte) TimerState.Stopped);
                    }
                }
            } catch {
                IsolatedStorageFile.GetUserStoreForApplication().DeleteFile("restart.bin");
            }
        }

        private void ClearRestartTime() { IsolatedStorageFile.GetUserStoreForApplication().DeleteFile("restart.bin"); }

        private void NavigateMainMenu(string v)
        {
            string month = DateTime.Today.ToString("MMMM").ToLower() + " report";
            string yearly = DateTime.Today.ToString("YYYY").ToLower() + " report"; 
            switch (v) {
                case "add time":
                    NavigationService.Navigate(new Uri("/ManuallyEnterTime.xaml", UriKind.Relative));
                    break;
                case "add return visit":
                    NavigationService.Navigate(new Uri("/AddNewRV.xaml", UriKind.Relative));
                    break;
                default:
                    if (v.Equals(month)) {
                        ShowMonthlyReport();
                    } else if (v.Equals(yearly)) {
                        ShowYearlyReport();
                    }
                    break;
            }
        }

        private void ShowYearlyReport()
        {
            DateTime from = new DateTime(DateTime.Today.Year,DateTime.Today.Month,1);
            DateTime to = DateTime.Today;

            TimeData[] entries = TimeDataInterface.GetEntries(from, to, SortOrder.DateOldestToNewest);
            App.ViewModel.LoadTimeReport(entries);
            NavigationService.Navigate(new Uri("/TimeReport.xaml", UriKind.Relative));
        }

        private void ShowMonthlyReport()
        {
            DateTime from = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            DateTime to = new DateTime(from.Year, from.Month, 1).AddMonths(1).AddDays(-1); 

            TimeData[] entries = TimeDataInterface.GetEntries(from, to, SortOrder.DateOldestToNewest);
            App.ViewModel.LoadTimeReport(entries);
            NavigationService.Navigate(new Uri("/TimeReport.xaml", UriKind.Relative));
        }

        private void StackPanel_Tap(object sender, GestureEventArgs e) { }

        #region Nested type: TimerState

        private enum TimerState : byte
        {
            Stopped = 3,
            Paused = 0,
            Running = 1
        };

        #endregion
    }
}