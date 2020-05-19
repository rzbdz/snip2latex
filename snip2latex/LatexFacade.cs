using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Windows.UI.Xaml.Controls;
using com.baidu.api;
using System.Net;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Media;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.Storage;
using System.Runtime.InteropServices.WindowsRuntime;

namespace snip2latex
{
    public static class LatexFacade
    {
        public static void GetData()
        {

        }
        public static async Task<string> PostAsync(StorageFile image)
        {
            string result;
            string token = AccessToken.getAccessToken();
            string host = BaiduApi.getUrl() + "?access_token=" + token;
            Encoding encoding = Encoding.Default;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(host);
            request.Method = "post";
            request.KeepAlive = true;
            string base64 = await toBase64Async(image);
            String str = "image=" + base64;
            //str += "&disp_formula=true";
            byte[] buffer = encoding.GetBytes(str);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.Default);
            result = reader.ReadToEnd();
            return result;
        }

        private static async Task<string> toBase64Async(StorageFile image)
        {
            //BinaryFormatter binaryFormatter = new BinaryFormatter();
            //MemoryStream memoryStream = new MemoryStream();
            ////IRandomAccessStream iras = await image.OpenAsync(FileAccessMode.Read) ;
            //binaryFormatter.Serialize(memoryStream, image);
            IBuffer buffer = await FileIO.ReadBufferAsync(image as IStorageFile);
            byte[] bytes = buffer.ToArray();
            String base64 = Convert.ToBase64String(bytes);
            //base64 = HttpUtility.UrlEncode(base64);
            base64=base64.Replace("\r\n", "");
            base64=base64.Replace("+", "%2B");
            return base64;
        }
    }
}
