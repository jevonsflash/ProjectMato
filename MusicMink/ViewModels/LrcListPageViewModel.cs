using MusicMink.Common;
using MusicMinkAppLayer.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicMink.ViewModels
{
    public class LrcListPageViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;


        public LrcListPageViewModel()
        {
            InitializeLrcList();
        }
        public void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        private RelayCommand deleteLrcFileCommand;
        public RelayCommand DeleteLrcFileCommand
        {
            get
            {
                if (deleteLrcFileCommand == null) deleteLrcFileCommand = new RelayCommand(CanExecute, DeleteLrcFile);
                return deleteLrcFileCommand;
            }
        }


        private List<string> lrcList;

        public List<string> LrcList
        {
            get { return lrcList; }
            set
            {
                lrcList = value;
                NotifyPropertyChanged("LrcList");
            }
        }

        private string lrcCount;

        public string LrcCount
        {
            get { return lrcCount; }
            set
            {
                lrcCount = value;
                NotifyPropertyChanged("LrcCount");
            }
        }

        public async void DeleteLrcFile(object item)
        {
            await FileHelper.DeleteFileAsync("MatoLrc\\" + item as string);
            InitializeLrcList();
        }
        public async Task<int> CleanUnavailableLrc()
        {
            int delCount = 0;
            List<string> LrcListTemp = await FileHelper.GetFiles("MatoLrc");

            foreach (string strItem in LrcListTemp)
            {
                string fileNameWithoutExtention = strItem.Split('.')[0];
                string songName = fileNameWithoutExtention.Split('-')[0];
                string artistName = fileNameWithoutExtention.Split('-')[1];
                SongViewModel song = LibraryViewModel.Current.LookupSongByName(songName, artistName);
                if (song == null)
                {
                    await FileHelper.DeleteFileAsync("MatoLrc\\" + strItem);
                    delCount++;
                }
            }
            return delCount;
        }

        private async void InitializeLrcList()
        {
            LrcList = await FileHelper.GetFiles("MatoLrc");
            lrcCount = LrcList.Count().ToString();
            System.Diagnostics.Debug.WriteLine(LrcCount);
        }
        private bool CanExecute(object parameter)
        {
            return true;
        }

    }
}
