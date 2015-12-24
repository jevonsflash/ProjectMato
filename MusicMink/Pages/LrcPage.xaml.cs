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
        private List<Result2ForShow> result = new List<Result2ForShow>();
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
            result.Clear();
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                LBResult.DataContext = null;
            });

            list = LRCSer.GecimeLyricDeserializer(e.Node);
            if (list.Result.Count() == 0)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    JMessBox jb = new JMessBox("没有结果");
                    jb.Show();
                });
            }
            else
            {
                foreach (var item in list.Result)
                {
                    //执行对每个artistid的查询
                    DoHttpWebRequestArtist(item.artist_id);
                }
            }
        }

        public void DoHttpWebRequestArtist(int artistId)
        {
            HttpHelper ht = new HttpHelper();
            string url = "http://geci.me/api/artist/" + artistId.ToString();
            ht.CreatePostHttpResponse(url);
            ht.FileWatchEvent += ht_FileWatchEvent3;
        }

        async void ht_FileWatchEvent3(object sender, CompleteEventArgs e)
        {
            //序列化作者列表
            var artistModel = LRCSer.GecimeArtistDeserializer(e.Node);

            //如果作者列表为空则返回
            if (artistModel == null)
            {
                return;
            }
            //当前作者结果对象

            var temp = list.Result.FirstOrDefault(c => c.artist_id.ToString() == e.Node2);
            if (temp != null)
            {

                //加入到查询结果表
                result.Add(new Result2ForShow
                {
                    sid = temp.sid,
                    song = temp.song,
                    lrc = temp.lrc,
                    artist = artistModel.Result.name,
                });
                //绑定结果表


                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    LBResult.DataContext = null;
                    LBResult.DataContext = result;
                });
            }
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
