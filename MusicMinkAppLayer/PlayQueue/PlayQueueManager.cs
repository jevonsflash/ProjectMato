﻿using MusicMinkAppLayer.Diagnostics;
using MusicMinkAppLayer.Helpers;
using MusicMinkAppLayer.Tables;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Playback;
using Windows.Storage;

namespace MusicMinkAppLayer.PlayQueue
{
    /// <summary>
    /// Used by the Background Process to work the play queue (Foreground process uses PlayQueueModel)
    /// </summary>
    public class PlayQueueManager
    {
        private static PlayQueueManager _current;
        public static PlayQueueManager Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new PlayQueueManager();
                }

                return _current;
            }
        }

        private MediaPlayer mediaPlayer;

        public event TypedEventHandler<PlayQueueManager, TrackInfo> TrackChanged;

        public bool IsActive { get; private set; }

        private PlayQueueManager()
        {
            mediaPlayer = BackgroundMediaPlayer.Current;
            mediaPlayer.MediaPlayerRateChanged += HandleMediaPlayerMediaPlayerRateChanged;
            mediaPlayer.MediaFailed += HandleMediaPlayerMediaFailed;
            mediaPlayer.MediaEnded += HandleMediaPlayerMediaEnded;
            mediaPlayer.MediaOpened += HandleMediaPlayerMediaOpened;

            mediaPlayer.AutoPlay = false;

            IsActive = false;
        }

        #region Event Handlers

        private void HandleMediaPlayerMediaPlayerRateChanged(MediaPlayer sender, MediaPlayerRateChangedEventArgs args)
        {
        }

        private void HandleMediaPlayerMediaFailed(MediaPlayer sender, MediaPlayerFailedEventArgs args)
        {
            Logger.Current.Log(new CallerInfo(), LogLevel.Warning, "Media Player Failed With Error code {0}", args.ExtendedErrorCode.ToString());
        }

        private TrackInfo playingTrack = null;

        private void HandleMediaPlayerMediaEnded(MediaPlayer sender, object args)
        {
            DebugHelper.Assert(new CallerInfo(), playingTrack != null);

            if (playingTrack != null)
            {
                DatabaseManager.Current.AddHistoryItem(new HistoryTable(playingTrack.SongId, false, false, DateTime.UtcNow.Ticks, playingTrack.Title, playingTrack.Artist));

                SendMessageToForeground(PlayQueueConstantBGMessageId.TrackPlayed);
            }
           
            PlayNext();
        }

        private bool isFirstOpen = false;
        private bool playAfterOpen = true;

        void HandleMediaPlayerMediaOpened(MediaPlayer sender, object args)
        {
            if (isFirstOpen)
            {
                isFirstOpen = false;
                double percentage = ApplicationSettings.GetSettingsValue<double>(ApplicationSettings.CURRENT_TRACK_PERCENTAGE, 0.0);
                ApplicationSettings.PutSettingsValue(ApplicationSettings.CURRENT_TRACK_PERCENTAGE, 0.0);

                if (percentage > 0)
                {
                    Logger.Current.Init(LogType.PlayAction);

                    Logger.Current.Log(new CallerInfo(), LogLevel.Info, "Length Total {0}", mediaPlayer.NaturalDuration.Ticks);

                    mediaPlayer.Position = TimeSpan.FromTicks((long)(mediaPlayer.NaturalDuration.Ticks * percentage));
                }
            }

            int trackId = ApplicationSettings.GetSettingsValue<int>(ApplicationSettings.CURRENT_PLAYQUEUE_POSITION, 0);
            Logger.Current.Init(LogType.PlayAction);

            Logger.Current.Log(new CallerInfo(), LogLevel.Info, "Trying to play row {0}", trackId);

            playingTrack = TrackInfo.TrackInfoFromRowId(trackId);
            TrackChanged.Invoke(this, playingTrack);

            if (playAfterOpen)
            {
                sender.Play();
            }
            else
            {
                playAfterOpen = true;
            }
        }

        #endregion

        #region Methods

        public bool CanSkip()
        {
            int rowId = ApplicationSettings.GetSettingsValue<int>(ApplicationSettings.CURRENT_PLAYQUEUE_POSITION, 0);

            PlayQueueEntryTable currentPlayQueueRow = DatabaseManager.Current.LookupPlayQueueEntryById(rowId);

            return (currentPlayQueueRow != null && currentPlayQueueRow.NextId > 0);
        }

        public bool CanBack()
        {
            int rowId = ApplicationSettings.GetSettingsValue<int>(ApplicationSettings.CURRENT_PLAYQUEUE_POSITION, 0);

            PlayQueueEntryTable currentPlayQueueRow = DatabaseManager.Current.LookupPlayQueueEntryById(rowId);

            return (currentPlayQueueRow != null && currentPlayQueueRow.PrevId > 0);
        }

        public async void PlayNext()
        {
            int rowId = ApplicationSettings.GetSettingsValue<int>(ApplicationSettings.CURRENT_PLAYQUEUE_POSITION, 0);

            PlayQueueEntryTable currentPlayQueueRow = DatabaseManager.Current.LookupPlayQueueEntryById(rowId);

            if (currentPlayQueueRow != null)
            {
                int nextRowId = currentPlayQueueRow.NextId;
                ApplicationSettings.PutSettingsValue(ApplicationSettings.CURRENT_PLAYQUEUE_POSITION, nextRowId);

                if (nextRowId == 0)
                {
                    SendMessageToForeground(PlayQueueConstantBGMessageId.PlayQueueFinished);
                }
            }
            else
            {
                PlayQueueEntryTable head = DatabaseManager.Current.LookupPlayQueueEntryHead();

                if (head != null)
                {
                    ApplicationSettings.PutSettingsValue(ApplicationSettings.CURRENT_PLAYQUEUE_POSITION, head.RowId);
                }
                else
                {
                    ApplicationSettings.PutSettingsValue(ApplicationSettings.CURRENT_PLAYQUEUE_POSITION, 0);
                }
            }

            await PlayCurrent();
        }

        public async void PlayPrev()
        {
            int rowId = ApplicationSettings.GetSettingsValue<int>(ApplicationSettings.CURRENT_PLAYQUEUE_POSITION, 0);

            PlayQueueEntryTable currentPlayQueueRow = DatabaseManager.Current.LookupPlayQueueEntryById(rowId);

            int prevRowId = 0;
            if (currentPlayQueueRow != null)
            {
                prevRowId = currentPlayQueueRow.PrevId;
            }

            ApplicationSettings.PutSettingsValue(ApplicationSettings.CURRENT_PLAYQUEUE_POSITION, prevRowId);

            await PlayCurrent();
        }

        /// <summary>
        /// Tries to play the track with TrackInfo. If MakeActive is false, doesn't start playback just
        /// moves the play queue to that track
        /// </summary>
        /// <param name="trackInfo"></param>
        /// <param name="makeActive"></param>
        /// <returns></returns>
        async Task PlayTrack(TrackInfo trackInfo, bool makeActive)
        {
            try
            {
                StorageFile file = await StorageFile.GetFileFromPathAsync(trackInfo.Path);

                if (!makeActive)
                {
                    playAfterOpen = false;
                }
                else
                {
                    isFirstOpen = true;
                }

                mediaPlayer.SetFileSource(file);
                IsActive = makeActive;
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.Current.Log(new CallerInfo(), LogLevel.Error, "Couldn't play track {0} got UnauthorizedAccessException message {1}", trackInfo.SongId, ex.Message);

                IsActive = false;

                PlayNext();
            }
            catch (FileNotFoundException ex)
            {
                Logger.Current.Log(new CallerInfo(), LogLevel.Error, "Couldn't play track {0} got FileNotFoundException message {1}", trackInfo.SongId, ex.Message);

                IsActive = false;

                PlayNext();
            }
        }

        async Task PlayCurrent()
        {
            int rowId = ApplicationSettings.GetSettingsValue<int>(ApplicationSettings.CURRENT_PLAYQUEUE_POSITION, 0);

            TrackInfo trackInfo = null;

            if (rowId > 0)
            {
                trackInfo = TrackInfo.TrackInfoFromRowId(rowId);
            }

            // TODO: #18  Support other types
            if (trackInfo != null)
            {
                await PlayTrack(trackInfo, true);
            }
            else
            {
                IsActive = false;

                PlayQueueEntryTable head = DatabaseManager.Current.LookupPlayQueueEntryHead();

                if (head != null)
                {
                    ApplicationSettings.PutSettingsValue(ApplicationSettings.CURRENT_PLAYQUEUE_POSITION, head.RowId);

                    trackInfo = TrackInfo.TrackInfoFromRowId(head.RowId);

                    if (trackInfo != null)
                    {
                        await PlayTrack(trackInfo, false);
                    }
                }
                else
                {
                    ApplicationSettings.PutSettingsValue(ApplicationSettings.CURRENT_PLAYQUEUE_POSITION, 0);
                }
            }
        }

        public async void Play()
        {
            int rowId = ApplicationSettings.GetSettingsValue<int>(ApplicationSettings.CURRENT_PLAYQUEUE_POSITION, 0);

            if (rowId == 0)
            {
                ResetAndPlay();
            }
            else
            {
                await PlayCurrent();
            }
        }

        public async void ResetAndPlay()
        {
            PlayQueueEntryTable head = DatabaseManager.Current.LookupPlayQueueEntryHead();

            if (head != null)
            {
                ApplicationSettings.PutSettingsValue(ApplicationSettings.CURRENT_PLAYQUEUE_POSITION, head.RowId);
            }
            else
            {
                ApplicationSettings.PutSettingsValue(ApplicationSettings.CURRENT_PLAYQUEUE_POSITION, 0);
            }

            await PlayCurrent();
        }

        public void Connect()
        {
            Logger.Current.Log(new CallerInfo(), LogLevel.Warning, "Connecting DB");

            DatabaseManager.Current.Connect();
        }

        public void Disconnect()
        {
            Logger.Current.Log(new CallerInfo(), LogLevel.Warning, "Disconnect DB");

            if (IsActive && mediaPlayer.NaturalDuration.TotalSeconds > 0)
            {
                double percentage = mediaPlayer.Position.TotalSeconds / mediaPlayer.NaturalDuration.TotalSeconds;
                ApplicationSettings.PutSettingsValue(ApplicationSettings.CURRENT_TRACK_PERCENTAGE, percentage);
            }
            else
            {
                ApplicationSettings.PutSettingsValue(ApplicationSettings.CURRENT_TRACK_PERCENTAGE, 0.0);
            }

            IsActive = false;
            DatabaseManager.Current.Disconnect();
        }

        #endregion

        public void SendMessageToForeground(PlayQueueConstantBGMessageId messageId, object value)
        {
            var message = new ValueSet();
            message.Add(PlayQueueMessageHelper.BGMessageIdToMessageString(messageId), value);
            BackgroundMediaPlayer.SendMessageToForeground(message);
        }

        public void SendMessageToForeground(PlayQueueConstantBGMessageId messageId)
        {
            SendMessageToForeground(messageId, DateTime.Now.ToString());
        }
    }
}
