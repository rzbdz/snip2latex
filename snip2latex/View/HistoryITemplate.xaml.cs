using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace snip2latex.View
{
    public sealed partial class HistoryITemplate : UserControl
    {
        public Model.recognizedData Data { get { return this.DataContext as Model.recognizedData; } }
        public HistoryITemplate()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) => Bindings.Update();
        }

        private void StackPanel_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            this.itemFlyout.ShowAt((UIElement)sender,e.GetPosition((UIElement)sender));
        }


        private async void look_Click(object sender, RoutedEventArgs e)
        {
            var d = ((e.OriginalSource)as FrameworkElement).DataContext as Model.recognizedData;
            this.detailWeb.NavigateToString(d.htmlResult.result_f);
            this.detailImage.Source = d.bitmapImage;
            this.detailCode.Text = d.code;
            ContentDialogResult result = await this.detailedDialog.ShowAsync();
            switch (result) {
                case ContentDialogResult.Primary:
                    DataPackage pkg = new DataPackage();
                    pkg.SetText(d.code);
                    Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(pkg);
                    break;
                case ContentDialogResult.Secondary:
                    deleteH(d);
                    break;
                default:
                    break;
            }
        }

        private async void delete_Click(object sender, RoutedEventArgs e)
        {
            var d = ((e.OriginalSource) as FrameworkElement).DataContext as Model.recognizedData;
            deleteH(d);
            await History.history.refreshAsync();
        }
        private async void deleteH(Model.recognizedData data)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "确认",
                Content = "你确定要删除此记录吗,过程不可挽回!",
                PrimaryButtonText = "删除",
                CloseButtonText = "取消",
            };
            ContentDialogResult rs = await dialog.ShowAsync();
            if (rs == ContentDialogResult.Primary) {
                Model.HistoryData.deleteHistory(data);
                await History.history.refreshAsync();
            }
            History.history.refresh();
        }
    }
}
