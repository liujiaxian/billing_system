using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Utility
{
    /// <summary>
    /// Html文件处理类
    /// </summary>
    public class HtmlHelper
    {
        /// <summary>
        /// 获取指定ID的标签内容
        /// </summary>
        /// <param name="html">HTML源码</param>
        /// <param name="id">标签ID</param>
        /// <returns></returns>
        public static string GetElementById(string html, string id)
        {
            string pattern = @"<([a-z]+)(?:(?!id)[^<>])*id=([""']?){0}\2[^>]*>(?>(?<o><\1[^>]*>)|(?<-o></\1>)|(?:(?!</?\1).))*(?(o)(?!))</\1>";

            pattern = string.Format(pattern, Regex.Escape(id));

            Match match = Regex.Match(html, pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            return match.Success ? match.Value : "";
        }
        /// <summary>
        /// 通过class属性获取对应标签集合
        /// </summary>
        /// <param name="html">HTML源码</param>
        /// <param name="className">class值</param>
        /// <returns></returns>
        public static string[] GetElementsByClass(string html, string className)
        {
            return GetElements(html, "", className);
        }
        /// <summary>
        /// 通过标签名获取标签集合
        /// </summary>
        /// <param name="html">HTML源码</param>
        /// <param name="tagName">标签名(如div)</param>
        /// <returns></returns>
        public static string[] GetElementsByTagName(string html, string tagName)
        {
            return GetElements(html, tagName, "");
        }
        /// <summary>
        /// 通过同时指定标签名+class值获取标签集合
        /// </summary>
        /// <param name="html">HTML源码</param>
        /// <param name="tagName">标签名</param>
        /// <param name="className">class值</param>
        /// <returns></returns>
        public static string[] GetElementsByTagAndClass(string html, string tagName, string className)
        {
            return GetElements(html, tagName, className);
        }


        private static string[] GetElements(string html, string tagName, string className)
        {
            string pattern = "";
            if (tagName != "" && className != "")
            {
                pattern = @"<({0})(?:(?!class)[^<>])*class=([""']?){1}\2[^>]*>(?>(?<o><\1[^>]*>)|(?<-o></\1>)|(?:(?!</?\1).))*(?(o)(?!))</\1>";
                pattern = string.Format(pattern, Regex.Escape(tagName), Regex.Escape(className));
            }
            else if (tagName != "")
            {
                pattern = @"<({0})(?:[^<>])*>(?>(?<o><\1[^>]*>)|(?<-o></\1>)|(?:(?!</?\1).))*(?(o)(?!))</\1>";
                pattern = string.Format(pattern, Regex.Escape(tagName));
            }
            else if (className != "")
            {
                pattern = @"<([a-z]+)(?:(?!class)[^<>])*class=([""']?){0}\2[^>]*>(?>(?<o><\1[^>]*>)|(?<-o></\1>)|(?:(?!</?\1).))*(?(o)(?!))</\1>";
                pattern = string.Format(pattern, Regex.Escape(className));
            }
            if (pattern == "")
            {
                return new string[] { };
            }
            List<string> list = new List<string>();

            Regex reg = new Regex(pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            Match match = reg.Match(html);

            while (match.Success)
            {
                list.Add(match.Value);

                match = reg.Match(html, match.Index + match.Length);
            }
            return list.ToArray();
        }

        #region 根据出生日期计算年龄
        public static int CalculateAgeCorrect(DateTime birthDate, DateTime now)
        {
            int age = now.Year - birthDate.Year;
            if (now.Month < birthDate.Month || (now.Month == birthDate.Month && now.Day < birthDate.Day)) age--;
            return age;
        }
        #endregion

        public static string ToMoneyStr(decimal money)
        {
            string str = null;
            if (-money > 1000)
            {
                str = Math.Round(money / 1000, 1) + "k";
            }
            else if (-money > 10000)
            {
                str = Math.Round(money / 10000, 1) + "w";
            }
            else
            {
                str = money.ToString();
            }

            return str;
        }
    }
}
