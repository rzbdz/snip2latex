using System;
using System.IO;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Net;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Media.Imaging;
using snip2latex;
using System.Collections.Generic;
using snip2latex.Model;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace snip2latex.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ConvertPage : Page
    {
        public static ConvertPage convertPage;
        MathJaxServerForTencent tencentServer;
        public static string saveStringTorefresh;
        public ConvertPage()
        {
            this.InitializeComponent();
            ApplicationView.GetForCurrentView().Title = "开始导入图片文件识别吧!";
            initalizeProgressringAndImage();
            this.tencentServer = new MathJaxServerForTencent();
            tencentServer.init();
            MainPage.Current.showBackButton();
            this.WebDemo.NavigateToString(tencentServer.hint());
            convertPage = this;
            if(App.currentApplicationSettings != null) {
                if (App.currentApplicationSettings.isDefaultWords) {
                    this.recognizeWordsCheck.IsChecked = true;
                }
            }
        }

        private void initalizeProgressringAndImage()
        {
            progressring.Visibility = Visibility.Collapsed;
            if (App.currentApplicationSettings != null) {
                if (App.currentApplicationSettings.isNotClearRecognizedCode) {

                }
                else {
                    TextDemo.Text = "";
                }
            }
            else {
                TextDemo.Text = "";
            }
            ImageControl.Source = new BitmapImage();
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            ImageButton.IsEnabled = false;
            initalizeProgressringAndImage();
            progressring.Visibility = Visibility.Visible;
            await chooseImageAndDeSerAsync();
            ImageButton.IsEnabled = true;
            progressring.Visibility = Visibility.Collapsed;

        }

        private void fixWebButton_Click(object sender, RoutedEventArgs e)
        {
            string boxStr = this.TextDemo.Text;
            boxStr = tencentServer.fixFomulashtml(boxStr);
            WebDemo.NavigateToString(boxStr);
        }

        private async System.Threading.Tasks.Task chooseImageAndDeSerAsync()
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            var file = await picker.PickSingleFileAsync();
            if (file != null) {
                file = await file.CopyAsync(ApplicationData.Current.LocalFolder, file.GetHashCode() + file.Name);
                try {
                    BitmapImage bmp = new BitmapImage();
                    await bmp.SetSourceAsync(await file.OpenAsync(FileAccessMode.Read));
                    ImageControl.Source = bmp;
                    List<String> data;
                    if (this.recognizeWordsCheck.IsChecked == true) {
                        data = await TencentPaperData.getPaperStringArrayAsync(file);
                    }
                    else {
                        data = await TencentData.getLatexStringArrayAsync(file);
                    }
                    if (data == null) throw new Exception("Json didn't deserialize anything");
                    HtmlResult htmlResult;
                    try {
                        tencentServer.init();
                        htmlResult = tencentServer.getServerHtmls(data);
                    }
                    catch (Exception e) {
                        htmlResult = tencentServer.WebServerErrorHandles(e);
                    }
                    if (recognizeWordsCheck.IsChecked == true) {
                        foreach (var i in data) {
                            TextDemo.Text += i + "\n";
                        }
                        WebDemo.NavigateToString(htmlResult.result_w);
                    }
                    else if (recognizeWordsCheck.IsChecked == false) {
                        foreach (var i in data) {
                            TextDemo.Text += i + "\n";
                        }
                        WebDemo.NavigateToString(htmlResult.result_f);
                    }
                    else {
                        WebDemo.NavigateToString(tencentServer.WebServerErrorHandle(new Exception("wrong option")));
                    }
                    HistoryData.addHistory(new recognizedData(bmp,TextDemo.Text,htmlResult,file));
                    await HistoryData.storeAsync();
                }
                catch (WebException ex) {
                    try {
                        var res = (HttpWebResponse)ex.Response;
                        StreamReader streamReader = new StreamReader(res.GetResponseStream());
                        MainPage.Current.toNavigate(typeof(ErrorPage));

                        ErrorPage.errorPage.showError(ex.Message + streamReader.ReadToEnd());

                        initalizeProgressringAndImage();
                    }
                    catch (Exception e) {
                        MainPage.Current.toNavigate(typeof(ErrorPage));
                        ErrorPage.errorPage.showError(e.Message + "(Probably network problem)");
                        initalizeProgressringAndImage();
                    }
                }
                catch (Exception ex) {
                    MainPage.Current.toNavigate(typeof(ErrorPage));
                    ErrorPage.errorPage.showError(ex.Message);
                    initalizeProgressringAndImage();
                }
            }
            else {
                DisplayNoFileDialog();
                initalizeProgressringAndImage();
            }
            saveStringTorefresh = TextDemo.Text;
        }

        private async void DisplayNoFileDialog()
        {
            ContentDialog noFileDialog = new ContentDialog
            {
                Title = "额......",
                Content = "你没有选择任何图片!",
                PrimaryButtonText = "重新选择",
                CloseButtonText = "取消"
            };

            ContentDialogResult result = await noFileDialog.ShowAsync();
            if (result == ContentDialogResult.Primary) {
                await chooseImageAndDeSerAsync();
            }
        }

        private void refreshCodeButton_Click(object sender, RoutedEventArgs e)
        {
            if (saveStringTorefresh != null) {
                this.TextDemo.Text = saveStringTorefresh;
            }
        }
    }
}
