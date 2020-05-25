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
    public class MathJaxServerBase
    {
        protected readonly String pre = "<!DOCTYPE html><html><head><meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width=device-width\"><title>Math Jax example</title><script src=\"https://polyfill.io/v3/polyfill.min.js?features=es6\"></script><script id=\"MathJax-script\" async src=\"https://cdn.jsdelivr.net/npm/mathjax@3/es5/tex-mml-chtml.js\"></script></head><body>";
        protected readonly String suf = "</body></html>";
        protected readonly String errpre = "<html><body><center><h1 >&gt;_&lt;LaTeX 渲染器遇到异常</h1><p>遇到异常,异常信息为:</p></center><p>";
        protected readonly String inLineleftBrace = "\\(";
        protected readonly String inLineRightBrace = "\\)";
        protected readonly String outLineleftBrace = "\\[";
        protected readonly String outLineRightBrace = "\\]";
        protected string result = "";
        protected string result_f = "";
        protected string result_w = "";
        public string hint()
        {
            return "<html><body><center><p>请点击开始识别进行识别</p></center></body></html>";
        }
        public void init()
        {
            result = "";
        }
        public string WebServerErrorHandle(Exception ex)
        {
            return errpre + ex.Message + "</p></body></html>"; ;
        }
        public HtmlResult WebServerErrorHandles(Exception ex)
        {
            string errsuf = "</p></body></html>";
            return new HtmlResult(errpre + ex.Message + errsuf, errpre + ex.Message + errsuf);
        }
        public string singleOutlineFomula(string fomulaString)
        {
            result = pre + "<p>" + outLineleftBrace + fomulaString + outLineRightBrace + "</p>" + suf;
            return result;
        }

        public string fixFomulashtml(String box)
        {
            if (box != "") {
                return pre + fixFomulas(box) + suf;

            }
            else {
                return hint();
            }
        }
        public string fixFomulas(String boxInput)
        {
            //string str = boxInput.Replace(" ", " \\ ");
            string str = boxInput;
            str = str.Replace("\\\\ \\ ", "\\\\ ");
            //str = str.Replace("\n", " \\\\ ");

            str = outLineleftBrace + str + outLineRightBrace;
            return str;
        }

        public string getServerHtmlAsync()
        {
            return result;
        }
        public HtmlResult getServerHtmls()
        {

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
