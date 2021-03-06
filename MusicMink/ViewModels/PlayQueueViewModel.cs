﻿using MusicMink.Collections;
using MusicMink.Common;
using MusicMinkAppLayer.Diagnostics;
using MusicMinkAppLayer.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace MusicMink.ViewModels
{
    public class PlayQueueViewModel : BaseViewModel<PlayQueueModel>
    {
        public static class Properties
        {
            public const string CurrentTrack = "CurrentTrack";
            public const string CurrentTrackRowId = "CurrentTrackRowId";
            public const string CurrentTrackPosition = "CurrentTrackPosition";
            public const string PlaybackQueue = "PlaybackQueue";
            public const string IsPlaying = "IsPlaying";
            public const string IsActive = "IsActive";
            public const string TotalTicks = "TotalTicks";
            public const string TotalTime = "TotalTime";
            public const string TimeLeft = "TimeLeft";
            public const string ElapsedTime = "ElapsedTime";
            public const string PercentTime = "PercentTime";
            public const string NextTrack = "NextTrack";
            public const string PrevTrack = "PrevTrack";
            public const string ProgressTime = "ProgressTime";
            public const string TracksLeft = "TracksLeft";
            public const string IsEmpty = "IsEmpty";
            public const string IsLoop = "IsLoop";
            public const string VolumeValue = "VolumeValue";
        }

        private Dictionary<int, PlayQueueEntryViewModel> LookupDictionary = new Dictionary<int, PlayQueueEntryViewModel>();

        Random seed = new Random();

        public PlayQueueViewModel(PlayQueueModel model) : base(model)
        {
            SettingsViewModel.Current.PropertyChanged += HandleSettingsViewModelPropertyChanged;

            rootModel.PropertyChanged += HandleRootModelPropertyChanged;

            this.PropertyChanged += HandlePlayQueueViewModelPropertyChanged;
        }

        public void Initalize()
        {
            LookupDictionary.Clear();

            PlaybackQueue = new ObservableCopyCollection<PlayQueueEntryViewModel, PlayQueueEntryModel>(rootModel.PlaybackQueue,
                (playQueueEntryModel) =>
                {
                    return new PlayQueueEntryViewModel(playQueueEntryModel);
                });

            foreach (PlayQueueEntryViewModel model in PlaybackQueue)
            {
                LookupDictionary.Add(model.RowId, model);
            }

            int currentPosition = 1;
            foreach (PlayQueueEntryViewModel model in PlaybackQueue)
            {
                model.TotalPosition = currentPosition;
                currentPosition++;
            }

            PlaybackQueue.CollectionChanged += HandlePlaybackQueueCollectionChanged;

            CurrentTrack = LibraryViewModel.Current.LookupSongById(rootModel.NowPlaying);
        }

        #region Event Handlers

        private void HandlePlayQueueViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {

        }

        void HandleSettingsViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
          
        }

        void HandleRootModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case PlayQueueModel.Properties.IsActive:
                    NotifyPropertyChanged(Properties.IsActive);
                    break;
                case PlayQueueModel.Properties.NowPlaying:
                    CurrentTrack = LibraryViewModel.Current.LookupSongById(rootModel.NowPlaying);
                    break;
                case PlayQueueModel.Properties.NextTrack:
                    NotifyPropertyChanged(Properties.NextTrack);
                    SkipPlayer.RaiseExecuteChanged();
                    break;
                case PlayQueueModel.Properties.PrevTrack:
                    NotifyPropertyChanged(Properties.PrevTrack);
                    PrevPlayer.RaiseExecuteChanged();
                    break;
                case PlayQueueModel.Properties.IsPlaying:
                    NotifyPropertyChanged(Properties.IsPlaying);
                    break;
                case PlayQueueModel.Properties.CurrentTime:
                    NotifyPropertyChanged(Properties.TimeLeft);
                    NotifyPropertyChanged(Properties.ElapsedTime);
                    NotifyPropertyChanged(Properties.PercentTime);
                    NotifyPropertyChanged(Properties.ProgressTime);
                    break;
                case PlayQueueModel.Properties.FullTime:
                    NotifyPropertyChanged(Properties.TimeLeft);
                    NotifyPropertyChanged(Properties.TotalTicks);
                    NotifyPropertyChanged(Properties.TotalTime);
                    NotifyPropertyChanged(Properties.PercentTime);
                    NotifyPropertyChanged(Properties.ProgressTime);
                    break;
                case PlayQueueModel.Properties.CurrentPlaybackQueueEntryId:
                    NotifyPropertyChanged(Properties.CurrentTrackRowId);
                    NotifyPropertyChanged(Properties.CurrentTrackPosition);
                    NotifyPropertyChanged(Properties.TracksLeft);
                    NotifyPropertyChanged(Properties.NextTrack);
                    NotifyPropertyChanged(Properties.PrevTrack);
                    ClearPlayed.RaiseExecuteChanged();
                    break;
                case PlayQueueModel.Properties.IsLoop:
                    NotifyPropertyChanged(Properties.IsLoop);
                    break;
                case PlayQueueModel.Properties.VolumeValue:
                    NotifyPropertyChanged(Properties.VolumeValue);
                    break;
            }
        }

        int oldIndex;
        private void HandlePlaybackQueueCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!PlaybackQueue.IsRootChanging)
            {
                if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    oldIndex = e.OldStartingIndex;
                }
                else if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    int newIndex = e.NewStartingIndex;

                    PlaybackQueue.IsEndChanging = true;

                    rootModel.MoveSong(oldIndex, newIndex);

                    PlaybackQueue.IsEndChanging = false;
                }
            }
            else
            {
                if (e.Action == NotifyCollectionChangedAction.Remove ||
                    e.Action == NotifyCollectionChangedAction.Replace)
                {
                    foreach (PlayQueueEntryViewModel model in e.OldItems)
                    {
                        LookupDictionary.Remove(model.RowId);
                    }
                }
                if (e.Action == NotifyCollectionChangedAction.Add ||
                    e.Action == NotifyCollectionChangedAction.Replace)
                {
                    foreach (PlayQueueEntryViewModel model in e.NewItems)
                    {
                        LookupDictionary.Add(model.RowId, model);
                    }
                }
                if (e.Action == NotifyCollectionChangedAction.Reset)
                {
                    LookupDictionary.Clear();
                }

                PlayPausePlayer.RaiseExecuteChanged();
                ShuffleRemaining.RaiseExecuteChanged();
                NotifyPropertyChanged(Properties.IsEmpty);
            }

            int currentPosition = 1;
            foreach (PlayQueueEntryViewModel model in PlaybackQueue)
            {
                model.TotalPosition = currentPosition;
                currentPosition++;
            }
            NotifyPropertyChanged(Properties.NextTrack);
            NotifyPropertyChanged(Properties.PrevTrack);
            NotifyPropertyChanged(Properties.TracksLeft);
            ClearPlayed.RaiseExecuteChanged();
            ClearAll.RaiseExecuteChanged();
        }

        #endregion

        #region Properties

        public String TotalTime
        {
            get
            {
                return rootModel.FullTime.ToString(@"%m\:ss");
            }
        }

        public long TotalTicks
        {
            get
            {
                return rootModel.FullTime.Ticks;
            }
        }

        public String ElapsedTime
        {
            get
            {
                return rootModel.CurrentTime.ToString(@"%m\:ss");
            }
        }

        public String ProgressTime
        {
            get
            {
                return ElapsedTime + @"/" + TotalTime;
            }
        }

        public String TimeLeft
        {
            get
            {
                return (rootModel.CurrentTime - rootModel.FullTime).ToString(@"\-%m\:ss");
            }
        }

        public double PercentTime
        {
            get
            {
                if (rootModel.FullTime.TotalSeconds == 0)
                {
                    return 0.0;
                }
                else
                {
                    return rootModel.CurrentTime.TotalSeconds / rootModel.FullTime.TotalSeconds;
                }
            }
        }

        private SongViewModel _currentTrack;
        public SongViewModel CurrentTrack
        {
            get
            {
                return _currentTrack;
            }
            private set
            {
                if (_currentTrack != value)
                {
                    _currentTrack = value;
                    NotifyPropertyChanged(Properties.CurrentTrack);
                    NotifyPropertyChanged(Properties.IsActive);
                }
            }
        }

        public SongViewModel NextTrack
        {
            get
            {
                // CurrentTrackPosition is a 1-indexed so don't have to + 1
                if (CurrentTrackPosition >= PlaybackQueue.Count) return null;

                return PlaybackQueue[CurrentTrackPosition].Song;
            }
        }

        public SongViewModel PrevTrack
        {
            get
            {
                // CurrentTrackPosition is a 1-indexed, so need to look in array position -2
                if (CurrentTrackPosition <= 1) return null;

                return PlaybackQueue[CurrentTrackPosition - 2].Song;
            }
        }

        public int CurrentTrackPosition
        {
            get
            {
                if (rootModel.CurrentPlaybackQueueEntryId == 0) return 0;

                PlayQueueEntryViewModel activeRow = null;

                DebugHelper.Assert(new CallerInfo(), LookupDictionary.TryGetValue(rootModel.CurrentPlaybackQueueEntryId, out activeRow));

                if (activeRow == null) return 0;

                return activeRow.TotalPosition;
            }
        }

        public int TracksLeft
        {
            get
            {
                return PlaybackQueue.Count - CurrentTrackPosition;
            }
        }


        public int CurrentTrackRowId
        {
            get
            {
                return rootModel.CurrentPlaybackQueueEntryId;
            }
        }

        private ObservableCopyCollection<PlayQueueEntryViewModel, PlayQueueEntryModel> _playbackQueue;
        public ObservableCopyCollection<PlayQueueEntryViewModel, PlayQueueEntryModel> PlaybackQueue
        {
            get
            {
                return _playbackQueue;
            }
            private set
            {
                if (_playbackQueue != value)
                {
                    _playbackQueue = value;
                    NotifyPropertyChanged(Properties.PlaybackQueue);
                }
            }
        }

        public bool IsPlaying
        {
            get
            {
                return rootModel.IsPlaying;
            }
        }

        public bool IsActive
        {
            get
            {
                return CurrentTrack != null;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return PlaybackQueue.Count == 0;
            }
        }

        public bool IsLoop
        {
            get
            {
                return rootModel.IsLoop;
            }
        }

        public double VolumeValue
        {
            get
            {
                return rootModel.VolumeValue;
            }
        }

        #endregion

        #region Commands

        private RelayCommand _skipPlayer;
        public RelayCommand SkipPlayer
        {
            get
            {
                if (_skipPlayer == null) _skipPlayer = new RelayCommand(CanExecuteSkipPlayer, ExecuteSkipPlayer);

                return _skipPlayer;
            }
        }

        private void ExecuteSkipPlayer(object parameter)
        {
            this.rootModel.Skip();
        }

        private bool CanExecuteSkipPlayer(object parameter)
        {
            return (rootModel.NextTrack > 0);
        }

        private RelayCommand _prevPlayer;
        public RelayCommand PrevPlayer
        {
            get
            {
                if (_prevPlayer == null) _prevPlayer = new RelayCommand(CanExecutePrevPlayer, ExecutePrevPlayer);

                return _prevPlayer;
            }
        }

        private void ExecutePrevPlayer(object parameter)
        {
            this.rootModel.GoBack();
        }

        private bool CanExecutePrevPlayer(object parameter)
        {
            return (rootModel.PrevTrack > 0);
        }

        private RelayCommand _playPausePlayer;
        public RelayCommand PlayPausePlayer
        {
            get
            {
                if (_playPausePlayer == null) _playPausePlayer = new RelayCommand(CanExecutePlayPausePlayer, ExecutePlayPausePlayer);

                return _playPausePlayer;
            }
        }

        public void ExecutePlayPausePlayer(object parameter)
        {
            this.rootModel.PlayPause();
        }

        private bool CanExecutePlayPausePlayer(object parameter)
        {
            return PlaybackQueue.Count > 0;
        }

        private RelayCommand _shuffleRemaining;
        public RelayCommand ShuffleRemaining
        {
            get
            {
                if (_shuffleRemaining == null) _shuffleRemaining = new RelayCommand(CanExecuteShuffleRemaining, ExecuteShuffleRemaining);

                return _shuffleRemaining;
            }
        }

        private void ExecuteShuffleRemaining(object parameter)
        {
            for (int i = CurrentTrackPosition; i < PlaybackQueue.Count - 1; i++)
            {
                int swapCell = seed.Next(i, PlaybackQueue.Count);

                rootModel.MoveSong(swapCell, i);
            }
        }

        private bool CanExecuteShuffleRemaining(object parameter)
        {
            return PlaybackQueue.Count > 0;
        }

        private RelayCommand _clearPlayed;
        public RelayCommand ClearPlayed
        {
            get
            {
                if (_clearPlayed == null) _clearPlayed = new RelayCommand(CanExecuteClearPlayed, ExecuteClearPlayed);

                return _clearPlayed;
            }
        }

        private void ExecuteClearPlayed(object parameter)
        {
            while (CurrentTrackPosition > 1)
            {
                rootModel.RemoveEntry(PlaybackQueue[0].RowId);
            }
        }

        private bool CanExecuteClearPlayed(object parameter)
        {
            return CurrentTrackPosition > 1;
        }

        private RelayCommand _clearAll;
        public RelayCommand ClearAll
        {
            get
            {
                if (_clearAll == null) _clearAll = new RelayCommand(CanExecuteClearAll, ExecuteClearAll);

                return _clearAll;
            }
        }

        private void ExecuteClearAll(object parameter)
        {
            rootModel.ClearQueue();
        }

        private bool CanExecuteClearAll(object parameter)
        {
            return PlaybackQueue.Count > 0;
        }

        private RelayCommand _playModeSwitch;
        public RelayCommand PlayModeSwitch
        {
            get
            {
                if (_playModeSwitch == null) _playModeSwitch = new RelayCommand(CanExecutePlayModeSwitch, ExecutePlayModeSwitch);
                return _playModeSwitch;
            }
        }

        private void ExecutePlayModeSwitch(object parameter)
        {
            rootModel.PlayModeSwitch();
        }

        private bool CanExecutePlayModeSwitch(object parameter)
        {
            return PlaybackQueue.Count > 0;
        }


        
        #endregion

        #region Methods

        public void PlaySong(SongViewModel s)
        {
            rootModel.PlaySong(s.SongId);
        }

        public void QueueSong(SongViewModel s)
        {
            rootModel.QueueSong(s.SongId);
        }

        public void PlaySongList(IList<SongViewModel> Songs, bool startPlay, int cap = 0)
        {
            PlaySongList<SongViewModel>(Songs, (song) => { return song; }, startPlay, cap);
        }

        public void PlaySongList<T>(IList<T> Songs, Func<T, SongViewModel> ItemToSong, bool startPlay, int cap = 0)
        {
            int actualCap = cap;

            if (cap == 0)
            {
                actualCap = PlayQueueModel.MAX_QUEUE_SIZE;
            }

            actualCap = Math.Min(actualCap, PlayQueueModel.MAX_QUEUE_SIZE);

            if (Songs.Count > 0)
            {
                if (startPlay)
                {
                    this.PlaySong(ItemToSong(Songs[0]));
                }
                else
                {
                    this.QueueSong(ItemToSong(Songs[0]));
                }

                for (int i = 1; i < Songs.Count && i < actualCap; i++)
                {
                    this.QueueSong(ItemToSong(Songs[i]));
                }
            }
        }

        public void ShuffleSongList(IList<SongViewModel> Songs, bool startPlay, int cap = 0)
        {
            ShuffleSongList(Songs, (song) => { return song; }, startPlay, cap);
        }

        public void ShuffleSongList<T>(IList<T> Songs, Func<T, SongViewModel> ItemToSong, bool startPlay, int cap = 0)
        {
            int actualCap = cap;

            if (cap == 0)
            {
                actualCap = PlayQueueModel.MAX_QUEUE_SIZE;
            }

            actualCap = Math.Min(actualCap, PlayQueueModel.MAX_QUEUE_SIZE);

            List<T> copy = Songs.ToList<T>();

            for (int n = copy.Count - 1; n >= 0 && (copy.Count - 1 - n) < actualCap; n--)
            {
                int next = seed.Next(n);

                if (n == copy.Count - 1 && startPlay)
                {
                    this.PlaySong(ItemToSong(copy[next]));
                }
                else
                {
                    this.QueueSong(ItemToSong(copy[next]));
                }
                copy[next] = copy[n];
            }
        }

        internal void RemoveSong(PlayQueueEntryViewModel removedSong)
        {
            rootModel.RemoveEntry(removedSong.RowId);
        }

        internal void Clear()
        {
            rootModel.ClearQueue();
        }

        internal void Suspend()
        {
            rootModel.Suspend();
        }

        internal void Resume()
        {
            rootModel.Resume();
        }

        internal void PlayFromSong(PlayQueueEntryViewModel playQueueEntryViewModel)
        {
            rootModel.PlayFromSong(playQueueEntryViewModel.RowId);
        }

        internal PlayQueueEntryViewModel LookupPlayqueueEntry(int rowId)
        {
            if (LookupDictionary.ContainsKey(rowId))
            {
                return LookupDictionary[rowId];
            }

            return null;
        }

        internal void RemoveAllInstancesOfSong(SongViewModel song)
        {
            rootModel.RemoveSong(song.SongId);
        }

        internal void ScrubToPercentage(double percentage)
        {
            rootModel.ScrubToPercentage(percentage);
        }


        internal void SetVolume(double volumValue)
        {
            rootModel.SetVolume(volumValue);
        }

        #endregion
    }
}
