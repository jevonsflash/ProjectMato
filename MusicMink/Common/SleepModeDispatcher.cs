using MusicMink.ViewModels;
using MusicMinkAppLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.UI.Xaml;

namespace MusicMink.Common
{

    public class SleepModeDispatcher
    {
        private DispatcherTimer timer = new DispatcherTimer();

        public DispatcherTimer Timer
        {
            get { return timer; }
            set { timer = value; }
        }

        public SleepModeDispatcher()
        {
            SettingsViewModel.Current.TimingOffValue = new TimeSpan(0, 20, 0).TotalSeconds;
            SettingsViewModel.Current.IsSleepModeOn = false;
        }

        public void SleepModeSet()
        {
            Timer.Interval = TimeSpan.FromSeconds(SettingsViewModel.Current.TimingOffValue);
            Timer.Tick += Timer_Tick1;
        }

        public void SleepModeUnSet()
        {
            Timer.Tick -= Timer_Tick1;
        }

        private void Timer_Tick1(object sender, object e)
        {
            if (SettingsViewModel.Current.IsStopWhenTerminate)
            {
                //暂停播放
                LibraryViewModel.Current.PlayQueue.ExecutePlayPausePlayer(null);
            }

            Timer.Tick -= Timer_Tick1;
            Application.Current.Exit();
        }

        public void SleepModeOn()
        {
            Timer.Start();
        }
        public void SleepModeOff()
        {
            Timer.Stop();
        }
    }
}
