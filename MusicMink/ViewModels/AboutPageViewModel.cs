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
        public AboutPageViewModel()
        {
            Version = "1.1.0";
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
