using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tinyfx.Models
{
    /// <summary>
    /// 配置类
    /// </summary>
    public class TinyConfiguration : DotLiquid.ILiquidizable
    {
        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 站点名
        /// </summary>
        public string SiteName { get; set; }

        /// <summary>
        /// 是否公开访问
        /// </summary>
        public bool IsSitePublic { get; set; }

        /// <summary>
        /// 分页大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 导航栏
        /// </summary>
        public string[][] NavigationLinks { get; set; }

        /// <summary>
        /// 站点创立日期
        /// </summary>
        public DateTime SiteBuildDate { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 登录过期时间
        /// </summary>
        public int AuthExpireSeconds { get; set; }

        /// <summary>
        /// 数据是否加密
        /// </summary>
        public bool Encryption { get; set; }

        /// <summary>
        /// 数据目录
        /// </summary>
        public string DataDirectory { get; set; }

        /// <summary>
        /// 登录失败间隔秒数
        /// </summary>
        public int LoginRetryTimeSpanSeconds { get; set; }

        /// <summary>
        /// 转换为 DotLiquid 的模板数据类型对象
        /// </summary>
        /// <returns></returns>
        public object ToLiquid()
        {
            return new
            {
                Port,
                SiteName,
                IsSitePublic,
                PageSize,
                NavigationLinks,
                SiteBuildDate,
                Username,
                Password,
                AuthExpireSeconds,
                Encryption,
                DataDirectory,
                LoginRetryTimeSpanSeconds
            };
        }
    }
}
