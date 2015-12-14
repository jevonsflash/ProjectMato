using MusicMinkAppLayer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MusicMinkAppLayer.Helpers
{
    public class LRCSer
    {
        public static Gecime GecimeDeserializer(string jsonStr)
        {
            Gecime result = new Gecime();
            if (!string.IsNullOrEmpty(jsonStr))
            {
                result = JsonConvert.DeserializeObject<Gecime>(jsonStr);
                return result;
            }
            else
            {
                return null;
            }
        }


        public static LRCItem InitLrc(string lrcStr)
        {
            LRCItem lrc = new LRCItem();
            //if (lrcStr.StartsWith("[ti:"))
            //{
            //    lrc.Title = SplitInfo(lrcStr);
            //}
            //else if (lrcStr.StartsWith("[ar:"))
            //{
            //    lrc.Artist = SplitInfo(lrcStr);
            //}
            //else if (lrcStr.StartsWith("[al:"))
            //{
            //    lrc.Album = SplitInfo(lrcStr);
            //}
            //else if (lrcStr.StartsWith("[by:"))
            //{
            //    lrc.LrcBy = SplitInfo(lrcStr);
            //}
            //else if (lrcStr.StartsWith("[offset:"))
            //{
            //    lrc.Offset = SplitInfo(lrcStr);
            //}
            //else
            //{
            //    Regex regex = new Regex(@"\[([0-9.:]*)\]+(.*)", RegexOptions.Compiled);
            //    MatchCollection mc = regex.Matches(lrcStr);
            //    double time = TimeSpan.Parse("00:" + mc[0].Groups[1].Value).TotalSeconds;
            //    string word = mc[0].Groups[2].Value;
            //    lrc.LrcWord.Add(time, word);
            //}

            lrcStr = lrcStr.Replace("\r", "\n");
            String[] av = lrcStr.Split('\n');
            int i;
            for (i = 0; i < av.GetLength(0); i++)
            {
                if (av[i] != "")
                {
                    MatchCollection matchTime = Regex.Matches(av[i], @"(?<=\[)\d{2}:\d{2}\.\d{2}(?=\])");
                    Match matchContent = Regex.Match(av[i], @"(?<=\])(?!\[).*");

                    if (!string.IsNullOrEmpty(matchContent.ToString()))
                    {
                        foreach (var item in matchTime)
                        {
                            lrc.LrcWord.Add(Math.Round(stringToInterval(item.ToString()), 1), matchContent.ToString());
                        }
                    }
                    lrc.LrcWord = (from a in lrc.LrcWord
                                   orderby a.Key ascending
                                   select a).ToDictionary(k => k.Key, v => v.Value);

                }
            }



            return lrc;
        }



        /// <summary>
        /// 处理信息(私有方法)
        /// </summary>
        /// <param name="line"></param>
        /// <returns>返回基础信息</returns>
        public static string SplitInfo(string line)
        {
            return line.Substring(line.IndexOf(":") + 1).TrimEnd(']');
        }
        /// <summary>
        /// String转换Interval
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static double stringToInterval(String str)
        {
            try
            {
                double min = double.Parse(str.Split(':').GetValue(0).ToString());
                double sec = double.Parse(str.Split(':').GetValue(1).ToString());
                return min * 60.0 + sec;
            }
            catch
            {
                return uint.MaxValue;
            }
        }

        /// <summary>
        /// interval转换String
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        private static String intervalToString(double interval)
        {
            int min;
            float sec;
            min = (int)interval / 60;
            sec = (float)(interval - (float)min * 60.0);
            String smin = String.Format("{0:d2}", min);
            String ssec;
            ssec = String.Format("{0:00.0}", sec);
            return smin + ":" + ssec;
        }

    }
}
