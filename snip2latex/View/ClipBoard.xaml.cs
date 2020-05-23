using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace snip2latex.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ClipBoard : Page
    {
        MathJaxServerForTencent tencentServer;

        public ClipBoard()
        {
            this.InitializeComponent();
            ApplicationView.GetForCurrentView().Title = "粘贴图片文件识别吧!";
            initalizeProgressringAndImage();
            tencentServer = new MathJaxServerForTencent();
            tencentServer.init();
            MainPage.Current.showBackButton();
            this.WebDemo.NavigateToString(tencentServer.hint());
        }

        private void initalizeProgressringAndImage()
        {
            progresring.Visibility = Visibility.Collapsed;
            TextDemo.Text = "";
            ImageControl.Source = new BitmapImage();
        }


        private void fixWebButton_Click(object sender, RoutedEventArgs e)
        {
            progresring.Visibility = Visibility.Visible;
            string boxStr = this.TextDemo.Text;
            boxStr = tencentServer.fixFomulashtml(boxStr);
            WebDemo.NavigateToString(boxStr);
            progresring.Visibility = Visibility.Collapsed;
        }



        private async System.Threading.Tasks.Task pasteImageAndDeSerAsync()
        {
            var dp = Clipboard.GetContent();
            IReadOnlyList<string> f = dp.AvailableFormats;
            foreach (string format in f) {
                Debug.WriteLine(format);
            }
            if (f.Contains(StandardDataFormats.Bitmap)) {
                RandomAccessStreamReference file = await dp.GetBitmapAsync();

                BitmapImage image = new BitmapImage();
                await image.SetSourceAsync(await file.OpenReadAsync());
                ImageControl.Source = image;
                await pasteImageAndDeSerAsync(file);
            }
            else if (f.Contains(StandardDataFormats.StorageItems)) {
                DisplayErrorDialog("你复制的是文件不是图片");
            }
            else {
                DisplayErrorDialog("剪切板没有图片");
            }
        }

        private async System.Threading.Tasks.Task pasteImageAndDeSerAsync(RandomAccessStreamReference imageStream)
        {
            if (imageStream != null) {
                try {
                    BitmapImage bmp = new BitmapImage();
                    await bmp.SetSourceAsync(await imageStream.OpenReadAsync());
                    ImageControl.Source = bmp;
                    List<String> data = await TencentData.getLatexStringArrayAsync(imageStream);
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
                }
                catch (WebException ex) {
                    try {
                        var res = (HttpWebResponse)ex.Response;
                        StreamReader streamReader = new StreamReader(res.GetResponseStream());
                        MainPage.Current.toNavigate(typeof(ErrorPage));
                        ErrorPage.errorPage.showError(ex.Message + streamReader.ReadToEnd());
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
                MainPage.Current.toNavigate(typeof(ErrorPage));
                ErrorPage.errorPage.showError("读取剪切板图片失败!检查剪切板内容");
                initalizeProgressringAndImage();
            }
        }



        private async void DisplayErrorDialog(String content)
        {
            ContentDialog errorDialog = new ContentDialog
            {
                Title = "额......",
                Content = content + "(请重新截图复制或进行其他图片复制操作)",
                PrimaryButtonText = "重新粘贴",
                CloseButtonText = "取消"
            };
            ImageButton.IsEnabled = false;
            ContentDialogResult result;
            try {
                result = await errorDialog.ShowAsync();
            }
            catch (Exception e) {
                MainPage.Current.toNavigate(typeof(ErrorPage));
                ErrorPage.errorPage.showError(e.Message);
                result = ContentDialogResult.None;
            }
            if (result == ContentDialogResult.Primary) {
                await pasteImageAndDeSerAsync();
            }
        }

        private async void ImageButton_Click(object sender, RoutedEventArgs e)
        {
            ImageButton.IsEnabled = false;
            initalizeProgressringAndImage();
            progresring.Visibility = Visibility.Visible;
            //progresring.IsActive = true;
            await pasteImageAndDeSerAsync();
            progresring.Visibility = Visibility.Collapsed;
            ImageButton.IsEnabled = true;
        }
    }
}
