using System;
using System.IO;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Net;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Media.Imaging;
using snip2latex.View;
using Windows.UI.ViewManagement;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace snip2latex.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Home : Page
    {
        public Home()
        {
            ApplicationView.GetForCurrentView().Title = "开始识别吧!";
            this.InitializeComponent();
            MainPage.Current.hideBackButton();
        }

        private void getFromfileButton_Click(object sender, RoutedEventArgs e)
        {
            MainPage.Current.toNavigate(typeof(ConvertPage));

        }

        private void getFromSnipButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
