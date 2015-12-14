using MusicMink.ViewModels;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace MusicMink.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NowPlaying : BasePage
    {

        public NowPlaying()
        {

            this.InitializeComponent();
            this.DataContext = LibraryViewModel.Current.PlayQueue;
            VisualStateManager.GoToState(this, "MenuClose", true);
            this.GDMenu.DataContext = LibraryViewModel.Current;
        }

        private void PrevTrackImageDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (LibraryViewModel.Current.PlayQueue.PrevPlayer.CanExecute(null))
            {
                LibraryViewModel.Current.PlayQueue.PrevPlayer.Execute(null);
            }
        }

        private void NextTrackImageDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (LibraryViewModel.Current.PlayQueue.SkipPlayer.CanExecute(null))
            {
                LibraryViewModel.Current.PlayQueue.SkipPlayer.Execute(null);
            }
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

        private void TextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            NavigationManager.Current.Navigate(NavigationLocation.Queue);
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

        private void BTNMainMenu_Click(object sender, RoutedEventArgs e)
        {
            if (this.VisualStateGroup.CurrentState != null)
            {
                if (this.VisualStateGroup.CurrentState.Name == "MenuOpen")
                {
                    VisualStateManager.GoToState(this, "MenuClose", true);
                }
                else
                {
                    VisualStateManager.GoToState(this, "MenuOpen", true);
                }
            }
            else
            {
                VisualStateManager.GoToState(this, "MenuOpen", true);

            }

        }


        Binding savedWidthBinding;
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

            PlayerControlText.Text = newTime.ToString(@"%m\:ss") + @"/" + LibraryViewModel.Current.PlayQueue.TotalTime;

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

            PlayerControlText.Text = string.Empty;
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            this.UCLrc.Visibility = Visibility.Visible;
        }
    }
}
