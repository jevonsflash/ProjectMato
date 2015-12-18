﻿using MusicMink.ViewModels;
using MusicMinkAppLayer.Helpers;
using MusicMinkAppLayer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “用户控件”项模板在 http://go.microsoft.com/fwlink/?LinkId=234236 上有介绍

namespace MusicMink.Controls
{
    public sealed partial class TestControl : UserControl
    {
        private LrcControlViewModel lrcViewModel = new LrcControlViewModel();
        private List<Result2> list;

        public TestControl()
        {
            this.InitializeComponent();
        }


        public string test
        {
            get { return (string)GetValue(testProperty); }
            set { SetValue(testProperty, value); }
        }

        // Using a DependencyProperty as the backing store for test.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty testProperty =
            DependencyProperty.Register("test", typeof(string), typeof(TestControl), new PropertyMetadata("", OnChanged));




        public string artist
        {
            get { return (string)GetValue(artistProperty); }
            set { SetValue(artistProperty, value); }
        }

        // Using a DependencyProperty as the backing store for artist.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty artistProperty =
            DependencyProperty.Register("artist", typeof(string), typeof(TestControl), new PropertyMetadata("", OnChanged));


        private static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TestControl testControl = d as TestControl;
            testControl.TBSongName.DataContext = testControl.test;
            testControl.TBArtistName.DataContext = testControl.artist;
            testControl.InitializeLrc(testControl.test + "-" + testControl.artist + ".lrc");

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
            string fileName = test + "-" + artist + ".lrc";
            if (await FileHelper.IsExistFileAsync("/" + fileName))
            {
                if (await FileHelper.CreateAndWriteFileAsync("/" + fileName, lrcStr))
                {
                    LRCItem lrcItem = LRCSer.InitLrc(lrcStr);
                    lrcViewModel.LrcData = lrcItem;
                }

            }

        }


        #endregion
        /// <summary>
        /// 初始化
        /// </summary>
        public async void InitializeLrc(string fileName)
        {
            string lrcStr;

            lrcStr = await ReadLrcFile(fileName);
            if (!string.IsNullOrEmpty(lrcStr))
            {
                LRCItem lrcItem = LRCSer.InitLrc(lrcStr);
                lrcViewModel.LrcData = lrcItem;
            }
            else
            {
                if (true)
                {
                    DoHttpWebRequest(test);
                }
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
            if (test == null)
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
                    gd1.Foreground = new SolidColorBrush(Colors.Gray);
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
        private void BTNLrcManage_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.Current.Navigate(NavigationLocation.LrcListPage);

        }

        private void BTNLrcSearch_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.Current.Navigate(NavigationLocation.SearchLrcPage);

        }

    }
}