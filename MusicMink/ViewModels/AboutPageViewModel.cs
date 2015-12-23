using MusicMink.Common;
using System;
using System.Collections.Generic;
using System.Text;
#if WINDOWS_PHONE_APP
using Windows.ApplicationModel.Email;
#endif
namespace MusicMink.ViewModels
{
    public class AboutPageViewModel : NotifyPropertyChangedUI
    {
        private RelayCommand goLoveCommand;
        public RelayCommand GoLoveCommand
        {
            get
            {
                if (goLoveCommand == null) goLoveCommand = new RelayCommand(CanExecute, GoLove);
                return goLoveCommand;
            }
        }

        private RelayCommand goMailCommand;
        public RelayCommand GoMailCommand
        {
            get
            {
                if (goMailCommand == null) goMailCommand = new RelayCommand(CanExecute, GoMail);
                return goMailCommand;
            }
        }

        private RelayCommand goWeiboCommand;
        public RelayCommand GoWeiboCommand
        {
            get
            {
                if (goWeiboCommand == null) goWeiboCommand = new RelayCommand(CanExecute, GoWeibo);
                return goWeiboCommand;
            }
        }


        private List<string> strUpdate;

        public List<string> StrUpdate
        {
            get { return strUpdate; }
            set
            {
                strUpdate = value;
                NotifyPropertyChanged("StrUpdate");
            }
        }
        private string version;

        public string Version
        {
            get { return version; }
            set
            {
                version = value;
                NotifyPropertyChanged("Version");
            }
        }

        private string introduction;

        public string Introduction
        {
            get { return introduction; }
            set
            {
                introduction = value;
                NotifyPropertyChanged("Introduction");
            }
        }

        private string brief;

        public string Brief
        {
            get { return brief; }
            set
            {
                brief = value;
                NotifyPropertyChanged("Brief");
            }
        }


        public AboutPageViewModel()
        {
            StringBuilder sb = new StringBuilder();
            Version = (string)App.Current.Resources["Version"];
            Brief = "番茄播放器";
            sb.Append("番茄播放器(Mato Player)是一款本地音乐播放器，设计理念是专注本地音乐体验，保持单手操作的易用性。");
            sb.Append("3.0版本带来了重大更新，底层架构重写带来新功能和更好的性能。");
            //sb.Append("对不起我们来晚了，终于支持进度条拖放了~！；控制栏让您能在任意一个页面查看歌曲信息，改变播放状态；更实用的播放列队，您可以自由排序您的列队，联合搜索歌曲，专辑或艺术家；新加入手势功能，在首页您可以通过滑动专辑图片进行上一曲/下一曲操作；更强大的歌曲管理系统，您可以更改歌曲信息，专辑封面，还有新加入的星级评价模块；我们还带来了主题功能");
            sb.Append("其他功能：联网歌词查看，歌词管理，睡眠模式等。");
            Introduction = sb.ToString();
            StrUpdate = new List<string>();
            StrUpdate.Add("1.对不起我们来晚了,终于支持进度条拖放~！");
            StrUpdate.Add("2.手势功能来了,在首页您可以通过滑动专辑图片进行上一曲/下一曲操作");
            StrUpdate.Add("3.全局歌曲控制栏,您可以在任意页面查看和控制曲目");
            StrUpdate.Add("4.搜索功能大升级,支持联合搜索");
            StrUpdate.Add("5.全新列队功能,支持自定义播放队列，歌曲自由排序");
            StrUpdate.Add("6.全新的音乐库管理逻辑,您可以自由编辑歌曲信息，管理库，添加星级评价等");
            StrUpdate.Add("7.UI调整,支持主题色随主题背景更改");



        }

        private async void GoMail(object obj)
        {
            string subject = "";
            string body = "";
            string emailAddress = "jevons@hotmail.com";

#if WINDOWS_PHONE_APP

            var emailMessage = new EmailMessage();
            emailMessage.Body = body;
            emailMessage.To.Add(new EmailRecipient(emailAddress));
            await EmailManager.ShowComposeNewEmailAsync(emailMessage);
#endif
#if WINDOWS_APP

            string uriToLaunch = "mailto:" + emailAddress + "?subject=" + subject + "&body=" + body;
            UriBuilder uriSite = new UriBuilder(uriToLaunch);
            await Windows.System.Launcher.LaunchUriAsync(uriSite.Uri);
#endif

        }

        private async void GoWeibo(object obj)
        {
            UriBuilder uriSite = new UriBuilder("http://weibo.com/jevonsflash");
            await Windows.System.Launcher.LaunchUriAsync(uriSite.Uri);
        }

        private async void GoLove(object obj)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store:reviewapp"));
        }
        private bool CanExecute(object parameter)
        {
            return true;
        }

    }
}
