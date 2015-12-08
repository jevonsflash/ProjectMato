using MusicMink.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace MusicMink.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : BasePage
    {
        private Binding savedWidthBinding;

        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = LibraryViewModel.Current.PlayQueue;
        }


        private void Rectangle_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (Math.Abs(e.Cumulative.Translation.Y) < 10)
            {
                if (e.Cumulative.Translation.X > 30 && LibraryViewModel.Current.PlayQueue.PrevPlayer.CanExecute(null))
                {
                    LibraryViewModel.Current.PlayQueue.PrevPlayer.Execute(null);
                }
                else if (e.Cumulative.Translation.X < -30 && LibraryViewModel.Current.PlayQueue.SkipPlayer.CanExecute(null))
                {
                    LibraryViewModel.Current.PlayQueue.SkipPlayer.Execute(null);
                }
            }
        }


        private void BTSetting_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.Current.Navigate(NavigationLocation.SettingsPage);
        }

        private void BTList_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.Current.Navigate(NavigationLocation.Library);
        }

        private void BTMode_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BTPre_Click(object sender, RoutedEventArgs e)
        {
            if (LibraryViewModel.Current.PlayQueue.PrevPlayer.CanExecute(null))
            {
                LibraryViewModel.Current.PlayQueue.PrevPlayer.Execute(null);
            }
        }

        private void BTPlay_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BTNext_Click(object sender, RoutedEventArgs e)
        {
            if (LibraryViewModel.Current.PlayQueue.SkipPlayer.CanExecute(null))
            {
                LibraryViewModel.Current.PlayQueue.SkipPlayer.Execute(null);
            }

        }

        private void BTCurrent_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.Current.Navigate(NavigationLocation.Queue);

        }

        private void HandlePlayerControlProgressBarBezzelManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            ProgressBarScrubView.Visibility = Visibility.Visible;

            BindingExpression bindingExpression = PlayerControlProgressBarCompleted.GetBindingExpression(Rectangle.WidthProperty);
            savedWidthBinding = bindingExpression.ParentBinding;
        }

        private void HandlePlayerControlProgressBarBezzelManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            double newWidth = e.Position.X;

            if (newWidth < 0) newWidth = 0;
            if (newWidth > PlayerControlProgressBarFull.ActualWidth) newWidth = PlayerControlProgressBarFull.ActualWidth;

            PlayerControlProgressBarCompleted.Width = newWidth;

            double percentage = newWidth / PlayerControlProgressBarFull.ActualWidth;

            TimeSpan newTime = TimeSpan.FromTicks((long)(percentage * LibraryViewModel.Current.PlayQueue.TotalTicks));

            //PlayerControlText.Text = newTime.ToString(@"%m\:ss") + @"/" + LibraryViewModel.Current.PlayQueue.TotalTime;

        }

        private void HandlePlayerControlProgressBarBezzelManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            double newWidth = e.Position.X;

            if (newWidth < 0) newWidth = 0;
            if (newWidth > PlayerControlProgressBarFull.ActualWidth) newWidth = PlayerControlProgressBarFull.ActualWidth;

            PlayerControlProgressBarCompleted.Width = newWidth;

            double percentage = newWidth / PlayerControlProgressBarFull.ActualWidth;

            LibraryViewModel.Current.PlayQueue.ScrubToPercentage(percentage);

            ProgressBarScrubView.Visibility = Visibility.Collapsed;

            PlayerControlProgressBarCompleted.SetBinding(Rectangle.WidthProperty, savedWidthBinding);
            savedWidthBinding = null;

            //PlayerControlText.Text = string.Empty;
        }

        private void CurrentTrackPanel_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (Math.Abs(e.Cumulative.Translation.Y) < 10)
            {
                if (e.Cumulative.Translation.X > 30 && LibraryViewModel.Current.PlayQueue.PrevPlayer.CanExecute(null))
                {
                    LibraryViewModel.Current.PlayQueue.PrevPlayer.Execute(null);
                }
                else if (e.Cumulative.Translation.X < -30 && LibraryViewModel.Current.PlayQueue.SkipPlayer.CanExecute(null))
                {
                    LibraryViewModel.Current.PlayQueue.SkipPlayer.Execute(null);
                }
            }
        }

        private void BTNLrc_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
