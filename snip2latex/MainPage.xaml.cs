using snip2latex.View;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace snip2latex
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static MainPage Current;

        public MainPage()
        {
            this.InitializeComponent();
            Current = this;
            setTitleBarColor(Windows.UI.Color.FromArgb(1, 73, 73, 73), Windows.UI.Color.FromArgb(1, 100, 100, 100));
            MainFrame.Navigate(typeof(Home));

        }
        private void setTitleBarColor(Windows.UI.Color bgColor, Windows.UI.Color btnHoverColor)
        {
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = bgColor;
            titleBar.ButtonBackgroundColor = bgColor;
            titleBar.ButtonHoverBackgroundColor = btnHoverColor;
        }

        private void mainNav_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked) {
            }
        }

        public void goBack()
        {
            if (MainFrame.CanGoBack) {
                MainFrame.GoBack();
            }
            else {
                MainFrame.Navigate(typeof(Home));
            }
        }
        public void toNavigate(Type sourcePageType)
        {
            this.MainFrame.Navigate(sourcePageType);
        }
    }
}
