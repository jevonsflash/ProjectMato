using MusicMink.ViewModels;
using MusicMinkAppLayer.Helpers;
using MusicMinkAppLayer.Models;
using System;
using System.Collections.Generic;
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
    public sealed partial class LrcPage : BasePage
    {
        private Gecime_Lyric list = new Gecime_Lyric();

        private string songName = string.Empty;
        private string artistName = string.Empty;

        public LrcPage()
        {
            this.InitializeComponent();
            this.DataContext = LibraryViewModel.Current.PlayQueue;
            Loaded += LrcPage_Loaded;
        }

        private void LrcPage_Loaded(object sender, RoutedEventArgs e)
        {
            songName = this.TBSongName.Text;
            artistName = this.TBArtistName.Text;
        }
        public void DoHttpWebRequest(string keyWord)
        {
            HttpHelper ht = new HttpHelper();
            string url = "http://geci.me/api/lyric/" + keyWord;
            ht.CreatePostHttpResponse(url);
            ht.FileWatchEvent += ht_FileWatchEvent;
        }

        async private void ht_FileWatchEvent(object sender, CompleteEventArgs e)
        {
            list = LRCSer.GecimeLyricDeserializer(e.Node);
            if (list.Result.Count() == 0)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    JMessBox jb = new JMessBox("没有结果");
                    jb.Show();
                });
            }
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                LBResult.DataContext = list.Result;
            });

        }
        private void BTNSearch_Click(object sender, RoutedEventArgs e)
        {
            songName = this.TBSongName.Text;
            artistName = this.TBArtistName.Text;
            if (!string.IsNullOrEmpty(songName))
            {
                DoHttpWebRequest(songName);

            }
            else
            {
                JMessBox jb = new JMessBox("至少填写歌曲名称");
                jb.Show();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string lrcStr = (sender as Button).DataContext.ToString();
            HttpHelper ht = new HttpHelper();
            ht.CreatePostHttpResponse(lrcStr);
            ht.FileWatchEvent += ht_FileWatchEvent2;
        }

        void ht_FileWatchEvent2(object sender, CompleteEventArgs e)
        {
            string lrcStr = e.Node;
            GetCompleted(lrcStr);
        }


        /// <summary>
        /// 替换已存在的歌词文件
        /// </summary>
        /// <param name="content">歌词内容</param>
        private async void GetCompleted(string content)
        {
            string fileName = "MatoLrc\\" + songName + "-" + artistName + ".lrc";
            await FileHelper.DeleteFileAsync(fileName);
            if (await FileHelper.CreateAndWriteFileAsync(fileName, content))
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    JMessBox jb = new JMessBox(string.Format("已更新 {0}", songName + "-" + artistName + ".lrc"));
                    jb.Show();
                });
            }
            else
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    JMessBox jb = new JMessBox("更新失败");
                    jb.Show();
                });
            }
        }

        private void BTNCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationHelper.GoBack(null);
        }
    }
}
