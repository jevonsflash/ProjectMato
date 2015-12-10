using MusicMink.ViewModels;
using System;
using Windows.UI.Xaml;

namespace MusicMink.Pages
{
    /// <summary>
    /// Settings!
    /// </summary>
    public sealed partial class Settings : BasePage
    {
        public Settings()
        {
            this.InitializeComponent();

            this.DataContext = SettingsViewModel.Current;
        }
    }
}
