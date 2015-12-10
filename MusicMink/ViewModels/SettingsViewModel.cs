using MusicMink.Common;
using MusicMinkAppLayer.Diagnostics;
using MusicMinkAppLayer.Helpers;
using MusicMinkAppLayer.Models;
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
            public const string IsSleepModeOn = "IsSleepModeOn";
            public const string IsStopWhenTerminate = "IsStopWhenTerminate";
            public const string IsAutoLrc = "IsAutoLrc";
            public const string IsAutoOffset = "IsAutoOffset";
            public const string BackgroundKey = "BackgroundKey";
            public const string IsClassicModeEnabled = "IsClassicModeEnabled";
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

        public BackgroundModel BackgroundKey
        {
            get
            {
                return GetSettingField<BackgroundModel>(ApplicationSettings.BACKGROUND_KEY, new BackgroundModel() {
                    Backgrounds=new List<BackgroundEntityModel>() {
                        new BackgroundEntityModel(0, "mato", "mato", "ss", true, ""),
                        new BackgroundEntityModel(0, "mato", "mato", "DayResource", false, ""),
                        new BackgroundEntityModel(0, "mato","mato","ss",false,""),
                        new BackgroundEntityModel(0, "mato","mato","ss",true,"")
                    }
                });
            }
            set
            {
                SetSettingField<BackgroundModel>(ApplicationSettings.BACKGROUND_KEY, value, Properties.BackgroundKey);
                ThemeManager.Load(string.Format("/{0}.xaml",value.Backgrounds.Find(c=>c.IsSel==true).Img));
            }
        }


        public bool IsClassicModeEnabled
        {
            get
            {
                return GetSettingField<bool>(ApplicationSettings.SETTING_IS_CLASSIC_MODE_ON, false);
            }
            set
            {
                SetSettingField<bool>(ApplicationSettings.SETTING_IS_CLASSIC_MODE_ON, value, Properties.IsClassicModeEnabled);
            }
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
