using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Tinyfx.Common;
using Tinyfx.Crypto;
using Tinyfx.Models;

namespace Tinyfx.Cores
{
    public static class TinyfxCore
    {
        /// <summary>
        /// 图片上传文件夹名
        /// </summary>
        public static readonly String IMAGE_UPLOAD_DIR = "Uploads";

        /// <summary>
        /// 默认序列化工具
        /// </summary>
        public static readonly ISerializor SERIALIZER = new XmlSerializor();

        /// <summary>
        /// 配置文件名
        /// </summary>
        public static readonly string TINY_CONFIG_FILE = "";

        /// <summary>
        /// 文章摘要最大长度
        /// </summary>
        public static readonly int MAX_POST_DESCRIPTION_LENGTH = 150;

        /// <summary>
        /// TiNYFX 版本
        /// </summary>
        public static readonly string TINYFX_VERSION = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

        /// <summary>
        /// MIME 映射
        /// </summary>
        public static Dictionary<string, string> Mime { get; private set; }

        /// <summary>
        /// 工作根目录，可以外部设置，可以通过 AutoDetectRoot 探测。
        /// 这是可执行程序的路径，或者 Web 程序的根路径，不应该是数据目录所在的路径
        /// </summary>
        public static string Root { get; set; }

        /// <summary>
        /// 数据库文件，可以外部设置
        /// </summary>
        public static string DbFilename { get; set; }

        /// <summary>
        /// 配置文件
        /// </summary>
        public static string ConfigFileName { get; private set; }

        /// <summary>
        /// 配置
        /// </summary>
        public static TinyConfiguration Configuration { get; private set; }

        /// <summary>
        /// 默认构造
        /// </summary>
        static TinyfxCore()
        {
            InitMimes();
        }

        /// <summary>
        /// 初始化，全局唯一入口点，在此之前 Root 属性必须指定，否则将自动探测，这也许不符合你的期望
        /// </summary>
        public static void Initialize()
        {
            if (string.IsNullOrEmpty(Root))
            {
                AutoDetectRoot();
            }

            ConfigFileName = Path.Combine(Root, "Tiny.config");
            LoadConfiguration();
            DbFilename = Path.Combine(Configuration.DataDirectory, "TinyData.xml");
        }

        /// <summary>
        /// 自动探测根目录
        /// </summary>
        public static void AutoDetectRoot()
        {
            try
            {
                var types = AppDomain.CurrentDomain.GetAssemblies()
                         .SelectMany(a => a.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IUnc))))
                         .ToArray();
                if (types.Count() < 1) throw new ArgumentNullException("没有指定根目录的实现");
                foreach (var v in types)
                {
                    var msds = v.GetMethods();
                    foreach (var msd in msds)
                    {
                        Root = (Activator.CreateInstance(v) as IUnc).GetRoot();
                        break;
                    }
                    break;
                }
            }
            catch (Exception)
            {
                Root = AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        /// <summary>
        /// 初始化 MIME
        /// </summary>
        static void InitMimes()
        {
            Mime = new Dictionary<string, string>();
            Mime.Add(".jpeg", "image/jpeg");
            Mime.Add(".jpg", "image/jpeg");
            Mime.Add(".png", "image/png");
            Mime.Add(".gif", "image/gif");
            Mime.Add(".pdf", "application/pdf");
            Mime.Add(".apk", "application/vnd.android.package-archive");
            Mime.Add(".zip", "application/x-zip-compressed");
            Mime.Add(".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            Mime.Add(".txt", "text/plain");
            Mime.Add(".7z", "application/x-7z-compressed");
            Mime.Add(".rar", "application/x-rar-compressed");
            Mime.Add(".docx", "application/vnd.openxmlformats-Mimeicedocument.wordprocessingml.document");
            Mime.Add(".pptx", "application/vnd.openxmlformats-Mimeicedocument.presentationml.presentation");
            Mime.Add(".mp3", "audio/mpeg");
            Mime.Add(".mp4", "video/mp4");
            Mime.Add(".flv", "video/x-flv");
        }

        /// <summary>
        /// 加载外部配置，如果没有，加载默认配置
        /// </summary>
        private static void LoadConfiguration()
        {
            if (string.IsNullOrEmpty(ConfigFileName))
            {
                throw new ArgumentException("配置文件路径不存在!");
            }
            // 判断是否有配置文件，如果有就读取
            if (File.Exists(ConfigFileName))
            {
                using (var fs = File.OpenRead(ConfigFileName))
                {
                    Configuration = SERIALIZER.DeserializorFromStream<TinyConfiguration>(fs);
                }
            }
            else
            {
                UpdateConfig(LoadDefaultConfiguration());
            }
        }

        /// <summary>
        /// 更新配置文件
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static bool UpdateConfig(TinyConfiguration config)
        {
            try
            {
                Configuration = config;
                var fs = File.OpenWrite(ConfigFileName);
                var xs = SERIALIZER.SerializorToStream(config);
                xs.CopyTo(fs);
                xs.Close();
                fs.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 默认站点配置
        /// </summary>
        /// <returns></returns>
        public static TinyConfiguration LoadDefaultConfiguration()
        {
            TinyConfiguration _tinyConfiguration = new TinyConfiguration();
            _tinyConfiguration = new TinyConfiguration();
            _tinyConfiguration.AuthExpireSeconds = 28800;
            _tinyConfiguration.IsSitePublic = true;
            _tinyConfiguration.NavigationLinks = new string[][]
            {
                new string[] { "/","HOME" },
                new string[] { "https://estermont.wordpress.com/", "DOWNLOAD" },
                new string[] { "https://github.com/yahch", "GITHUB" }
            };
            _tinyConfiguration.PageSize = 5;
            _tinyConfiguration.Password = PBKDF2.Encrypt("admin");
            _tinyConfiguration.Port = 6600;
            _tinyConfiguration.SiteBuildDate = new DateTime(2018, 3, 15);
            _tinyConfiguration.SiteName = "TiNY";
            _tinyConfiguration.Username = "admin";
            _tinyConfiguration.Encryption = false;
            _tinyConfiguration.LoginRetryTimeSpanSeconds = 30;
            _tinyConfiguration.DataDirectory = Root;

            return _tinyConfiguration;
        }

    }
}
