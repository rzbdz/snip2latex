using snip2latex.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
    public sealed partial class History : Page
    {
        public static History history;
        public ObservableCollection<recognizedData> HistoryCollection;
        public History()
        {
            ApplicationView.GetForCurrentView().Title = "识别记录...";
            this.InitializeComponent();
            HistoryCollection = new ObservableCollection<recognizedData>();
            history = this;
            MainPage.Current.hideBackButton();
            init();

        }
        public async void init()
        {
            var Collection = await HistoryData.readAsync();
            foreach (var i in Collection) {
                HistoryCollection.Add(i);
            }
        }
        public async Task initAsync()
        {
            var Collection = await HistoryData.readAsync();
            foreach (var i in Collection) {
                HistoryCollection.Add(i);
            }
        }
        public async Task refreshAsync()
        {
            HistoryCollection.Clear();
            await initAsync();
            await Model.HistoryData.storeAsync();
            GC.Collect();
        }
        public async void refresh()
        {
            HistoryCollection.Clear();
            await initAsync();
            await Model.HistoryData.storeAsync();
            GC.Collect();
        }
        public async System.Threading.Tasks.Task clearAsync()
        {
            ContentDialog c = new ContentDialog
            {
                Title = "警告",
                Content = "清空过程是不可逆的!",
                PrimaryButtonText = "删除",
                CloseButtonText = "取消",
            };
            var rs = await c.ShowAsync();
            if (rs == ContentDialogResult.Primary) {
                await Model.HistoryData.clearAllAsync();
                await Model.HistoryData.storeAsync();
            }
            refresh();
        }
        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            this.refresh();
        }

        private async void trashButton_Click(object sender, RoutedEventArgs e)
        {
            await clearAsync();
        }
    }
}
