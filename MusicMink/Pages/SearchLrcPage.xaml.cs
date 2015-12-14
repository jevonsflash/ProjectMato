using MusicMink.ViewModels;
using MusicMinkAppLayer.Helpers;
using MusicMinkAppLayer.Models;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Weather.JMessbox;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace MusicMink.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SearchLrcPage : Page, INotifyPropertyChanged
    {
        private string songName = string.Empty;
        private string artistName = string.Empty;
        public SearchLrcPage()
        {
            InitializeComponent();
            this.DataContext = LibraryViewModel.Current.PlayQueue;

        }



        private LRCItem lrcData;
        private Gecime_Lyric list;

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string value)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(value));
            }
        }

        public LRCItem LrcData
        {
            get { return lrcData; }
            set
            {
                lrcData = value;
                NotifyPropertyChanged("LrcData");
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                songName = (e.Parameter as string[]).First();
                artistName = (e.Parameter as string[]).Last();
            }
            base.OnNavigatedTo(e);
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
        }



        #region 请求歌词算法
        /// <summary>
        /// 发送歌词请求
        /// </summary>
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
            list = LRCSer.GecimeLyricDeserializer(e.Node);
            if (list.result.Count() == 0)
            {

            }
            else
            {
                foreach (var item in list.result)
                {
                    //执行对每个artistid的查询
                    DoHttpWebRequestArtist(item.artist_id);
                }
            }
        }
        /// <summary>
        /// 处理返回值2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ht_FileWatchEvent2(object sender, CompleteEventArgs e)
        {
            string lrcStr = e.Node;
            GetCompleted(lrcStr);
        }
        #endregion


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

            var temp = list.result.FirstOrDefault(c => c.artist_id.ToString() == e.Node2);
            dynamic result = null;
            //加入到查询结果表
            result.Add(new
            {
                sid = temp.sid,
                song = temp.song,
                lrc = temp.lrc,
                artist = artistModel.name,
            });
            //绑定结果表
            await Task.Run(() =>
             {

                 this.LBResult.DataContext = null;
                 this.LBResult.DataContext = result;
             });
        }

        /// <summary>
        /// 替换已存在的歌词文件
        /// </summary>
        /// <param name="content">歌词内容</param>
        private async void GetCompleted(string content)
        {
            string lrcFileName = this.songName + "-" + this.artistName + ".lrc";
            await FileHelper.DeleteFileAsync("/" + lrcFileName);
            if (await FileHelper.CreateAndWriteFileAsync("/" + lrcFileName, content))
            {
                await Task.Run(() =>
                 {
                     JMessBox jb = new JMessBox(string.Format("已更新 {0}", lrcFileName));
                     jb.Show();
                 });
            }
            else
            {
                await Task.Run(() =>
                 {
                     JMessBox jb = new JMessBox("更新失败");
                     jb.Show();
                 });
            }
        }
        private void BTNCancel_Click(object sender, RoutedEventArgs e)
        {
            //Go Back
        }

        private void BTNSearch_Click(object sender, RoutedEventArgs e)
        {
            DoHttpWebRequest(songName);
        }

        private void BTNDownLoad_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string lrcStr = (sender as Button).DataContext.ToString();
            HttpHelper ht = new HttpHelper();
            ht.CreatePostHttpResponse(lrcStr);
            ht.FileWatchEvent += ht_FileWatchEvent2;

        }
    }

}
