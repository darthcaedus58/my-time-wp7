using System;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using MvvmSettings.ViewModel;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;

namespace MyTime
{
    /// <summary>
    /// Description for Settings.
    /// </summary>
    public partial class SettingsPage : PhoneApplicationPage
    {
        

        /// <summary>
        /// Initializes a new instance of the Settings class.
        /// </summary>
        public SettingsPage()
        {
            DataContext = App.ViewModel;
            InitializeComponent();
            this.Loaded += new System.Windows.RoutedEventHandler(Settings_Loaded);
        }

        void Settings_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var grid = this.FindName("SettingsRoot") as Grid;
            if (null == grid) return;
            var sp = App.AppSettings.BuildXaml();
            grid.Children.Add(sp);

            tbAppVersion.Text = App.GetVersion();
            tbCoreVersion.Text = MyTimeDatabaseLib.Main.GetVersion();
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            App.AppSettings.SaveSettings();

            base.OnNavigatedFrom(e);
        }

        private void button1_Click(object sender, RoutedEventArgs e) 
        { 
            PhoneNumberChooserTask ph = new PhoneNumberChooserTask();
            ph.Show();
            ph.Completed +=new EventHandler<PhoneNumberResult>(ph_Completed);
        }

        private void ph_Completed(object sender, PhoneNumberResult e)
        {
            
        }

        private void HyperlinkButtonRateApp_Click(object sender, RoutedEventArgs e)
        {
            var marketplaceReviewTask = new MarketplaceReviewTask();

            marketplaceReviewTask.Show();
        }

        private void HyperlinkButtonHelpMeQuestion_Click(object sender, RoutedEventArgs e)
        {
            var emailcomposer = new EmailComposeTask();
            emailcomposer.Subject = "Field Service App Help";
            emailcomposer.To = "help@square-hiptobe.com";
            emailcomposer.Body = string.Format("Description of Problem:\n\nSteps to Reproduce:\n\nI hereby give you my permission to contact me regarding this issue.\n\nApplication Version: {0}\nCore Version: {1}", tbAppVersion.Text, tbCoreVersion.Text);
            emailcomposer.Show();
        }
    }
}
