using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using snip2latex;

namespace com.baidu.api
{
    public static class AccessToken

    {
        // 调用getAccessToken()获取的 access_token建议根据expires_in 时间 设置缓存
        // 返回token示例
        //public static String TOKEN = "24.adda70c11b9786206253ddb70affdc46.2592000.1493524354.282335-1234567";

        // 百度云中开通对应服务应用的 API Key 建议开通应用的时候多选服务
        private static String clientId = MyBaiduToken.getApiKey();
        // 百度云中开通对应服务应用的 Secret Key
        private static String clientSecret = MyBaiduToken.getSecretKey();

        public static String getAccessToken()
        {
            String authHost = "https://aip.baidubce.com/oauth/2.0/token";
            HttpClient client = new HttpClient();
            List<KeyValuePair<String, String>> paraList = new List<KeyValuePair<string, string>>();
            paraList.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
            paraList.Add(new KeyValuePair<string, string>("client_id", clientId));
            paraList.Add(new KeyValuePair<string, string>("client_secret", clientSecret));
            String result;
            try {
                HttpResponseMessage response = client.PostAsync(authHost, new FormUrlEncodedContent(paraList)).Result;
                 result = response.Content.ReadAsStringAsync().Result;
            }
            catch (WebException ex) { result = ""; throw ex;  }
            JsonSerializer json = JsonSerializer.Create();
            Token tk = json.Deserialize<Token>(new JsonTextReader(new StringReader(result)));
            return tk.access_token;
        }
    }

    public static class BaiduApi
    {
        public static string getUrl() { return "https://aip.baidubce.com/rest/2.0/ocr/v1/formula"; }
    }

    public class Token
    {
        public string refresh_token { get; set; }
        public int expires_in { get; set; }
        public string scope { get; set; }
        public string session_key { get; set; }
        public string access_token { get; set; }
        public string session_secret { get; set; }
    }

}
