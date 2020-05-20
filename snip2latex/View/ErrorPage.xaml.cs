using System;
using System.Collections.Generic;
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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace snip2latex.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ErrorPage : Page
    {
        public static ErrorPage errorPage;
        public ErrorPage()
        {
            this.InitializeComponent();
            errorPage = this;
        }

        public void showError(string error_msg)
        {
            this.errorBlock.Text = error_msg;
            this.errorBlock.TextAlignment = TextAlignment.DetectFromContent;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainPage.Current.goBack();
        }
    }

}
