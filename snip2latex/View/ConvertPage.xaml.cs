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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace snip2latex.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ConvertPage : Page
    {
        public static ConvertPage convertPage;
        public ConvertPage()
        {
            ApplicationView.GetForCurrentView().Title = "开始导入图片文件识别吧!";
            this.InitializeComponent();
            convertPage = this;
            this.WebDemo.NavigateToString("<html><body><center><p>请点击开始识别进行识别</p></center></body></html>");
            MainPage.Current.showBackButton();
        }

        private void initalize()
        {
            progresring.Visibility = Visibility.Collapsed;
            TextDemo.Text = "";
            ImageControl.Source = new BitmapImage(new Uri("ms-appx:///Assets/Square150x150Logo.scale-200.png"));
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            progresring.Visibility = Visibility.Visible;
            progresring.IsActive = true;
            ImageButton.IsEnabled = false;
            await chooseImageAndDeSerAsync();
            ImageButton.IsEnabled = true;
        }



        private void fixWebButton_Click(object sender, RoutedEventArgs e)
        {
            string boxStr = this.TextDemo.Text;
            boxStr =  MathJaxServer.fixFomulashtml(boxStr);
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
                try {
                    BitmapImage bmp = new BitmapImage();
                    await bmp.SetSourceAsync(await file.OpenAsync(FileAccessMode.Read));
                    ImageControl.Source = bmp;
                    String str = await LatexFacade.PostNewAsync(file);
                    Model.DataWrapperReturn data = Model.Data.wrapper(str);
                    if (data == null) throw new Exception("Json didn't deserialize anything");
                    Model.Data.restoreWords(data);
                    //string htmlString;
                    HtmlResult htmlResult;
                    try {
                        MathJaxServer.init();
                        //MathJaxServer.multiOutlineFomulas(data, Model.Data.FomulaWordsSeparateOption.bothFomulaAndWords);
                        //MathJaxServer.multiOutlineFomulas(data);
                        //htmlString = MathJaxServer.getServerHtmlAsync();
                        htmlResult = MathJaxServer.getServerHtmls(data);
                    }
                    catch (Exception e) {
                        //htmlString = MathJaxServer.WebServerErrorHandle(e);
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
                    //WebDemo.NavigateToString(htmlString);
                    //WebDemo_f.NavigateToString(htmlResult.result_f);
                    //WebDemo_w.NavigateToString(htmlResult.result_w);
                    progresring.Visibility = Visibility.Collapsed;
                }
                catch (WebException ex) {
                    try {
                        var res = (HttpWebResponse)ex.Response;
                        StreamReader streamReader = new StreamReader(res.GetResponseStream());
                        ErrorPage.errorPage.showError(ex.Message + streamReader.ReadToEnd());
                        
                        initalize();
                    }
                    catch (Exception e) {
                        MainPage.Current.toNavigate(typeof(ErrorPage));
                        ErrorPage.errorPage.showError(e.Message + "(Probably network problem)");
                        initalize();
                    }
                }
                catch (Exception ex) {
                    MainPage.Current.toNavigate(typeof(ErrorPage));
                    ErrorPage.errorPage.showError(ex.Message);
                    initalize();
                }
            }
            else {
                DisplayNoFileDialog();
                initalize();
            }
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

    }
}
