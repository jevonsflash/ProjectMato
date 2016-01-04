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
    public sealed partial class LrcListPage : BasePage
    {
        private LrcListPageViewModel lrcListPageViewModel = new LrcListPageViewModel();
        public LrcListPage()
        {
            this.InitializeComponent();
            this.DataContext = lrcListPageViewModel;
        }


        private async void BTNCleanUnavailableLrc_Click(object sender, RoutedEventArgs e)
        {
            int delCount = await lrcListPageViewModel.CleanUnavailableLrc();
            JMessBox jb = new JMessBox(string.Format("完成，已清理{0}个文件", delCount.ToString()));
            jb.Show();
        }

        private void BTNDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = ((sender as Button).Parent as Grid).DataContext;
            lrcListPageViewModel.DeleteLrcFile(item);
            this.TBLrcCount.Text = lrcListPageViewModel.LrcList.Count.ToString();
        }
    }

}
