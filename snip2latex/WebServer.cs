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
        private static string result="";
        public static void init()
        {
            result = "";
        }
        public static string WebServerErrorHandle(Exception ex)
        {
            result= "<html><body><center><h1 >&gt;_&lt;LaTeX 渲染器遇到异常</h1><p>遇到异常,异常信息为:</p></center><p>" + ex.Message + "</p></body></html>";
            return result;
        }
        public static string singleOutlineFomula(string fomulaString)
        {
            result = pre + "<p>" + outLineleftBrace + fomulaString + outLineRightBrace + "</p>" + suf;
            return result;
        }
        public static string multiOutlineFomulas(Model.DataWrapperReturn data, Model.Data.FomulaWordsSeparateOption option)
        {
            result = pre;
            switch (option) {
                case Data.FomulaWordsSeparateOption.fomula:
                    foreach (var i in data.formula_result) {
                        result += "<p>" + outLineleftBrace + i.words + outLineRightBrace + "/p";
                    }
                    break;
                case Data.FomulaWordsSeparateOption.words:
                    foreach (var i in data.words_result) {
                        string str = i.words.Replace(" ", " \\ ");
                        str = i.words.Replace("\\\\ \\ ", "\\\\ ");
                        result += outLineleftBrace + str + outLineRightBrace;
                    }
                    break;
                case Data.FomulaWordsSeparateOption.bothFomulaAndWords:
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
            return result;
        }
        public static string getServerHtmlAsync()
        {
            return result;
        }
    }
}
