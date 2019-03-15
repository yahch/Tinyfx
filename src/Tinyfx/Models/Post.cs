using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tinyfx.Models
{
    public class Post
    {
        /// <summary>
        /// 编号
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }


        /// <summary>
        /// 是否公开可见
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// 正文
        /// </summary>
        public string Content { get; set; }

    }
}
