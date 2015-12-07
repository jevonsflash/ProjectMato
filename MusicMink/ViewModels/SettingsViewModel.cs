using MusicMink.Common;
using MusicMinkAppLayer.Diagnostics;
using MusicMinkAppLayer.Helpers;
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
            public const string AutoPullArtFromLastFM = "AutoPullArtFromLastFM";
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

        private RelayCommand _uploadLogs;
        public RelayCommand UploadLogs
        {
            get
            {
                if (_uploadLogs == null) _uploadLogs = new RelayCommand(CanExecuteUploadLogs, ExecuteUploadLogs);

                return _uploadLogs;
            }
        }

        private async void ExecuteUploadLogs(object parameter)
        {
            await Logger.Current.Flush();

            StorageFolder localFolder = ApplicationData.Current.LocalFolder;

            StorageFolder logFolder = await localFolder.GetFolderAsync(Logger.LOG_FOLDER);

            if (logFolder == null) return;

            IReadOnlyList<StorageFile> files = await logFolder.GetFilesAsync();

            EmailRecipient sendTo = new EmailRecipient(EMAIL_TARGET, EMAIL_NAME);

            EmailMessage mail = new EmailMessage();
            mail.Subject = Strings.GetResource("SendLogsSubject");
            mail.Body = Strings.GetResource("SendLogsBody");

            mail.To.Add(sendTo);

            foreach (IStorageFile file in files)
            {
                EmailAttachment logs = new EmailAttachment(file.Name, file);

                mail.Attachments.Add(logs);
            }

            await EmailManager.ShowComposeNewEmailAsync(mail);
        }

        private bool CanExecuteUploadLogs(object parameter)
        {
            return true;
        }

        #endregion

        #region Properties

        public bool AutoPullArtFromLastFM
        {
            get
            {
                return GetSettingField<bool>(ApplicationSettings.SETTING_IS_AUTO_PULL_ART_FROM_LASTFM_ON, true);
            }
            set
            {
                SetSettingField<bool>(ApplicationSettings.SETTING_IS_AUTO_PULL_ART_FROM_LASTFM_ON, value, Properties.AutoPullArtFromLastFM);
            }
        }

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

        public string BackgroundKey
        {
            get
            {
                return GetSettingField<string>(ApplicationSettings.BACKGROUND_KEY, "mato");
            }
            set
            {
                SetSettingField<string>(ApplicationSettings.BACKGROUND_KEY, value, Properties.BackgroundKey);
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
