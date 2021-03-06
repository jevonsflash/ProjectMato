﻿using MusicMink.Collections;
using MusicMink.Common;
using MusicMinkAppLayer.Diagnostics;
using MusicMinkAppLayer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using Windows.Globalization.Collation;

namespace MusicMink.ViewModels
{
    public class ArtistViewModel : BaseViewModel<ArtistModel>, IComparable
    {
        public static class Properties
        {
            public const string ArtistId = "ArtistId";

            public const string Albums = "Albums";
            public const string ContentInfo = "ContentInfo";
            public const string ContentInfoSongs = "ContentInfoSongs";
            public const string ContentInfoAlbums = "ContentInfoAlbums";
            public const string IsSongsEmpty = "IsSongsEmpty";
            public const string IsAlbumsEmpty = "IsAlbumsEmpty";
            public const string Name = "Name";
            public const string SortName = "SortName";
            public const string Songs = "Songs";

            public const string Picture = "Picture";

            public const string IsBeingDeleted = "IsBeingDeleted";
        }

        public ArtistViewModel(ArtistModel newArtist) : base(newArtist)
        {
            Songs.CollectionChanged += HandleSongsCollectionChanged;
            Albums.CollectionChanged += HandleAlbumsCollectionChanged;

            rootModel.PropertyChanged += HandleArtistModelPropertyChanged;
        }

        #region Event Handlers

        private void HandleArtistModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case ArtistModel.Properties.ArtistId:
                    DebugHelper.Alert(new CallerInfo(), "ArtistId Probably Shouldn't Change...");
                    NotifyPropertyChanged(Properties.ArtistId);
                    break;
                case ArtistModel.Properties.Name:
                    _sortName = null;
                    NotifyPropertyChanged(Properties.SortName);
                    NotifyPropertyChanged(Properties.Name);
                    break;
            }
        }

        private void HandleSongsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (LibraryViewModel.Current.LibraryLoaded)
            {
                NotifyPropertyChanged(Properties.ContentInfo);
                NotifyPropertyChanged(Properties.ContentInfoSongs);
                NotifyPropertyChanged(Properties.IsSongsEmpty);
            }
        }

        private void HandleAlbumsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (LibraryViewModel.Current.LibraryLoaded)
            {
                NotifyPropertyChanged(Properties.ContentInfo);
                NotifyPropertyChanged(Properties.ContentInfoAlbums);
                NotifyPropertyChanged(Properties.IsAlbumsEmpty);
            }
        }

        #endregion

        #region Properties

        public int ArtistId
        {
            get
            {
                return rootModel.ArtistId;
            }
        }

        private bool _isBeingDeleted = false;
        public bool IsBeingDeleted
        {
            get
            {
                return _isBeingDeleted;
            }
            set
            {
                if (_isBeingDeleted != value)
                {
                    _isBeingDeleted = value;
                    NotifyPropertyChanged(Properties.IsBeingDeleted);
                }
            }
        }

        private SortedList<SongViewModel> _songs = new SortedList<SongViewModel>(new SongSortGenericOrder(SongViewModel.Properties.SortName, true));
        public ObservableCollection<SongViewModel> Songs
        {
            get
            {
                return _songs;
            }
        }

        private ObservableCollection<AlbumViewModel> _albums = new ObservableCollection<AlbumViewModel>();
        public ObservableCollection<AlbumViewModel> Albums
        {
            get
            {
                return _albums;
            }
        }

        public string ContentInfo
        {
            get
            {
                if (Songs.Count == 0)
                {
                    if (Albums.Count == 0)
                    {
                        return Strings.GetResource("FormatNone");
                    }
                    else
                    {
                        return Strings.HandlePlural(Albums.Count, Strings.GetResource("FormatAlbumsPlural"), Strings.GetResource("FormatAlbumsSingular"));
                    }
                }
                else
                {
                    if (Albums.Count == 0)
                    {
                        return Strings.HandlePlural(Songs.Count, Strings.GetResource("FormatSongsPlural"), Strings.GetResource("FormatSongsSingular"));
                    }
                    else
                    {
                        return Strings.HandlePlural(Songs.Count, Strings.GetResource("FormatSongsPlural"), Strings.GetResource("FormatSongsSingular")) +
                            ", " + Strings.HandlePlural(Albums.Count, Strings.GetResource("FormatAlbumsPlural"), Strings.GetResource("FormatAlbumsSingular"));
                    }
                }
            }
        }

        public string ContentInfoAlbums
        {
            get
            {
                return Strings.HandlePlural(Albums.Count, Strings.GetResource("FormatAlbumsPlural"), Strings.GetResource("FormatAlbumsSingular"));
            }
        }

        public string ContentInfoSongs
        {
            get
            {
                return Strings.HandlePlural(Songs.Count, Strings.GetResource("FormatSongsPlural"), Strings.GetResource("FormatSongsSingular"));
            }
        }

        public bool IsAlbumsEmpty
        {
            get
            {
                return (Albums.Count == 0);
            }
        }

        public bool IsSongsEmpty
        {
            get
            {
                return (Songs.Count == 0);
            }
        }

        public string Name
        {
            get
            {
                if (rootModel.Name == string.Empty) return Strings.GetResource("UnknownArtistString");
                return rootModel.Name;
            }
        }

        private string _sortName;
        public string SortName
        {
            get
            {
                if (_sortName == null)
                {
                    CultureInfo ci = CultureInfo.CurrentUICulture;
                    CharacterGroupings slg = new CharacterGroupings();
                    List<string> list = HeaderCharacter.CreateDefaultGroups(slg);
                    _sortName = slg.Lookup(Name);
                }

                return _sortName;
            }
        }

        // TODO: Actually get real picture with uri and stuff
        public string Picture
        {
            get
            {
                return null;
            }
        }

        #endregion

        #region Commands

        private RelayCommand _playAllSongs;
        public RelayCommand PlayAllSongs
        {
            get
            {
                if (_playAllSongs == null) _playAllSongs = new RelayCommand(CanExecutePlayAllSongs, ExecutePlayAllSongs);

                return _playAllSongs;
            }
        }

        private void ExecutePlayAllSongs(object parameter)
        {
            LibraryViewModel.Current.PlayQueue.ShuffleSongList(Songs, true);
        }

        private bool CanExecutePlayAllSongs(object parameter)
        {
            return true;
        }


        private RelayCommand _queueAllSongs;
        public RelayCommand QueueAllSongs
        {
            get
            {
                if (_queueAllSongs == null) _queueAllSongs = new RelayCommand(CanExecuteQueueAllSongs, ExecuteQueueAllSongs);

                return _queueAllSongs;
            }
        }

        private void ExecuteQueueAllSongs(object parameter)
        {
            LibraryViewModel.Current.PlayQueue.ShuffleSongList(Songs, false);
        }

        private bool CanExecuteQueueAllSongs(object parameter)
        {
            return true;
        }

        #endregion

        #region IComparable

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            ArtistViewModel otherArtist = obj as ArtistViewModel;
            if (otherArtist != null)
            {
                return this.SortName.CompareTo(otherArtist.SortName);
            }
            else
            {
                throw new ArgumentException("Object is not a artistViewModel");
            }
        }

        #endregion
    }
}
