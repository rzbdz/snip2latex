using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using snip2latex.Model.Baidu;
using System.IO;
using Windows.Storage.Streams;

namespace snip2latex
{
    public static class MathJaxServerForBaidu
    {
        private readonly static String pre = "<!DOCTYPE html><html><head><meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width=device-width\"><title>Math Jax example</title><script src=\"https://polyfill.io/v3/polyfill.min.js?features=es6\"></script><script id=\"MathJax-script\" async src=\"https://cdn.jsdelivr.net/npm/mathjax@3/es5/tex-mml-chtml.js\"></script></head><body>";
        private readonly static String suf = "</body></html>";
        private readonly static String errpre = "<html><body><center><h1 >&gt;_&lt;LaTeX 渲染器遇到异常</h1><p>遇到异常,异常信息为:</p></center><p>";
        private readonly static String inLineleftBrace = "\\(";
        private readonly static String inLineRightBrace = "\\)";
        private readonly static String outLineleftBrace = "\\[";
        private readonly static String outLineRightBrace = "\\]";
        private static string result = "";
        private static string result_f = "";
        private static string result_w = "";
        public static string hint()
        {
            return "<html><body><center><p>请点击开始识别进行识别</p></center></body></html>";
        }
        public static void init()
        {
            result = "";
        }
        public static string WebServerErrorHandle(Exception ex)
        {
            return errpre + ex.Message + "</p></body></html>"; ;
        }
        public static HtmlResult WebServerErrorHandles(Exception ex)
        {
            string errsuf = "</p></body></html>";
            return new HtmlResult(errpre + ex.Message + errsuf, errpre + ex.Message + errsuf);
        }
        public static string singleOutlineFomula(string fomulaString)
        {
            result = pre + "<p>" + outLineleftBrace + fomulaString + outLineRightBrace + "</p>" + suf;
            return result;
        }
        public static string multiOutlineFomulas(DataWrapperReturn data, BaiduData.FomulaWordsSeparateOption option)
        {
            result = pre;
            switch (option) {
                case BaiduData.FomulaWordsSeparateOption.fomula:
                    foreach (var i in data.formula_result) {
                        result += "<p>" + outLineleftBrace + i.words + outLineRightBrace + "/p";
                    }
                    break;
                case BaiduData.FomulaWordsSeparateOption.words:
                    foreach (var i in data.words_result) {
                        string str = i.words.Replace(" ", " \\ ");
                        str = i.words.Replace("\\\\ \\ ", "\\\\ ");
                        result += outLineleftBrace + str + outLineRightBrace;
                    }
                    break;
                case BaiduData.FomulaWordsSeparateOption.bothFomulaAndWords:
                    result += "<p>纯公式部分</p>";
                    foreach (var i in data.formula_result) {
                        result += outLineleftBrace + i.words + outLineRightBrace;
                    }
                    result += "<p>含文本部分</p>";
                    foreach (var i in data.words_result) {
                        string str = i.words.Replace(" ", " \\ ");
                        str = i.words.Replace("\\\\ \\ ", "\\\\ ");
                        result += outLineleftBrace + str + outLineRightBrace;
                    }
                    break;
            }
            return result+suf;
        }
        public static string fixFomulashtml(String box)
        {
            if (box != "") {
                return pre + fixFomulas(box) + suf;

            }
            else {
                return hint();
            }
        }
        public static string fixFomulas(String boxInput)
        {
            //string str = boxInput.Replace(" ", " \\ ");
            string str =boxInput;
            str = str.Replace("\\\\ \\ ", "\\\\ ");
            //str = str.Replace("\n", " \\\\ ");
            
            str = outLineleftBrace + str + outLineRightBrace;
            return str;
        }
        public static void multiOutlineFomulas(DataWrapperReturn data)
        {
            result = pre;
            result_f = pre;
            result_w = pre;
            result += "<p>纯公式部分</p>";
            foreach (var i in data.formula_result) {
                string cache = outLineleftBrace + i.words + outLineRightBrace;
                result += cache;
                result_f += cache;
            }
            result += "<p>含文本部分</p>";
            foreach (var i in data.words_result) {
                string str = i.words.Replace(" ", " \\ ");
                str = i.words.Replace("\\\\ \\ ", "\\\\ ");
                string cache = outLineleftBrace + str + outLineRightBrace;
                result += cache;
                result_w += cache;
            }
        }
        public static string getServerHtmlAsync()
        {
            return result;
        }
        public static HtmlResult getServerHtmls()
        {

            return new HtmlResult(result_f, result_w);

        }

        public static HtmlResult getServerHtmls(DataWrapperReturn WrapperReturnData)
        {
            init();
            multiOutlineFomulas(WrapperReturnData);
            return new HtmlResult(result_f, result_w);

        }
    }
    public class HtmlResult
    {
        public HtmlResult(string f, string w)
        {
            this.result_f = f;
            this.result_w = w;
        }
        public string result_f { get; set; }
        public string result_w { get; set; }
    }
}
