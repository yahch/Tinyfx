using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tinyfx.Common
{
    /// <summary>
    /// 分页处理类
    /// </summary>
    public class PageNumber
    {
        /// <summary>
        /// 是否显示[首页]
        /// </summary>
        public bool ShowFirstPage { get; set; }

        /// <summary>
        /// 是否显示[末页]
        /// </summary>
        public bool ShowEndPage { get; set; }

        /// <summary>
        /// 翻页Url前缀
        /// </summary>
        public string UrlPrefix { get; set; }

        public PageNumber()
        {
            ShowFirstPage = true;
            ShowEndPage = true;
            UrlPrefix = "";
        }

        /// <summary>
        /// 获取分页，返回数据，如[["1","首页","/page/1"]]
        /// </summary>
        /// <param name="page">当前页</param>
        /// <param name="pages">总页数</param>
        /// <returns></returns>
        public List<string[]> GetPageNumbers(int page, int pages)
        {

            List<string[]> plists = new List<string[]>();

            //最多显示多少个页码  
            int _pageNum = 5;
            //当前页面小于1 则为1  
            page = page < 1 ? 1 : page;
            //当前页大于总页数 则为总页数  
            page = page > pages ? pages : page;
            //页数小当前页 则为当前页  
            pages = pages < page ? page : pages;

            //计算开始页  
            int _start = page - (int)Math.Floor((double)_pageNum / 2);
            _start = _start < 1 ? 1 : _start;
            //计算结束页  
            int _end = page + (int)Math.Floor((double)_pageNum / 2);
            _end = _end > pages ? pages : _end;

            //当前显示的页码个数不够最大页码数，在进行左右调整  
            int _curPageNum = _end - _start + 1;
            //左调整  
            if (_curPageNum < _pageNum && _start > 1)
            {
                _start = _start - (_pageNum - _curPageNum);
                _start = _start < 1 ? 1 : _start;
                _curPageNum = _end - _start + 1;
            }
            //右边调整  
            if (_curPageNum < _pageNum && _end < pages)
            {
                _end = _end + (_pageNum - _curPageNum);
                _end = _end > pages ? pages : _end;
            }

            if (ShowFirstPage)
                plists.Add(new string[] { "", "首页", string.IsNullOrEmpty(UrlPrefix) ? "" : UrlPrefix + "1" });

            if (page > 1)
            {
                plists.Add(new string[] { (page - 1).ToString(), "上页", string.IsNullOrEmpty(UrlPrefix) ? "" : UrlPrefix + (page - 1).ToString() });
            }
            for (int i = _start; i <= _end; i++)
            {
                plists.Add(new string[] { i.ToString(), i.ToString(), string.IsNullOrEmpty(UrlPrefix) ? "" : UrlPrefix + i.ToString() });
            }
            if (page < _end)
            {
                plists.Add(new string[] { (page + 1).ToString(), "下页" , string.IsNullOrEmpty(UrlPrefix) ? "" : UrlPrefix + (page + 1).ToString() });
            }

            if (ShowEndPage)
                plists.Add(new string[] { "", "末页", string.IsNullOrEmpty(UrlPrefix) ? "" : UrlPrefix + (pages).ToString() });

            return plists;
        }
    }
}
