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
        private bool isSleepModeOn;
        public bool IsSleepModeOn
        {
            get { return isSleepModeOn; }
            set { isSleepModeOn = value; }
        }
        private TimeSpan timeValue;

        public TimeSpan TimeValue
        {
            get { return timeValue; }
            set { timeValue = value; }
        }
        private DispatcherTimer timer = new DispatcherTimer();
        public SleepModeDispatcher()
        {
            this.TimeValue = new TimeSpan(0, 20, 0);
            this.IsSleepModeOn = false;
        }
        public DispatcherTimer Timer
        {
            get { return timer; }
            set { timer = value; }
        }
        public void SleepModeSet()
        {
            timer.Interval = this.TimeValue;
            timer.Tick += Timer_Tick1;

        }

        private void Timer_Tick1(object sender, object e)
        {
            if (SettingsViewModel.Current.IsStopWhenTerminate)
            {

                //暂停播放
                LibraryViewModel.Current.PlayQueue.ExecutePlayPausePlayer(null);

            }

            Application.Current.Exit();

        }

        public void SleepModeOn()
        {
            timer.Start();
            this.IsSleepModeOn = true;
        }
        public void SleepModeOff()
        {
            timer.Stop();
            this.IsSleepModeOn = false;
        }
    }
}
