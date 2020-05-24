using snip2latex.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace snip2latex.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Settings : Page
    {
        public Settings()
        {
            ApplicationView.GetForCurrentView().Title = "设置";
            this.InitializeComponent();
            MainPage.Current.hideBackButton();
            this.apiCombo.SelectedIndex = 0;
            if (App.currentApplicationSettings != null) {
                this.defaultWordsToggle.IsOn = App.currentApplicationSettings.isDefaultWords;
                this.isNotClearCodeToggle.IsOn = App.currentApplicationSettings.isNotClearRecognizedCode;
            }
        }


        private void newApiBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).SelectedIndex == 0) {
                this.apiBoxAndBtn.Visibility = Visibility.Collapsed;
            }
            else {
                this.apiBoxAndBtn.Visibility = Visibility.Visible;
            }
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            MainPage.Current.toNavigate(typeof(AboutPage));
        }
        private async System.Threading.Tasks.Task saveTheSettingsAsync()
        {
            
            App.currentApplicationSettings.isDefaultWords = this.defaultWordsToggle.IsOn;
            App.currentApplicationSettings.isNotClearRecognizedCode = this.isNotClearCodeToggle.IsOn;
            try {
                await SaveSetting.saveAllSettingsAsync(App.currentSettingFile, App.currentApplicationSettings);
            }
            catch (Exception) {

            }
        }

        private async void defaultWordsToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (App.currentApplicationSettings == null) {
                App.currentApplicationSettings = new ApplicationSettings();
            }
            if (((ToggleSwitch)sender).IsOn == App.currentApplicationSettings.isDefaultWords) {
            }
            else {
                await saveTheSettingsAsync();
            }

        }
        private async void isNotClearCodeToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (App.currentApplicationSettings == null) {
                App.currentApplicationSettings = new ApplicationSettings();
            }
            if (((ToggleSwitch)sender).IsOn == App.currentApplicationSettings.isNotClearRecognizedCode) {
            }
            else {
                await saveTheSettingsAsync();
            }

        }

        
    }
}
