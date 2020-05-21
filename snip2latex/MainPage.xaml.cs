using snip2latex.View;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
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
using Windows.UI.Xaml.Media.Animation;
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
            setTitleBarColor(Windows.UI.Color.FromArgb(1, 107, 105, 214), Windows.UI.Color.FromArgb(1, 210, 195, 255));
            MainFrame.Navigate(typeof(Home));
            mainNav.IsPaneOpen = false;
            int HomePageIndex = 0;
            mainNav.SelectedItem = mainNav.MenuItems[HomePageIndex];
            hideBackButton();
        }
        private void setTitleBarColor(Windows.UI.Color bgColor, Windows.UI.Color btnHoverColor)
        {
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = bgColor;
            titleBar.ButtonBackgroundColor = bgColor;
            titleBar.ButtonHoverBackgroundColor = btnHoverColor;
        }

        private readonly List<(string Tag, Type Page)> _pages = new List<(string Tag, Type Page)>
        {
            ("Home", typeof(Home)),
            ("History", typeof(History)),
        };

        private void mainNav_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked) {
                toNavigate(typeof(Settings));
            }
            else if (args.InvokedItemContainer != null) {
                var navItemTag = args.InvokedItemContainer.Tag.ToString();
                NavView_Navigate(navItemTag, args.RecommendedNavigationTransitionInfo);
            }
        }

        private void NavView_Navigate(string navItemTag, NavigationTransitionInfo transitionInfo)
        {
            Type _page = null;
            if (navItemTag == "settings") {
                _page = typeof(Settings);
            }
            else {
                var item = _pages.FirstOrDefault(p => p.Tag.Equals(navItemTag));
                _page = item.Page;
            }
            // 获取当前type(不重复)
            var preNavPageType = MainFrame.CurrentSourcePageType;

            // 更新,不重复
            if (!(_page is null) && !Type.Equals(preNavPageType, _page)) {
                MainFrame.Navigate(_page, null, transitionInfo);
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
        public void showBackButton()
        {
            this.mainNav.IsBackButtonVisible = NavigationViewBackButtonVisible.Visible;
        }
        public void hideBackButton()
        {
            this.mainNav.IsBackButtonVisible = NavigationViewBackButtonVisible.Collapsed;
        }
        private void BackInvoked(KeyboardAccelerator sender,
                         KeyboardAcceleratorInvokedEventArgs args)
        {
            On_BackRequested();
            args.Handled = true;
        }
        private void NavView_BackRequested(NavigationView sender,
                                   NavigationViewBackRequestedEventArgs args)
        {
            On_BackRequested();
        }
        private bool On_BackRequested()
        {
            if (!MainFrame.CanGoBack)
                return false;

            // Don't go back if the nav pane is overlayed.
            if (mainNav.IsPaneOpen &&
                (mainNav.DisplayMode == NavigationViewDisplayMode.Compact ||
                 mainNav.DisplayMode == NavigationViewDisplayMode.Minimal))
                return false;

            MainFrame.GoBack();
            return true;
        }
    }
}
