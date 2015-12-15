﻿using MusicMink.MediaSources;
using MusicMink.Pages;
using MusicMink.ViewModels;
using MusicMinkAppLayer.Diagnostics;
using MusicMinkAppLayer.Helpers;
using MusicMinkAppLayer.Models;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace MusicMink
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        private TransitionCollection transitions;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;
            this.Resuming += this.OnResuming;
            Application.Current.RequestedTheme = ApplicationTheme.Dark;
            this.UnhandledException += LogUnhandledException;
        }

        async void LogUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            DebugHelper.Alert(new CallerInfo(), "UnhandledException: TYPE: {0} MESSAGE: {1} STRING: {2} SOURCE: {3}", e.Exception.GetType(), e.Message, e.Exception.ToString(), e.Exception.Source);

            await Logger.Current.Flush();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Logger.Current.Init(LogType.FG);

            Logger.Current.Log(new CallerInfo(), LogLevel.Info, "App launched");

            PerfTracer pt = new PerfTracer("Startup Perf");
            LibraryViewModel.Current.PreInitalize();

            UpdateTheme();

            pt.Trace("LibraryViewModelInitalized");

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                // TODO: change this value to a cache size that is appropriate for your application
                rootFrame.CacheSize = 1;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // Removes the turnstile navigation for startup.
                if (rootFrame.ContentTransitions != null)
                {
                    this.transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        this.transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += this.RootFrame_FirstNavigated;

                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                
                Type target = typeof(LandingPage);
                if (SettingsViewModel.Current.IsNewSeason!= (string)App.Current.Resources["Version"])
                {
                    SettingsViewModel.Current.IsNewSeason = (string)App.Current.Resources["Version"];
                    target = typeof(GuidePage);
                }

                if (!rootFrame.Navigate(target, e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            // Ensure the current window is active
            Window.Current.Activate();

            // TODO: need to do this async to get load times down
            LibraryViewModel.Current.InitalizeLibrary();

            MediaImportManager.Current.Initalize();

            pt.Trace("Startup Done");
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);

            var continuationEventArgs = args as IContinuationActivatedEventArgs;
            if (continuationEventArgs != null)
            {
                NavigationManager.Current.HandleContinuation(continuationEventArgs);
            }
        }

        private void UpdateTheme()
        {
            SolidColorBrush brush = DebugHelper.CastAndAssert<SolidColorBrush>(App.Current.Resources["PhoneAccentBrush"]);

            Color c = brush.Color;

            c.R = (byte)(byte.MaxValue - c.R * 0.2);
            c.G = (byte)(byte.MaxValue - c.G * 0.2);
            c.B = (byte)(byte.MaxValue - c.B * 0.2);

            ((SolidColorBrush)App.Current.Resources["PlayerControlBackgroundColor"]).Color = c;
            BackgroundModel bm = BackgroundsModel.Current.GetBackgroundList().Find(p => p.IsSel == true);
            var bitmapImage = new BitmapImage(new Uri(string.Format("ms-resource:/Files{0}", bm.Img)));
            if (Application.Current.Resources.Keys.Contains("MainBackGroundBrush"))
            {
                ((ImageBrush)Application.Current.Resources["MainBackGroundBrush"]).ImageSource = bitmapImage;
            }
        }

        /// <summary>
        /// Restores the content transitions after the app has launched.
        /// </summary>
        /// <param name="sender">The object where the handler is attached.</param>
        /// <param name="e">Details about the navigation event.</param>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            LibraryViewModel.Current.PlayQueue.Suspend();

            Logger.Current.Log(new CallerInfo(), LogLevel.Info, "App suspended");

            await Logger.Current.Flush();

            deferral.Complete();
        }

        void OnResuming(object sender, object e)
        {
            Logger.Current.Init(LogType.FG);

            LibraryViewModel.Current.PlayQueue.Resume();

            Logger.Current.Log(new CallerInfo(), LogLevel.Info, "App resumed");
        }
    }
}