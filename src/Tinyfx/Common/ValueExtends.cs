using DotLiquid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Tinyfx.Utils;

namespace Tinyfx.Common
{
    public static class ValueExtends
    {
        /// <summary>
        /// 把Html转为纯文本，去掉所有html标记
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string AsPlainTextFromHtml(this string html)
        {
            return HtmlRemoval.StripTagsCharArray(html);
        }

        /// <summary>
        /// markdown 转换为 html
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string AsHtmlFromMarkdown(this string text)
        {
            CommonMark.CommonMarkSettings settings = CommonMark.CommonMarkSettings.Default.Clone();
            return CommonMark.CommonMarkConverter.Convert(text);
        }

        /// <summary>
        /// markdown 转换为纯文本
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string AsPlainTextFromMarkdown(this string text)
        {
            return AsPlainTextFromHtml(AsHtmlFromMarkdown(text));
        }

        /// <summary>
        /// 使用 HtmlEncode
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string AsHtmlEncode(this string text)
        {
            return System.Net.WebUtility.HtmlEncode(text);
        }

        /// <summary>
        /// MD5哈希值
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string AsMD5String(this string text)
        {
            byte[] result = Encoding.Default.GetBytes(text.Trim());
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            return BitConverter.ToString(output).Replace("-", "");
        }

        /// <summary>
        /// 将模板转换为html
        /// </summary>
        /// <param name="tmpl"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string AsHtmlFromTemplate(this string tmpl, object model)
        {
            string html = Template.Parse(tmpl).Render(Hash.FromAnonymousObject(model));
            return html;
        }

        /// <summary>
        /// 转换为 int 类型，默认 0
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static int AsInt(this string text)
        {
            int value = 0;
            int.TryParse(text, out value);
            return value;
        }

        /// <summary>
        /// 转换为 long 类型，默认 0
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static long AsLong(this string text)
        {
            long value = 0;
            long.TryParse(text, out value);
            return value;
        }

        /// <summary>
        /// 转换为查询集合，以分割 & = 的形式分割
        /// </summary>
        /// <param name="segments"></param>
        /// <returns></returns>
        public static Dictionary<string, string> AsQueryParameters(this string segments)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            if (segments.Length < 1) return dictionary;
            if (segments.StartsWith("?")) segments = segments.TrimStart('?');
            string[] qs = segments.Split('&');
            if (qs.Length < 1) return dictionary;
            foreach (string kv in qs)
            {
                string[] kvp = kv.Split('=');
                if (kvp != null && kvp.Length == 2)
                {
                    if (kvp[0].Length < 1 || kvp[1].Length < 1) continue;
                    string skey = System.Net.WebUtility.UrlDecode(kvp[0]);
                    string svalue = System.Net.WebUtility.UrlDecode(kvp[1]);
                    if (dictionary.ContainsKey(skey)) continue;
                    dictionary.Add(skey,svalue);
                }
            }
            return dictionary;
        }

        /// <summary>
        /// 转换为 Cookie 集合，以 = ; 的形式分割
        /// </summary>
        /// <param name="segments"></param>
        /// <returns></returns>
        public static Dictionary<string, string> AsCookieParameters(this string segments)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            if (segments.Length < 1) return dictionary;
            string[] qs = segments.Split(';');
            if (qs.Length < 1) return dictionary;
            foreach (string seq in qs)
            {
                if (seq.Length < 3) break;
                if (!seq.Contains("=")) break;
                int fstEqualSymbal = seq.IndexOf('=');
                string name = seq.Substring(0, fstEqualSymbal);
                string value = seq.Substring(fstEqualSymbal + 1, seq.Length - fstEqualSymbal - 1);
                if (dictionary.ContainsKey(name)) continue;
                dictionary.Add(name, value);
            }
            return dictionary;
        }
    }
}
