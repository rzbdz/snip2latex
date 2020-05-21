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
            this.InitializeComponent();
        }
        private async System.Threading.Tasks.Task chooseImageAndDeSerAsync(RandomAccessStreamReference imageStream)
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
                    box.Text = data.formula_result_num + "\n";
                    foreach (var i in data.formula_result) {
                        box.Text += i.words + "\n";
                    }
                    box.Text += "including words:\n";
                    foreach (var i in data.words_result) {
                        box.Text += i.words + "\n";
                    }
                }
                catch (WebException ex) {
                    try {
                        var res = (HttpWebResponse)ex.Response;
                        StreamReader streamReader = new StreamReader(res.GetResponseStream());
                        box.Text = streamReader.ReadToEnd();
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
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var dp = Clipboard.GetContent();
            IReadOnlyList<string>f = dp.AvailableFormats;
            foreach(string format in f) {
                Debug.WriteLine(format);
            }
            if (f.Contains("Text")) {
                string str = await dp.GetTextAsync();
                box.Text += str;
            }else if (f.Contains(StandardDataFormats.Bitmap)) {
                RandomAccessStreamReference file = await dp.GetBitmapAsync();
                
                BitmapImage image = new BitmapImage();
                await image.SetSourceAsync(await file.OpenReadAsync());
                ImageControl.Source = image;
                await chooseImageAndDeSerAsync(file); 
            }else if (f.Contains(StandardDataFormats.StorageItems)) {
                ContentDialog contentDialog = new ContentDialog
                {
                    Title = "dont give file",
                    CloseButtonText = "close",
                    PrimaryButtonText = "yes",


                };
                contentDialog.ShowAsync();

            }
        }
    }
}
