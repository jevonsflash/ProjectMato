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
            Version = (string)App.Current.Resources["Version"];
            Brief = "番茄播放器";
            Introduction = "番茄播放器(Mato Player)是一款本地音乐播放器，设计理念是单手操作，实用方便。支持定制播放列表，本地检索，歌词管理，睡眠模式等。";
            StrUpdate = new List<string>();
            StrUpdate.Add("暂无");
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
