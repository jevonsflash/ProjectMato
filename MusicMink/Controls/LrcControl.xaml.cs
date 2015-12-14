using MusicMinkAppLayer.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MusicMinkAppLayer.Models;
using MusicMink.Common;
using System.ComponentModel;
using Windows.UI;
using System.Threading.Tasks;

// “用户控件”项模板在 http://go.microsoft.com/fwlink/?LinkId=234236 上有介绍

namespace MusicMink.Controls
{
    public sealed partial class LrcControl : UserControl, INotifyPropertyChanged
    {
        private List<Result2> list;

        public LrcControl()
        {
            this.InitializeComponent();
            //this.InitializeLrc();
            this.DataContext = LrcData;
            //this.TBArtistName.DataContext = this.ArtistName;
            //this.TBSongName.DataContext = this.SongName;
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        private void BTNLrcManage_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.Current.Navigate(NavigationLocation.LrcListPage);

        }

        private void BTNLrcSearch_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.Current.Navigate(NavigationLocation.SearchLrcPage);

        }

        private LRCItem lrcData;

        public LRCItem LrcData
        {
            get { return lrcData; }
            set
            {
                lrcData = value;
                NotifyPropertyChanged("LrcData");
            }
        }
        //private string songName;

        //public string SongName
        //{
        //    get { return songName; }
        //    set { songName = value; }
        //}

        //private string artistName;

        //public string ArtistName
        //{
        //    get { return artistName; }
        //    set { artistName = value; }
        //}

        /// <summary>
        /// 歌词高亮Brush
        /// </summary>
        public Brush EmphasisBrush
        {
            get { return (Brush)GetValue(EmphasisBrushProperty); }
            set { SetValue(EmphasisBrushProperty, value); }
        }

        public static readonly DependencyProperty EmphasisBrushProperty =
            DependencyProperty.Register("EmphasisBrush", typeof(Brush), typeof(LrcControl), new PropertyMetadata(Colors.Gray));


        /// <summary>
        /// 时间轴
        /// </summary>
        public string TimeLine
        {
            get { return (string)GetValue(TimeLineProperty); }
            set { SetValue(TimeLineProperty, value); }
        }

        public static readonly DependencyProperty TimeLineProperty =
            DependencyProperty.Register("TimeLine", typeof(string), typeof(LrcControl), new PropertyMetadata("", new PropertyChangedCallback(OnTimeLinePropertyChanged)));
        public string Music
        {
            get { return (string)GetValue(MusicProperty); }
            set { SetValue(MusicProperty, value); }
        }

        public static readonly DependencyProperty MusicProperty =
            DependencyProperty.Register("Music", typeof(string), typeof(LrcControl), new PropertyMetadata("", new PropertyChangedCallback(OnMusicPropertyChanged)));

        public bool CanScroll
        {
            get { return (bool)GetValue(CanScrollProperty); }
            set { SetValue(CanScrollProperty, value); }
        }

        public static readonly DependencyProperty CanScrollProperty =
            DependencyProperty.Register("CanScroll", typeof(bool), typeof(LrcControl), new PropertyMetadata(false));


        public string Artist
        {
            get { return (string)GetValue(ArtistProperty); }
            set { SetValue(ArtistProperty, value); }
        }

        public static readonly DependencyProperty ArtistProperty =
            DependencyProperty.Register("Artist", typeof(string), typeof(LrcControl), new PropertyMetadata("", new PropertyChangedCallback(OnMusicPropertyChanged)));

        private async static void OnMusicPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            LrcControl lrcControl = sender as LrcControl;
            if (lrcControl != null)
            {
                await Task.Run(() =>
                {
                    if (!string.IsNullOrEmpty(lrcControl.Music) && !string.IsNullOrEmpty(lrcControl.Artist))
                    {

                        lrcControl.InitializeLrc();
                    }
                });


                //lrcControl.ArtistName = lrcControl.Artist;
                //lrcControl.SongName = lrcControl.Music;
            }
        }

        private static void OnTimeLinePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            LrcControl lrcControl = sender as LrcControl;
            if (lrcControl != null)
            {
                var item = lrcControl.LrcData.LrcWord.FirstOrDefault(c => c.Key < TimeSpan.Parse(lrcControl.TimeLine).TotalSeconds);
                int currentIndex = lrcControl.lrcData.LrcWord.ToList().IndexOf(item);
                lrcControl.CleanBrush();
                lrcControl.SetBrush(currentIndex);
                if (lrcControl.CanScroll)
                {
                    lrcControl.SetScroll(currentIndex);
                }

            }
        }

        #region 请求歌词算法
        /// <summary>
        /// 发送歌词请求
        /// </summary>
        public void DoHttpWebRequest(string keyWord)
        {
            HttpHelper ht = new HttpHelper();
            string url = "http://geci.me/api/lyric/" + keyWord;
            ht.CreatePostHttpResponse(url);
            ht.FileWatchEvent += ht_FileWatchEvent;
        }
        /// <summary>
        /// 处理返回值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ht_FileWatchEvent(object sender, CompleteEventArgs e)
        {
            list = LRCSer.GecimeLyricDeserializer(e.Node).result.ToList();
            if (list.Count == 0)
            {
                DisplayErr("暂无内容");
            }
            else
            {

                Result2 result2 = list.First();
                HttpHelper ht = new HttpHelper();
                ht.CreatePostHttpResponse(result2.lrc);
                ht.FileWatchEvent += ht_FileWatchEvent2;
            }
        }

        private void DisplayErr(string v)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 处理返回值2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void ht_FileWatchEvent2(object sender, CompleteEventArgs e)
        {
            string lrcStr = e.Node;
            string fileName = Music + "-" + Artist + ".lrc";
            if (await FileHelper.IsExistFileAsync("/" + fileName))
            {
                if (await FileHelper.CreateAndWriteFileAsync("/" + fileName, lrcStr))
                {
                    LRCItem lrcItem = LRCSer.InitLrc(lrcStr);
                    LrcData = lrcItem;

                }

            }

        }


        #endregion
        /// <summary>
        /// 初始化
        /// </summary>
        public async void InitializeLrc()
        {
            string lrcStr;
            string fileName = Music + "-" + Artist + ".lrc";
            lrcStr = await ReadLrcFile(fileName);
            if (!string.IsNullOrEmpty(lrcStr))
            {
                LRCItem lrcItem = LRCSer.InitLrc(lrcStr);
                LrcData = lrcItem;
            }
            else
            {
                DoHttpWebRequest(Music);
            }
        }

        private static async System.Threading.Tasks.Task<string> ReadLrcFile(string fileName)
        {
            string lrcStr = string.Empty;
            if (await FileHelper.IsExistFileAsync("/" + fileName))
            {
                lrcStr = await FileHelper.ReadTxtFileAsync("/" + fileName);
            }
            return lrcStr;
        }

        private void UpdateMusic()
        {
            if (Music == null)
            {
                LBLyric.DataContext = null;
                return;
            }
        }

        #region 工具方法
        private T FindFirstElementInVisualTree<T>(DependencyObject parentElement) where T : DependencyObject
        {
            var count = VisualTreeHelper.GetChildrenCount(parentElement);
            if (count == 0)
                return null;
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parentElement, i);
                if (child != null && child is T)
                {
                    return (T)child;
                }
                else
                {
                    var result = FindFirstElementInVisualTree<T>(child);
                    if (result != null)
                        return result;
                }
            }
            return null;
        }

        /// <summary>
        /// 清除歌词画刷
        /// </summary>
        private void CleanBrush()
        {
            for (int itemIndex = 0; itemIndex < this.LBLyric.Items.Count; itemIndex++)
            {
                ListBoxItem lbiPre = (ListBoxItem)this.LBLyric.ContainerFromIndex(itemIndex);
                if (lbiPre != null)
                {
                    TextBlock gd1 = FindFirstElementInVisualTree<TextBlock>(lbiPre);
                    gd1.Foreground = EmphasisBrush;
                    gd1.FontSize = 25;
                }
            }
        }

        /// <summary>
        /// 设置歌词画刷
        /// </summary>
        /// <param name="index">设置行号</param>
        private void SetBrush(int index)
        {
            ListBoxItem lbi = (ListBoxItem)this.LBLyric.ContainerFromIndex(index);
            if (lbi != null)
            {
                TextBlock gd1 = FindFirstElementInVisualTree<TextBlock>(lbi);
                if (gd1 != null)
                {
                    gd1.Foreground = new SolidColorBrush(Colors.White);
                    gd1.FontSize = 30;
                }
            }

        }

        /// <summary>
        /// 设置滚动条偏移量
        /// </summary>
        private void SetScroll(int index)
        {
            ScrollViewer sv = FindFirstElementInVisualTree<ScrollViewer>(this.LBLyric);
            if (sv != null)
            {
                int lrcOffset = index - 4;
                if (lrcOffset > 0)
                {
                    sv.ChangeView(null, lrcOffset, null);
                }
                else
                {
                    sv.ChangeView(null, 0, null);
                }
            }
        }
        #endregion

        private void BTNCollapse_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }
    }
}
