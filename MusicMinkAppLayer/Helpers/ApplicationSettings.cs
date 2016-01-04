using MusicMinkAppLayer.Diagnostics;
using Windows.Storage;

namespace MusicMinkAppLayer.Helpers
{
    /// <summary>
    /// Helper for accessing values from ApplicationData
    /// </summary>
    public static class ApplicationSettings
    {
        #region KEYS

        public const string SETTING_IS_NEW_SEASON = "SETTING_IS_NEW_SEASON";

        public const string SETTING_IS_AUTO_PULL_ART_FROM_LASTFM_ON = "SETTING_IS_AUTO_PULL_ART_FROM_LASTFM_ON_KEY";

        public const string LIBRARY_LAST_SUCCESFUL_SYNC_DATE = "LIBRARY_LAST_SUCCESFUL_SYNC_DATE_KEY";

        public const string IS_BACKGROUND_PROCESS_ACTIVE = "IS_BACKGROUND_PROCESS_ACTIVE_KEY";
        public const string IS_FOREGROUND_PROCESS_ACTIVE = "IS_FOREGROUND_PROCESS_ACTIVE_KEY";

        public const string CURRENT_PLAYQUEUE_POSITION = "CURRENT_PLAYQUEUE_POSITION_KEY";
        public const string CURRENT_TRACK_PERCENTAGE = "CURRENT_TRACK_PERCENTAGE_KEY";

        public const string IS_SLEEPMODE_ON = "IS_SLEEPMODE_ON";
        public const string TIMING_OFF_VALUE = "TIMING_OFF_VALUE";
        public const string IS_STOP_WHEN_TERMINATE = "IS_STOP_WHEN_TERMINATE";
        public const string IS_AUTO_LRC = "IS_AUTO_LRC";
        public const string IS_AUTO_OFFSET = "IS_AUTO_OFFSET";
        public const string IS_AUTO_GA = "IS_AUTO_GA";
        public const string BACKGROUND_KEY = "BACKGROUND_KEY";

        #endregion

        public static T GetSettingsValue<T>(string key, T defaultValue)
        {
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey(key))
            {
                ApplicationData.Current.LocalSettings.Values.Add(key, defaultValue);

                Logger.Current.Log(new CallerInfo(), LogLevel.Warning, "Setting {0} does not exist, defaulting to {1}", key, defaultValue);
            }

            return DebugHelper.CastAndAssert<T>(ApplicationData.Current.LocalSettings.Values[key]);
        }

        public static void PutSettingsValue(string key, object value)
        {
            Logger.Current.Init(LogType.ApplicationSettings);
            Logger.Current.Log(new CallerInfo(), LogLevel.Info, "Setting ApplicationSettingKey {0} to {1}", key, value);
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey(key))
            {
                ApplicationData.Current.LocalSettings.Values.Add(key, value);
            }
            else
            {
                ApplicationData.Current.LocalSettings.Values[key] = value;
            }
        }
    }
}
