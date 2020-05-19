using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snip2latex.Model
{
    public static class Data
    {
        public static DataWrapperReturn wrapper(String json)
        {
            JsonSerializer jsonSerializer = JsonSerializer.Create();
            json = json.Replace("\\\\", "%%$$320");
            return jsonSerializer.Deserialize<DataWrapperReturn>(new JsonTextReader(new StringReader(json)));
        }
        public enum FomulaWordsSeparateOption { fomula, words, bothFomulaAndWords }

        public static void restoreWords(Formula f)
        {
            if (f.words != null) {
                f.words = f.words.Replace("%%$$320", "\\");
            }
        }
        public static void restoreWords(Words f)
        {
            if (f.words != null) {
                f.words = f.words.Replace("%%$$320", "\\");
            }
        }
        public static void restoreWords(DataWrapperReturn data)
        {
            if (data.words_result != null) {
                foreach (var i in data.words_result) {
                    restoreWords(i);
                }
            }
            if (data.formula_result != null) {
                foreach (var i in data.formula_result) {
                    restoreWords(i);
                }
            }

        }
    }
    public class Location
    {
        public int width { get; set; }
        public int top { get; set; }
        public int left { get; set; }
        public int height { get; set; }
    }

    /// <summary>
    /// 非必选项
    /// </summary>
    public class Formula
    {
        //words in img location
        public Location location { get; set; }
        //words
        public string words { get; set; }
    }


    /// <summary>
    /// 必选项
    /// </summary>
    public class Words
    {
        public Location location { get; set; }
        public string words { get; set; }
    }

    public class DataWrapperReturn
    {
        //uint64	唯一的log id，用于问题定位
        public ulong log_id { get; set; }
        //图像方向，当detect_direction=true时存在。
        public int direction { get; set; }
        //识别结果中的公式个数，表示formula_result的元素个数
        public int formula_result_num { get; set; }
        //单公式集合
        public List<Formula> formula_result { get; set; }
        //必选项
        public int words_result_num { get; set; }
        //必选项
        public List<Words> words_result { get; set; }

    }

    public class DataWrapperRequest
    {
        public string image { get; set; }
        public string recognize_granularity { get; set; }
        public bool detect_direction { get; set; }
        public bool disp_formula { get; set; }
    }
}
