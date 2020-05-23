using snip2latex.Model.Baidu;

namespace snip2latex
{
    public class MathJaxServerForBaidu : MathJaxServerBase
    {

        public string multiOutlineFomulas(DataWrapperReturn data, BaiduData.FomulaWordsSeparateOption option)
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
            return result + suf;
        }
        
        public void multiOutlineFomulas(DataWrapperReturn data)
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
        
        public HtmlResult getServerHtmls(DataWrapperReturn WrapperReturnData)
        {
            init();
            multiOutlineFomulas(WrapperReturnData);
            return new HtmlResult(result_f, result_w);

        }
    }
}
