using Newtonsoft.Json;
using snip2latex.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace snip2latex.Model
{
    public static class HistoryData
    {
        public static ObservableCollection<recognizedData> HistoryCollection;
        public static void addHistory(recognizedData data)
        {
            if (HistoryCollection == null) {
                HistoryCollection = new ObservableCollection<recognizedData>();
            }
            HistoryCollection.Add(data);
        }
        public static async Task<ObservableCollection<recognizedData>> readAsync()
        {
            if (HistoryCollection == null) {
                try {
                    IRandomAccessStreamWithContentType stream = await (await ApplicationData.
                        Current.LocalFolder.GetFileAsync("history.json")).OpenReadAsync();
                    JsonSerializer serializer = JsonSerializer.Create();
                    StreamReader streamReader = new StreamReader(stream.AsStreamForRead());
                    ObservableCollection<recognizedData> readInCollection =
                        serializer.Deserialize<ObservableCollection<recognizedData>>(new JsonTextReader(streamReader));
                    HistoryCollection = readInCollection;
                }
                catch (Exception) {
                    HistoryCollection = new ObservableCollection<recognizedData>();
                }
            }
            return HistoryCollection;
        }
        public static async Task storeAsync()
        {
            try {
                String str = JsonConvert.SerializeObject((ObservableCollection<recognizedData>)HistoryCollection);
                Stream stream =await (await ApplicationData.Current.LocalFolder.CreateFileAsync("history.json")).OpenStreamForWriteAsync();
                StreamWriter writer = new StreamWriter(stream);
                writer.Write(str);
                writer.Flush();
                writer.Close();
            }
            catch (FileNotFoundException) {
                MainPage.Current.toNavigate(typeof(ErrorPage));
                ErrorPage.errorPage.showError("存档历史失败,请做好失去数据的准备!");
            }
        }
    }

    public class recognizedData
    {
        public recognizedData(BitmapImage bitmapImage, String code, HtmlResult htmlResult)
        {
            this.bitmapImage = bitmapImage;
            this.code = code;
            this.htmlResult = htmlResult;
            this.date = DateTime.Now.ToString("MM月dd日HH时mm分");
        }
        public BitmapImage bitmapImage { get; set; }
        public String code { get; set; }
        public HtmlResult htmlResult { get; set; }
        public string date { get; set; }
    }
}
