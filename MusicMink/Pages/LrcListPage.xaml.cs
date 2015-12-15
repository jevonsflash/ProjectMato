using MusicMink.ViewModels;
using MusicMinkAppLayer.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Weather.JMessbox;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace MusicMink.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class LrcListPage : Page, INotifyPropertyChanged
    {
        private List<string> lrcList;

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

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

        public LrcListPage()
        {
            this.InitializeComponent();
            CustomListLB.DataContext = LrcList;
        }

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
        private async void MenuItem_Click(object sender, RoutedEventArgs e)
        {

            var item = ((sender as HyperlinkButton).Parent as ListBoxItem).Content;
            await FileHelper.DeleteFileAsync("/" + item);

        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.InitializeLrcList();
        }

        private async Task<int> CleanUnavailableLrc()
        {

            int delCount = 0;
            List<string> LrcListTemp = await FileHelper.GetFiles();

            foreach (string strItem in LrcListTemp)
            {
                string fileNameWithoutExtention = strItem.Split('.')[0];
                string songName = fileNameWithoutExtention.Split('-')[0];
                string artistName = fileNameWithoutExtention.Split('-')[1];
                SongViewModel song = LibraryViewModel.Current.LookupSongByName(songName, artistName);
                if (song == null)
                {
                    await FileHelper.DeleteFileAsync("/" + strItem);
                    delCount++;
                }
            }
            return delCount;
        }

        private async void BTNCleanUnavailableLrc_Click(object sender, RoutedEventArgs e)
        {
            int delCount = await this.CleanUnavailableLrc();
            this.InitializeLrcList();
            JMessBox jb = new JMessBox(string.Format("完成，已清理{0}个文件", delCount.ToString()));
            jb.Show();
        }
        private async void InitializeLrcList()
        {
            LrcList = await FileHelper.GetFiles();
            lrcCount = LrcList.Count().ToString();
        }
    }

}
