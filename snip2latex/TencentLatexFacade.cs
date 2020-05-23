using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using TencentCloud.Common;
using TencentCloud.Common.Profile;
using TencentCloud.Ocr.V20181119;
using TencentCloud.Ocr.V20181119.Models;
using Windows.Storage;
using Windows.Storage.Streams;

namespace snip2latex
{
    public static class TencentData
    {

        public static async Task<List<String>> getLatexStringArrayAsync(RandomAccessStreamReference imageStream)
        {
            return await getReturnStringArrayAsync(await toBase64Async(imageStream));
        }

        public static async Task<List<String>> getLatexStringArrayAsync(StorageFile file)
        {
            return await getReturnStringArrayAsync(await toBase64Async(file));
        }

        private static async Task<List<String>> getReturnStringArrayAsync(string imageBase64)
        {
            Credential cred = new Credential
            {
                SecretId = TencentToken.secrectId,
                SecretKey = TencentToken.secrectKey
            };
            OcrClient client = new OcrClient(cred, "ap-guangzhou");
            FormulaOCRRequest request = new FormulaOCRRequest();
            request.ImageBase64 = imageBase64;
            FormulaOCRResponse response = await client.FormulaOCR(request);
            List<String> result = new List<string>();
            foreach (var i in response.FormulaInfos) {
                result.Add(i.DetectedText);
            }
            return result;
        }

        private static async Task<string> toBase64Async(RandomAccessStreamReference imageStream)
        {
            //FileStream fs = new FileStream(image.Path,FileMode.);
            IRandomAccessStreamWithContentType a = await imageStream.OpenReadAsync();
            byte[] bytes = new byte[a.Size];
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
