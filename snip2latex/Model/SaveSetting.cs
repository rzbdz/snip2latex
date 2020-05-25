using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Windows.Storage;
using Windows.Storage.Streams;

namespace snip2latex.Model
{
    public static class SaveSetting
    {
        public static async Task saveAllSettingsAsync(StorageFile file,ApplicationSettings settings)
        {
            IRandomAccessStream stream =await file.OpenAsync(FileAccessMode.ReadWrite);
            string str = JsonConvert.SerializeObject((ApplicationSettings)settings);
            StreamWriter writer = new StreamWriter(stream.AsStreamForWrite());
            writer.Write(str);
            writer.Flush();
            writer.Close();
        }

        public static async Task<ApplicationSettings> importAllSettingsAsync(StorageFile file)
        {
            //StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync("settings.json");
            if (file != null) {
                IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);
                StreamReader reader = new StreamReader(stream.AsStream());
                JsonTextReader jsonTextReader = new JsonTextReader(reader);
                JsonSerializer serializer = JsonSerializer.Create();
                ApplicationSettings s = (ApplicationSettings)serializer.Deserialize(jsonTextReader,typeof(ApplicationSettings));
                stream.Dispose();
                return s;
            }
            else {
                return null;
            }
        }
    }
    public class ApplicationSettings
    {
        public bool isDefaultWords { get; set; }
        public bool isDefaultSeparateCode { get; set; }
        public bool isNotClearRecognizedCode { get; set; }
        public UserToken userToken;
    }
}
