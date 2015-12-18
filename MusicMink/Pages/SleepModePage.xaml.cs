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

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace MusicMink.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SleepModePage : Page
    {


        private TimeSpan ts01;
        public SleepModePage()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.TSIsStopMusic.IsChecked = SettingsViewModel.Current.IsStopWhenTerminate;

            this.BTNStart.IsChecked = ((Application.Current as App).Smd01.IsSleepModeOn);
            this.TBMessage.Opacity = ((Application.Current as App).Smd01.IsSleepModeOn) ? 0.3 : 1.0;
            this.sliderTimer.IsEnabled = !((Application.Current as App).Smd01.IsSleepModeOn);
            this.ts01 = (Application.Current as App).Smd01.TimeValue;
            this.sliderTimer.Value = this.ts01.TotalSeconds;
            this.sliderTimer.Maximum = 7200;
        }

        private void BTNStart_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)this.BTNStart.IsChecked)
            {
                this.ts01 = TimeSpan.Parse(this.TBTimeValue.Text);
                (Application.Current as App).Smd01.TimeValue = this.ts01;
                (Application.Current as App).Smd01.SleepModeSet();
                (Application.Current as App).Smd01.SleepModeOn();
                this.TBMessage.Opacity = 0.3;
                this.sliderTimer.IsEnabled = false;
            }
            else
            {
                (Application.Current as App).Smd01.SleepModeOff();
                this.TBMessage.Opacity = 1;
                this.sliderTimer.IsEnabled = true;
            }
        }


        private void BTNIsStopMusic_Tap(object sender, RoutedEventArgs e)
        {
            SettingsViewModel.Current.IsStopWhenTerminate = this.TSIsStopMusic.IsChecked;

        }

    }
}

}
