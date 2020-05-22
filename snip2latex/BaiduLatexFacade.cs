using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.baidu.api;
using System.Net;
using Windows.Storage.Streams;
using Windows.Storage;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Net.Http;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web;

namespace snip2latex
{
    public static class BaiduLatexFacade
    {
        public static void GetData()
        {

        }

        public static async Task<string> PostAsync(StorageFile image)
        {
            string result;
            //string token = "24.5e94ae528427824fb5e03aaf8751a7ac.2592000.1592472357.282335-19946172";
            string token = AccessToken.getAccessToken();
            string host = BaiduApi.getUrl() + "?access_token=" + token;
            //string host = "https://aip.baidubce.com/rest/2.0/ocr/v1/formula?access_token=" + token;
            Encoding encoding = Encoding.Default;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(host);
            request.Method = "post";
            request.KeepAlive = true;
            string base64 = await toBase64Async(image);
            String str = "image=" + base64;
            str += "&disp_formula=true";
            byte[] buffer = encoding.GetBytes(str);
            request.ContentType = "application/x-www-form-urlencoded";
            var handler = new HttpClientHandler();
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream, Encoding.Default);
            result = reader.ReadToEnd(); 
            stream.Close();
            response.Close();
            return result;
        }

        public static async Task<string> PostNewAsync(RandomAccessStreamReference imageStream)
        {
            string result;
            string token = AccessToken.getAccessToken();
            string host = BaiduApi.getUrl() + "?access_token=" + token;
            Encoding encoding = Encoding.Default;
            HttpClient client = new HttpClient();
            //client.DefaultRequestHeaders.Add("Content-Type", "application/x-www-form-urlencoded");
            string imageString = await toBase64Async(imageStream);
            imageString ="image="+imageString+"&disp_formula=true";
            imageString =imageString.Replace("\r\n", "");
            imageString = imageString.Replace("+", "%2B");
            //byte[] buffer = encoding.GetBytes(imageString);
            //client.DefaultRequestHeaders.Add("Content-Length", buffer.Length.ToString());
            //List<KeyValuePair<string, string>> pairs = new List<KeyValuePair<string, string>>();
            //pairs.Add(new KeyValuePair<string, string>("image", imageString));
            //pairs.Add(new KeyValuePair<string, string>("disp_formula", "true"));
            //FormUrlEncodedContent content = new FormUrlEncodedContent(pairs);
            StringContent content = new StringContent(imageString,encoding, "application/x-www-form-urlencoded");
            //content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
            var response = await client.PostAsync(host, content);
            //request.GetRequestStream().Write(buffer, 0, buffer.Length);
            //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //Stream stream = response.GetResponseStream();
            //StreamReader reader = new StreamReader(stream, Encoding.Default);
            //result = reader.ReadToEnd();
            result = await response.Content.ReadAsStringAsync();
            return result;
        }

        public static async Task<string> PostNewAsync(StorageFile image)
        {
            string result;
            string token = AccessToken.getAccessToken();
            string host = BaiduApi.getUrl() + "?access_token=" + token;
            Encoding encoding = Encoding.Default;
            HttpClient client = new HttpClient();
            //client.DefaultRequestHeaders.Add("Content-Type", "application/x-www-form-urlencoded");
            string imageString = await toBase64Async(image);
            //str += "&disp_formula=true";
            byte[] buffer = encoding.GetBytes(imageString);
            //client.DefaultRequestHeaders.Add("Content-Length", buffer.Length.ToString());
            //StringContent content = new StringContent(buffer);
            List<KeyValuePair<string, string>> pairs = new List<KeyValuePair<string, string>>();
            pairs.Add(new KeyValuePair<string, string>("image", imageString));
            pairs.Add(new KeyValuePair<string, string>("disp_formula", "true"));
            FormUrlEncodedContent content = new FormUrlEncodedContent(pairs);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
            var response = await client.PostAsync(host,content);
            //request.GetRequestStream().Write(buffer, 0, buffer.Length);
            //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //Stream stream = response.GetResponseStream();
            //StreamReader reader = new StreamReader(stream, Encoding.Default);
            //result = reader.ReadToEnd();
            result = await response.Content.ReadAsStringAsync(); 
            return result;
        }
        
        private static async Task<string> toBase64Async(RandomAccessStreamReference imageStream)
        {
            //FileStream fs = new FileStream(image.Path,FileMode.);
            IRandomAccessStreamWithContentType a = await imageStream.OpenReadAsync();
            byte[] bytes =new byte[a.Size];
            var buffer = await a.ReadAsync(bytes.AsBuffer(), (uint)a.Size, InputStreamOptions.None);
            bytes = buffer.ToArray();
            //buffer.ToArray();
            String base64 = Convert.ToBase64String(bytes);
            //base64 = HttpUtility.UrlEncode(base64);
            return base64;
        }
        private static async Task<string> toBase64Async(StorageFile image)
        {
            //FileStream fs = new FileStream(image.Path,FileMode.);
            IBuffer buffer = await FileIO.ReadBufferAsync(image as IStorageFile);
            byte[] bytes = buffer.ToArray();
            String base64 = Convert.ToBase64String(bytes);
            //base64 = HttpUtility.UrlEncode(base64);
            //base64=base64.Replace("\r\n", "");
            //base64=base64.Replace("+", "%2B");
            return base64;
        }
    }
}
