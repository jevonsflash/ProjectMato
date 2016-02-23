using MusicMink.ViewModels;
using MusicMinkAppLayer.Models;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
            Loaded += Settings_Loaded;
        }

        private void Settings_Loaded(object sender, RoutedEventArgs e)
        {
            BackgroundModel sel = SettingsViewModel.Current.BackgroundList.Find(c => c.IsSel == true);
            try
            {
                this.LVFeature.SelectedIndex = sel.BackgroundId - 1;

            }
            catch (Exception ex)
            {

                this.LVFeature.SelectedIndex = 0;
            }
        }

        private void LVFeature_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<BackgroundModel> data = ((sender as ListBox).ItemsSource as List<BackgroundModel>);
            string selectedName = ((sender as ListBox).SelectedItem as BackgroundModel).Name;

            foreach (var item in data)
            {
                if (item.Name == selectedName)
                {
                    item.IsSel = true;
                }
                else
                {
                    item.IsSel = false;

                }
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.Current.Navigate(NavigationLocation.SleepModePage);
        }

    }
}
