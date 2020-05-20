using System;
using System.IO;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Net;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Media.Imaging;
using snip2latex.View;

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
            this.InitializeComponent();
        }

        private void initalize()
        {
            progresring.Visibility = Visibility.Collapsed;
            imagecontrol.Source = new BitmapImage(new Uri("ms-appx:///Assets/Square150x150Logo.scale-200.png"));
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            progresring.Visibility = Visibility.Visible;
            progresring.IsActive = true;
            await chooseImageAndDeSerAsync();

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
                    imagecontrol.Source = bmp;
                    String str = await LatexFacade.PostNewAsync(file);
                    Model.DataWrapperReturn data = Model.Data.wrapper(str);
                    if (data == null) throw new Exception("Json didn't deserialize anything");
                    Model.Data.restoreWords(data);
                    TextDemo.Text = data.formula_result_num + "\n";
                    await MathJaxServer.initAsync();
                    foreach (var i in data.formula_result) {
                        TextDemo.Text += i.words + "\n";
                    }
                    TextDemo.Text += "including words:\n";
                    foreach (var i in data.words_result) {
                        TextDemo.Text += i.words + "\n";
                    }
                    await MathJaxServer.multiOutlineFomulas(data, Model.Data.FomulaWordsSeparateOption.bothFomulaAndWords);
                    string htmlString = await MathJaxServer.getServerHtmlAsync();
                    WebDemo.NavigateToString(htmlString);
                    progresring.Visibility = Visibility.Collapsed;
                }
                catch (WebException ex) {
                    try {
                        var res = (HttpWebResponse)ex.Response;
                        StreamReader streamReader = new StreamReader(res.GetResponseStream());
                        TextDemo.Text = streamReader.ReadToEnd();
                        initalize();
                    }catch(Exception e) {
                        MainPage.Current.toNavigate(typeof(ErrorPage));
                        ErrorPage.errorPage.showError(e.Message+"(Probably network problem)");
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
                CloseButtonText = "我不搞了"
            };

            ContentDialogResult result = await noFileDialog.ShowAsync();
            if(result == ContentDialogResult.Primary) {
                await chooseImageAndDeSerAsync();
            }
        }
    }
}
