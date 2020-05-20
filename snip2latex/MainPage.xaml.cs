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
        public MainPage()
        {
            this.InitializeComponent();
            setTitleBarColor(Windows.UI.Color.FromArgb(1, 73, 73, 73), Windows.UI.Color.FromArgb(1, 100, 100, 100));
            
        }
        private void setTitleBarColor(Windows.UI.Color bgColor, Windows.UI.Color btnHoverColor)
        {
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = bgColor;
            titleBar.ButtonBackgroundColor =bgColor;
            titleBar.ButtonHoverBackgroundColor = btnHoverColor;
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
                    String str = await LatexFacade.PostAsync(file);
                    Model.DataWrapperReturn data = Model.Data.wrapper(str);
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
                    var res = (HttpWebResponse)ex.Response;
                    StreamReader streamReader = new StreamReader(res.GetResponseStream());
                    TextDemo.Text = streamReader.ReadToEnd();
                }
                catch (Exception ex) {
                    TextDemo.Text = ex.ToString();
                }
            }

        }

    }
}
