// ***********************************************************************
// Assembly         : MyTime
// Author           : trevo_000
// Created          : 11-03-2012
//
// Last Modified By : trevo_000
// Last Modified On : 11-10-2012
// ***********************************************************************
// <copyright file="App.xaml.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;
using Coding4Fun.Phone.Controls;
using FieldService.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using MyTimeDatabaseLib;

namespace FieldService
{
    /// <summary>
    /// Class App
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// The _settingsProvider
        /// </summary>
        private static SettingsProvider _settingsProvider;

        /// <summary>
        /// The view model
        /// </summary>
        private static MainViewModel _viewModel;

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            (App.Current.Resources["PhoneAccentBrush"] as SolidColorBrush).Color = Color.FromArgb(0xFF, 0xD2, 0xDA, 0x86);
            (App.Current.Resources["PhoneForegroundBrush"] as SolidColorBrush).Color = Color.FromArgb(0xFF, 0x4C, 0x44, 0x78);
            (App.Current.Resources["PhoneBackgroundBrush"] as SolidColorBrush).Color = Color.FromArgb(0xFF, 0x8F, 0xCC, 0xF8);


            _settingsProvider = new SettingsProvider();
            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Show graphics profiling information while debugging.
            if (Debugger.IsAttached) {
                // Display the current frame rate counters.
                Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }
            TimeDataInterface.CheckDatabase();
            ReturnVisitsInterface.CheckDatabase();
            RvPreviousVisitsDataInterface.CheckDatabase();
        }

        /// <summary>
        /// Gets the app settings.
        /// </summary>
        /// <value>The app settings.</value>
        public static SettingsProvider AppSettingsProvider { get { return _settingsProvider ?? new SettingsProvider(); } }

        /// <summary>
        /// A static ViewModel used by the views to bind against.
        /// </summary>
        /// <value>The view model.</value>
        /// <returns>The MainViewModel object.</returns>
        public static MainViewModel ViewModel
        {
            get
            {
                // Delay creation of the view model until necessary
                return _viewModel ?? (_viewModel = new MainViewModel());
            }
        }

        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <value>The root frame.</value>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched

        #region Events

        /// <summary>
        /// Handles the Activated event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ActivatedEventArgs" /> instance containing the event data.</param>
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            // Ensure that application state is restored appropriately
            if (!ViewModel.IsRvDataLoaded) {
                ViewModel.LoadReturnVisitList(SortOrder.DateOldestToNewest);
            }
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        /// <summary>
        /// Handles the Closing event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ClosingEventArgs" /> instance containing the event data.</param>
        private void Application_Closing(object sender, ClosingEventArgs e) { }

        /// <summary>
        /// Handles the Deactivated event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DeactivatedEventArgs" /> instance containing the event data.</param>
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            // Ensure that required application state is persisted here.
        }

        /// <summary>
        /// Handles the Launching event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="LaunchingEventArgs" /> instance containing the event data.</param>
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            
            LittleWatson.CheckForPreviousException();
        }

        // Code to execute if a navigation fails

        // Code to execute on Unhandled Exceptions
        /// <summary>
        /// Handles the UnhandledException event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ApplicationUnhandledExceptionEventArgs" /> instance containing the event data.</param>
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            LittleWatson.ReportException(e.ExceptionObject, DateTime.Now.ToShortDateString());
            if (Debugger.IsAttached) {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
        }

        #endregion

        #region Phone application initialization

        // Avoid double-initialization
        /// <summary>
        /// The phone application initialized
        /// </summary>
        private bool _phoneApplicationInitialized;

        // Do not add any additional code to this method

        // Do not add any additional code to this method

        #region Events

        /// <summary>
        /// Completes the initialize phone application.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NavigationEventArgs" /> instance containing the event data.</param>
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion

        /// <summary>
        /// Initializes the phone application.
        /// </summary>
        private void InitializePhoneApplication()
        {
            LittleWatson.CheckForPreviousException();
            if (_phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            _phoneApplicationInitialized = true;
        }

        #endregion

        /// <summary>
        /// Handles the NavigationFailed event of the RootFrame control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="NavigationFailedEventArgs" /> instance containing the event data.</param>
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            LittleWatson.ReportException(e.Exception, DateTime.Now.ToShortDateString());

            if (Debugger.IsAttached) {
                // A navigation has failed; break into the debugger
                Debugger.Break();
            }
        }

        /// <summary>
        /// Toasts me.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="title">The title.</param>
        public static void ToastMe(string content, string title = "Field Service")
        {
            var toast = new ToastPrompt {
                                            Title = title,
                                            Message = content
                                        };

            toast.Show();
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <returns>System.String.</returns>
        public static string GetVersion() { return Regex.Match(Assembly.GetExecutingAssembly().FullName, @"Version=(?<version>[\d\.]*)").Groups["version"].Value; }
    }
}