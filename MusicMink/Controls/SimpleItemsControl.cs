﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// “用户控件”项模板在 http://go.microsoft.com/fwlink/?LinkId=234235 上有介绍

namespace MusicMink.Controls
{
    public sealed class SimpleItemsControl : ItemsControl
    {
        public SimpleItemsControl()
        {
            this.DefaultStyleKey = typeof(SimpleItemsControl);


        }
    }
}
