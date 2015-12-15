using MusicMink.Common;
using MusicMinkAppLayer.Diagnostics;
using MusicMinkAppLayer.Helpers;
using MusicMinkAppLayer.Models;
using MusicMinkAppLayer.Tables;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.ApplicationModel.Email;
using Windows.Storage;

namespace MusicMink.ViewModels
{
    class SettingsViewModel : NotifyPropertyChangedUI
    {
        public const string EMAIL_TARGET = "chimewp@gmail.com";
        public const string EMAIL_NAME = "MusicMink";

        public static class Properties
        {
            public const string IsNewSeason = "IsNewSeason";
            public const string IsSleepModeOn = "IsSleepModeOn";
            public const string IsStopWhenTerminate = "IsStopWhenTerminate";
            public const string IsAutoLrc = "IsAutoLrc";
            public const string IsAutoOffset = "IsAutoOffset";
        }

        private static SettingsViewModel _current;
        public static SettingsViewModel Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new SettingsViewModel();
                }

                return _current;
            }
        }

        private SettingsViewModel()
        {
        }

        #region Commands

        #endregion

        #region Properties


        public bool IsSleepModeOn
        {
            get
            {
                return GetSettingField<bool>(ApplicationSettings.IS_SLEEPMODE_ON, false);
            }
            set
            {
                SetSettingField<bool>(ApplicationSettings.IS_SLEEPMODE_ON, value, Properties.IsSleepModeOn);
            }
        }

        public bool IsStopWhenTerminate
        {
            get
            {
                return GetSettingField<bool>(ApplicationSettings.IS_STOP_WHEN_TERMINATE, false);
            }
            set
            {
                SetSettingField<bool>(ApplicationSettings.IS_STOP_WHEN_TERMINATE, value, Properties.IsStopWhenTerminate);
            }
        }

        public bool IsAutoLrc
        {
            get
            {
                return GetSettingField<bool>(ApplicationSettings.IS_AUTO_LRC, true);
            }
            set
            {
                SetSettingField<bool>(ApplicationSettings.IS_AUTO_LRC, value, Properties.IsAutoLrc);
            }
        }
        public bool IsAutoOffset
        {
            get
            {
                return GetSettingField<bool>(ApplicationSettings.IS_AUTO_OFFSET, true);
            }
            set
            {
                SetSettingField<bool>(ApplicationSettings.IS_AUTO_OFFSET, value, Properties.IsAutoOffset);
            }
        }

        public string IsNewSeason
        {
            get
            {
                return GetSettingField<string>(ApplicationSettings.SETTING_IS_NEW_SEASON, "0.0.0");
            }
            set
            {
                SetSettingField<string>(ApplicationSettings.SETTING_IS_NEW_SEASON, value, Properties.IsNewSeason);
            }
        }


        public List<BackgroundModel> BackgroundList
        {
            get { return BackgroundsModel.Current.GetBackgroundList(); }
            set { BackgroundsModel.Current.UpdateBackground(value); }
        }

        #endregion



        #region ReflectionHelpers

        protected void SetSettingField<U>(string setting_key, U value, string property)
        {
            U currentValue = ApplicationSettings.GetSettingsValue<U>(setting_key, default(U));

            if (!EqualityComparer<U>.Default.Equals(currentValue, value))
            {
                ApplicationSettings.PutSettingsValue(setting_key, value);
                NotifyPropertyChanged(property);
            }
        }

        protected U GetSettingField<U>(string setting_key, U defaultValue)
        {
            return ApplicationSettings.GetSettingsValue<U>(setting_key, defaultValue);
        }

        #endregion
    }
}
