using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using snip2latex.Model;
using System.IO;
using Windows.Storage.Streams;

namespace snip2latex
{
    public static class MathJaxServer
    {
        private readonly static String pre = "<!DOCTYPE html><html><head><meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width=device-width\"><title>Math Jax example</title><script src=\"https://polyfill.io/v3/polyfill.min.js?features=es6\"></script><script id=\"MathJax-script\" async src=\"https://cdn.jsdelivr.net/npm/mathjax@3/es5/tex-mml-chtml.js\"></script></head><body>";
        private readonly static String suf = "</body></html>";
        private readonly static String inLineleftBrace = "\\(";
        private readonly static String inLineRightBrace = "\\)";
        private readonly static String outLineleftBrace = "\\[";
        private readonly static String outLineRightBrace = "\\]";
        public static async Task initAsync()
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile storageFile = await storageFolder.CreateFileAsync("webserver.html", CreationCollisionOption.ReplaceExisting);
        }
        public static async Task singleOutlineFomula(string fomulaString)
        {
            StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync("webserver.html");
            await FileIO.WriteTextAsync(file, pre);
            await FileIO.AppendTextAsync(file, "<p>"+outLineleftBrace + fomulaString + outLineRightBrace+"</p>");
            await FileIO.AppendTextAsync(file, suf);
        }
        public static async Task multiOutlineFomulas(Model.DataWrapperReturn data,Model.Data.FomulaWordsSeparateOption option)
        {
            StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync("webserver.html");
            await FileIO.WriteTextAsync(file, pre);
            switch (option) {
                case Data.FomulaWordsSeparateOption.fomula:
                    foreach(var i in data.formula_result) {
                        await FileIO.AppendTextAsync(file,"<p>"+ outLineleftBrace + i.words + outLineRightBrace+"/p");
                    }
                    break;
                case Data.FomulaWordsSeparateOption.words:
                    foreach (var i in data.words_result) {
                        string str = i.words.Replace(" ", " \\ ");
                        str = i.words.Replace("\\\\ \\ ", "\\\\ ");
                        await FileIO.AppendTextAsync(file, outLineleftBrace + str + outLineRightBrace);
                    }
                    break;
                case Data.FomulaWordsSeparateOption.bothFomulaAndWords:
                    await FileIO.AppendTextAsync(file, "<p>纯公式部分</p>");
                    foreach (var i in data.formula_result) {
                        await FileIO.AppendTextAsync(file, outLineleftBrace + i.words + outLineRightBrace);
                    }
                    await FileIO.AppendTextAsync(file, "<p>含文本部分</p>");
                    foreach (var i in data.words_result) {
                        string str = i.words.Replace(" ", " \\ ");
                        str = i.words.Replace("\\\\ \\ ", "\\\\ ");
                        await FileIO.AppendTextAsync(file, outLineleftBrace + str + outLineRightBrace);
                    }
                    break;
            }
            await FileIO.AppendTextAsync(file, suf);
        }
        public static async Task<string> getServerHtmlAsync()
        {
            StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync("webserver.html");
            Stream stream = await file.OpenStreamForReadAsync();
            StreamReader streamReader = new StreamReader(stream, Encoding.Default);
            return streamReader.ReadToEnd();
        }
    }
}
