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
            ApplicationView.GetForCurrentView().Title = "设置!";
            this.InitializeComponent();
            MainPage.Current.hideBackButton(); 
        }


        private void newApiBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(((ComboBox)sender).SelectedIndex == 0) {
                this.apiBoxAndBtn.Visibility = Visibility.Collapsed;
            }
            else {
                this.apiBoxAndBtn.Visibility = Visibility.Visible;
            }
        }
    }
}
