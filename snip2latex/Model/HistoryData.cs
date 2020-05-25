using Newtonsoft.Json;
using snip2latex.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
        public static async Task clearAllAsync()
        {
            foreach (var i in HistoryCollection) {
                try {
                    await i.imageFile.DeleteAsync();
                }
                catch(Exception) {
                    continue;
                }
            }
            HistoryCollection.Clear();
            GC.Collect();
        }
        public static async void deleteHistory(recognizedData data)
        {
            int i;
            for (i = 0; i < HistoryCollection.Count(); i++) {
                if (HistoryCollection.ElementAt(i).imageName == data.imageName && HistoryCollection.ElementAt(i).date == data.date) {
                    break;
                }
            }
            try {
                var cache = HistoryCollection.ElementAt(i).imageFile;
                HistoryCollection.RemoveAt(i);
                await cache.DeleteAsync();
            }catch(Exception e){
                MainPage.Current.toNavigate(typeof(ErrorPage));
                ErrorPage.errorPage.showError(e.Message);
            }
            GC.Collect();
            
        }
        public static async Task<ObservableCollection<recognizedData>> readAsync()
        {
            ObservableCollection<recognizedData> readInCollection = new ObservableCollection<recognizedData>(); ;
            if (HistoryCollection == null) {
                try {
                    IRandomAccessStreamWithContentType stream = await (await ApplicationData.
                        Current.LocalFolder.GetFileAsync("history.json")).OpenReadAsync();
                    JsonSerializer serializer = JsonSerializer.Create();
                    StreamReader streamReader = new StreamReader(stream.AsStreamForRead());
                    readInCollection =
                        serializer.Deserialize<ObservableCollection<recognizedData>>(new JsonTextReader(streamReader));
                    stream.Dispose();
                    foreach (var i in readInCollection) {
                        try {
                            i.imageFile = await ApplicationData.Current.LocalFolder.GetFileAsync(i.imageName);
                            i.bitmapImage = new BitmapImage();
                            await i.bitmapImage.SetSourceAsync(await i.imageFile.OpenReadAsync());
                        }
                        catch (FileNotFoundException) {
                            continue;
                        }
                    }
                }
                catch (Exception) {
                    //HistoryCollection = new ObservableCollection<recognizedData>();
                }
                finally {
                    HistoryCollection = readInCollection;
                    GC.Collect();
                }
            }
            return HistoryCollection;
        }
        public static async Task storeAsync()
        {
            try {
                String str = JsonConvert.SerializeObject((ObservableCollection<recognizedData>)HistoryCollection);
                Stream stream = await (await ApplicationData.Current.LocalFolder.CreateFileAsync("history.json", CreationCollisionOption.ReplaceExisting)).OpenStreamForWriteAsync();
                StreamWriter writer = new StreamWriter(stream);
                writer.Write(str);
                writer.Flush();
                writer.Close();
            }
            catch (FileNotFoundException) {
                MainPage.Current.toNavigate(typeof(ErrorPage));
                ErrorPage.errorPage.showError("存档历史失败,请做好失去数据的准备!");
            }
            finally {
                GC.Collect();
            }
        }
    }

    [JsonObject(MemberSerialization.OptOut)]
    public class recognizedData
    {
        [JsonConstructor]
        public recognizedData(string base64, string code, HtmlResult htmlResult, string date, string imageName)
        {
            this.base64 = base64;
            this.code = code;
            this.htmlResult = htmlResult;
            this.date = date;
            this.imageName = imageName;
        }
        public recognizedData(BitmapImage bitmapImage, String code, HtmlResult htmlResult, StorageFile file)
        {
            this.bitmapImage = bitmapImage;
            this.code = code;
            this.htmlResult = htmlResult;
            this.date = DateTime.Now.ToString("MM月dd日HH时mm分");
            if (file != null) {
                this.imageFile = file;
                this.imageName = file.Name;
            }

        }

        public String base64 { get; set; }
        public String code { get; set; }
        public HtmlResult htmlResult { get; set; }
        public string date { get; set; }
        [JsonIgnore]
        public StorageFile imageFile { get; set; }
        public String imageName { get; set; }
        [JsonIgnore]
        public BitmapImage bitmapImage { get; set; }
    }
}
