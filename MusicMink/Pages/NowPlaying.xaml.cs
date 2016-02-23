using MusicMink.ViewModels;
using System;
using Weather.JMessbox;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Animation;
using System.Diagnostics;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace MusicMink.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NowPlaying : BasePage
    {
        DispatcherTimer dt = new DispatcherTimer();
        bool flag = true;
        public NowPlaying()
        {

            this.InitializeComponent();
            this.DataContext = LibraryViewModel.Current.PlayQueue;
            VisualStateManager.GoToState(this, "MenuClose", true);
            this.GDMenu.DataContext = LibraryViewModel.Current;
            dt.Interval = TimeSpan.FromSeconds(3);
            dt.Tick += Dt_Tick;
            dt.Start();
            //this.UCLrc.DataContext = LibraryViewModel.Current.PlayQueue.CurrentTrack.Name;
        }

        private void Dt_Tick(object sender, object e)
        {
            if (flag)
            {
                this.SbGoListPage1.Begin();

            }
            else
            {
                this.SbGoListPage2.Begin();

            }
            flag = !flag;
        }

        private void TextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            NavigationManager.Current.Navigate(NavigationLocation.Queue);
        }
        private void TextBlock_Tapped(object sender, RoutedEventArgs e)
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
                    this.GDMask.Visibility = Visibility.Collapsed;
                }
                else
                {
                    VisualStateManager.GoToState(this, "MenuOpen", true);
                    this.GDMask.Visibility = Visibility.Visible;

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
            this.LrcControl.Visibility = Visibility.Visible;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;

        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }
        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            Frame frame = Window.Current.Content as Frame;
            if (frame == null)
            {
                return;
            }
            e.Handled = true;
            JMessBox jb = new JMessBox("再按一次离开");
            jb.Completed += (b) =>
            {
                if (b)
                {
                    //退出代码
                    if (SettingsViewModel.Current.IsStopWhenTerminate)
                    {
                        //暂停播放
                        LibraryViewModel.Current.PlayQueue.ExecutePlayPausePlayer(null);
                    }
                    Application.Current.Exit();
                }
            };
            jb.Show();
        }

        private void GDMask_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (this.VisualStateGroup.CurrentState != null)
            {
                VisualStateManager.GoToState(this, "MenuClose", true);
                this.GDMask.Visibility = Visibility.Collapsed;
            }
        }

        private void FVAlbumArtFliper_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.GDPage != null)
            {


                if ((sender as FlipView).SelectedIndex == 0)
                {


                    if (LibraryViewModel.Current.PlayQueue.PrevPlayer.CanExecute(null))
                    {
                        LibraryViewModel.Current.PlayQueue.PrevPlayer.Execute(null);
                    }
                   (sender as FlipView).SelectedIndex = 1;

                }
                else if ((sender as FlipView).SelectedIndex == 2)
                {

                    if (LibraryViewModel.Current.PlayQueue.SkipPlayer.CanExecute(null))
                    {
                        LibraryViewModel.Current.PlayQueue.SkipPlayer.Execute(null);
                    }
                    (sender as FlipView).SelectedIndex = 1;

                }
            }
        }
    }
}
