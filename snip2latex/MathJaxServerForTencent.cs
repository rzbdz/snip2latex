using System;
using System.Collections.Generic;

namespace snip2latex
{
    public  class MathJaxServerForTencent:MathJaxServerBase
    {
       
        public  string multiOutlineFomulas(List<String> stringArray)
        {
            result = pre;
            result_f = pre;
            result_w = pre;
            foreach (var i in stringArray) {
                string cache = outLineleftBrace + i + outLineRightBrace;
                result += cache;
                result_f += cache;
                result_w += cache;
            }
            return result_f + suf;
        }

        public  HtmlResult getServerHtmls(List<String> stringArray)
        {
            init();
            multiOutlineFomulas(stringArray);
            return new HtmlResult(result_f, result_w);
        }
    }
}
