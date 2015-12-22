using MusicMink.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

// “用户控件”项模板在 http://go.microsoft.com/fwlink/?LinkId=234236 上有介绍

namespace MusicMink.Controls
{
    public sealed partial class ErrorboxControl : UserControl
    {
        private string message;
        private bool isbtnshow;
        public ErrorboxControl()
        {
            this.InitializeComponent();
        }



        public bool IsBtnShow
        {
            get { return (bool)GetValue(IsBtnShowProperty); }
            set
            {
                SetValue(IsBtnShowProperty, value);

            }
        }

        // Using a DependencyProperty as the backing store for IsBtnShow.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsBtnShowProperty =
            DependencyProperty.Register("IsBtnShow", typeof(bool), typeof(ErrorboxControl), new PropertyMetadata(false, OnMsgChanged));



        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(ErrorboxControl), new PropertyMetadata("你还没有添加任何歌曲哦~", OnMsgChanged));


        private static void OnMsgChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ErrorboxControl errorboxControl = d as ErrorboxControl;
            errorboxControl.message = errorboxControl.Message;
            errorboxControl.isbtnshow = errorboxControl.IsBtnShow;
            errorboxControl.InitContent();
        }
        public void InitContent()
        {
            this.TBMessage.Text = message;
            this.BTNFun.Visibility = isbtnshow ? Visibility.Visible : Visibility.Collapsed;
        }

        private void BTNClose_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }

        private void BTNFun_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.Current.Navigate(NavigationLocation.ManageLibrary);
        }
    }
}
