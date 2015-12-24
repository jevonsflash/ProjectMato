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
    public sealed partial class SleepModePage : BasePage
    {
        public SleepModePage()
        {
            InitializeComponent();
            this.DataContext = SettingsViewModel.Current;
        }

        private void BTNStart_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)this.BTNStart.IsChecked)
            {
                (Application.Current as App).Smd01.SleepModeSet();
                (Application.Current as App).Smd01.SleepModeOn();
            }
            else
            {
                (Application.Current as App).Smd01.SleepModeOff();
                (Application.Current as App).Smd01.SleepModeUnSet();

            }
        }


        private void BTNIsStopMusic_Tap(object sender, RoutedEventArgs e)
        {
            SettingsViewModel.Current.IsStopWhenTerminate = this.TSIsStopMusic.IsOn;

        }

    }
}


