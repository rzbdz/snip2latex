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
        public ClipBoard()
        {
            ApplicationView.GetForCurrentView().Title = "复制好图片之后识别吧!";
            this.InitializeComponent();
            this.WebDemo.NavigateToString("<html><body><center><p>请点击开始识别进行识别</p></center></body></html>");
            MainPage.Current.showBackButton();
        }

        private void initalize()
        {
            progresring.Visibility = Visibility.Collapsed;
            TextDemo.Text = "";
            ImageControl.Source = new BitmapImage(new Uri("ms-appx:///Assets/Square150x150Logo.scale-200.png"));
        }


        private void fixWebButton_Click(object sender, RoutedEventArgs e)
        {
            string boxStr = this.TextDemo.Text;
            boxStr = MathJaxServer.fixFomulashtml(boxStr);
            WebDemo.NavigateToString(boxStr);
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
                    String str = await LatexFacade.PostNewAsync(imageStream);
                    Model.DataWrapperReturn data = Model.Data.wrapper(str);
                    if (data == null) throw new Exception("Json didn't deserialize anything");
                    Model.Data.restoreWords(data);
                    HtmlResult htmlResult;
                    try {
                        MathJaxServer.init();
                        htmlResult = MathJaxServer.getServerHtmls(data);
                    }
                    catch (Exception e) {
                        htmlResult = MathJaxServer.WebServerErrorHandles(e);
                    }
                    if (recognizeWordsCheck.IsChecked == true) {
                        foreach (var i in data.words_result) {
                            TextDemo.Text += i.words + "\n";
                        }
                        WebDemo.NavigateToString(htmlResult.result_w);
                    }
                    else if (recognizeWordsCheck.IsChecked == false) {
                        foreach (var i in data.formula_result) {
                            TextDemo.Text += i.words + "\n";
                        }
                        WebDemo.NavigateToString(htmlResult.result_f);
                    }
                    else {
                        WebDemo.NavigateToString(MathJaxServer.WebServerErrorHandle(new Exception("wrong option")));
                    }
                    progresring.Visibility = Visibility.Collapsed;
                }
                catch (WebException ex) {
                    try {
                        var res = (HttpWebResponse)ex.Response;
                        StreamReader streamReader = new StreamReader(res.GetResponseStream());
                        ErrorPage.errorPage.showError(ex.Message + streamReader.ReadToEnd());
                    }
                    catch (Exception e) {
                        MainPage.Current.toNavigate(typeof(ErrorPage));
                        ErrorPage.errorPage.showError(e.Message + "(Probably network problem)");
                    }
                }
                catch (Exception ex) {
                    MainPage.Current.toNavigate(typeof(ErrorPage));
                    ErrorPage.errorPage.showError(ex.Message);
                }
            }
            else {
                ErrorPage.errorPage.showError("读取剪切板图片失败!检查剪切板内容");
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

            ContentDialogResult result = await errorDialog.ShowAsync();
            if (result == ContentDialogResult.Primary) {
                await pasteImageAndDeSerAsync();
            }
        }

        private async void ImageButton_Click(object sender, RoutedEventArgs e)
        {
            progresring.Visibility = Visibility.Visible;
            await pasteImageAndDeSerAsync();
        }
    }
}
