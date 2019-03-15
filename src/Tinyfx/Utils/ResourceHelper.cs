using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tinyfx.Utils
{
    /// <summary>
    /// 资源辅助类
    /// </summary>
    public class ResourceHelper
    {
        /// <summary>
        /// 加载资源流
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        public static Stream LoadResource(string res)
        {
            Stream sm = Assembly.GetExecutingAssembly().GetManifestResourceStream("Tinyfx.Resources." + res);
            return sm;
        }

        /// <summary>
        /// 加载资源文本
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        public static string LoadStringResource(string res)
        {
            try
            {
                Stream sm = Assembly.GetExecutingAssembly().GetManifestResourceStream("Tinyfx.Resources." + res);
                StreamReader sr = new StreamReader(sm, Encoding.UTF8);
                string str = sr.ReadToEnd();
                sr.Close();
                sm.Close();
                return str;
            }
            catch(Exception)
            {
                return "";
            }
        }
    }
}
