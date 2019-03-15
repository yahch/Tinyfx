using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tinyfx.Common;
using Tinyfx.Configurations;
using Tinyfx.Crypto;
using Tinyfx.Models;
using Tinyfx.Utils;

namespace Tinyfx.Cores
{
    public class TinyfxPageRender
    {

        private TinyConfiguration _tinyConfiguration;

        private enum PagePost { NONE, PAGE, POST };

        private PressService _pressService;

        public TinyfxPageRender(TinyConfiguration configuration)
        {
            _tinyConfiguration = configuration;
            _pressService = new PressService();
        }

        /// <summary>
        /// 渲染文章列表或文章详情页
        /// </summary>
        /// <param name="obj"></param>
        public string RenderPageOrPost(int page, long post)
        {
            PagePost type = PagePost.NONE;

            #region 参数验证
            if (page < 0) page = 0;
            if (post < 0) post = 0;

            if (page == 0 && post == 0)
            {
                page = 1;
                post = 0;
            }
            if (page > 0 && post > 0)
            {
                post = 0;
            }
            if (page > 0)
            {
                type = PagePost.PAGE;
            }
            if (post > 0)
            {
                type = PagePost.POST;
            }
            #endregion

            string topTmpl = ResourceHelper.LoadStringResource("top.html");
            string bottomTmpl = ResourceHelper.LoadStringResource("bottom.html");

            string GetNoPostsHtml()
            {
                StringBuilder sbHtmlNoPosts = new StringBuilder();

                sbHtmlNoPosts.AppendLine(topTmpl.AsHtmlFromTemplate(new
                {
                    Configuration = _tinyConfiguration
                }));

                sbHtmlNoPosts.Append("<article class=\"post\"><div class=\"post__main echo\">你来到了没有知识的荒原...</div></article>");

                sbHtmlNoPosts.AppendLine(bottomTmpl.AsHtmlFromTemplate(new
                {
                    Configuration = _tinyConfiguration
                }));

                return sbHtmlNoPosts.ToString();
            }

            string GetPageHtml()
            {
                int maxPage = int.MaxValue;

                var pageData = _pressService.GetPostInPage(page, ref maxPage, false, _tinyConfiguration.PageSize);

                if (page > maxPage) page = maxPage;

                if (pageData == null || pageData.Count() < 1)
                {
                    return GetNoPostsHtml();
                }

                StringBuilder sbHtml = new StringBuilder();

                sbHtml.AppendLine(topTmpl.AsHtmlFromTemplate(new
                {
                    BrowserTitle = _tinyConfiguration.SiteName,
                    Description = _tinyConfiguration.SiteName,
                    Configuration = _tinyConfiguration
                }));

                string pageTmpl = ResourceHelper.LoadStringResource("page.html");
                string pagerTmpl = ResourceHelper.LoadStringResource("pager.html");

                foreach (var item in pageData)
                {
                    string content = item.Content.AsPlainTextFromMarkdown();

                    if (content.Length < 1) content = item.Title;
                    if (content.Length > (TinyfxCore.MAX_POST_DESCRIPTION_LENGTH + 10)) content = content.Substring(0, TinyfxCore.MAX_POST_DESCRIPTION_LENGTH) + "...";

                    sbHtml.AppendLine(pageTmpl.AsHtmlFromTemplate(new
                    {
                        Date = new DateTime(item.Id).ToString(),
                        Pid = item.Id,
                        Title = item.Title,
                        Content = content.AsHtmlEncode(),
                        Configuration = _tinyConfiguration
                    }));
                }

                string PrevPage = null;
                string NextPage = null;

                if (page - 1 > 0)
                {
                    PrevPage = (page - 1).ToString();
                }

                if (page + 1 <= maxPage)
                {
                    NextPage = (page + 1).ToString();
                }


                sbHtml.AppendLine(pagerTmpl.AsHtmlFromTemplate(new
                {
                    PrevPage = PrevPage,
                    NextPage = NextPage,
                    TotalPage = maxPage,
                    CurrentPage = page,
                    Configuration = _tinyConfiguration
                }));

                sbHtml.AppendLine(bottomTmpl.AsHtmlFromTemplate(new
                {
                    Configuration = _tinyConfiguration
                }));

                return sbHtml.ToString();
            }

            string GetPostHtml()
            {
                var postItem = _pressService.GetPostByID(post, false);

                if (postItem == null)
                {
                    return GetNoPostsHtml();
                }

                StringBuilder sbHtml = new StringBuilder();

                sbHtml.Append(topTmpl.AsHtmlFromTemplate(new
                {
                    BrowserTitle = postItem.Title + " - " + _tinyConfiguration.SiteName,
                    Description = postItem.Title,
                    SiteLink = "/",
                    Configuration = _tinyConfiguration
                }));

                string postTmpl = ResourceHelper.LoadStringResource("post.html");

                sbHtml.Append(postTmpl.AsHtmlFromTemplate(new
                {
                    Date = new DateTime(postItem.Id).ToString(),
                    Pid = postItem.Id,
                    Title = postItem.Title,
                    Content = postItem.Content.AsHtmlFromMarkdown(),
                    Configuration = _tinyConfiguration
                }));

                sbHtml.Append(bottomTmpl.AsHtmlFromTemplate(new
                {
                    SiteName = _tinyConfiguration.SiteName,
                    Configuration = _tinyConfiguration
                }));

                return sbHtml.ToString();
            }


            if (type == PagePost.PAGE)
            {
                return GetPageHtml();
            }
            else if (type == PagePost.POST)
            {
                return GetPostHtml();
            }
            else
            {
                return GetNoPostsHtml();
            }
        }

        public string RenderLogin(string method, bool isLogined, bool isPass)
        {
            string html = ResourceHelper.LoadStringResource("login.html");

            if (method == "GET")
            {
                return html.AsHtmlFromTemplate(new
                {
                    IsLogined = isLogined.ToString()
                });
            }
            else if (method == "POST")
            {
                if (!isPass)
                {
                    return html.AsHtmlFromTemplate(new { Error = "登录失败" });
                }
                else return null;
            }
            else return "";
        }

        public string RenderAdminDashboard()
        {
            string dashboard = ResourceHelper.LoadStringResource("dashboard.html").AsHtmlFromTemplate(new
            {
                AppVersion = TinyfxCore.TINYFX_VERSION,
                OsInfo = Environment.OSVersion.ToString(),
                MachineName = Environment.MachineName,
                Processor = Environment.ProcessorCount,
                UserName = Environment.UserName,
                PostsCount = _pressService.AllPosts.Count().ToString(),
                RunDays = ((int)Math.Ceiling((DateTime.Now - _tinyConfiguration.SiteBuildDate).TotalDays)).ToString(),
                Timezone = TimeZoneInfo.Local.ToString(),
                Configuration = _tinyConfiguration
            });

            string html = ResourceHelper.LoadStringResource("admin.html");
            return html.AsHtmlFromTemplate(new
            {
                Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                RenderBody = dashboard,
                Configuration = _tinyConfiguration
            });
        }

        public string RenderCreatePost(string method, long postId, string title, string content, bool isPublic)
        {
            string createPostHtml = ResourceHelper.LoadStringResource("createpost.html");
            string html = ResourceHelper.LoadStringResource("admin.html");

            if (method == "GET")
            {
                string[] pageData = null;
                List<string> plist = new List<string>();
                var post = _pressService.GetPostByID(postId, true);
                if (post != null)
                {
                    plist.Add(post.Id.ToString());
                    plist.Add(post.Title.AsHtmlEncode());
                    plist.Add(post.Content.AsHtmlEncode());
                    plist.Add(post.Visible.ToString());
                    pageData = plist.ToArray();
                }

                return html.AsHtmlFromTemplate(new
                {
                    Version = TinyfxCore.TINYFX_VERSION,
                    RenderBody = createPostHtml.AsHtmlFromTemplate(new
                    {
                        PageData = pageData,
                        Configuration = _tinyConfiguration
                    }),
                    Configuration = _tinyConfiguration
                });
            }
            else if (method == "POST")
            {
                if (title == null || title.Length < 1)
                {
                    return html.AsHtmlFromTemplate(new
                    {
                        Version = TinyfxCore.TINYFX_VERSION,
                        RenderBody = createPostHtml.AsHtmlFromTemplate(new
                        {
                            Error = "标题不能为空",
                            Configuration = _tinyConfiguration
                        }),
                        Configuration = _tinyConfiguration
                    });
                }

                if (content == null || content.Length < 1)
                {
                    return html.AsHtmlFromTemplate(new
                    {
                        Version = TinyfxCore.TINYFX_VERSION,
                        RenderBody = createPostHtml.AsHtmlFromTemplate(new
                        {
                            Error = "正文不能为空",
                            Configuration = _tinyConfiguration
                        }),
                        Configuration = _tinyConfiguration
                    });
                }

                Post post = new Post();
                post.Id = postId;
                post.Title = title;
                post.Content = content;
                post.Visible = isPublic;

                bool rs = _pressService.InsertOrUpdatePost(post);

                if (rs)
                {
                    return null;
                }
                else
                {
                    return html.AsHtmlFromTemplate(new
                    {
                        Version = TinyfxCore.TINYFX_VERSION,
                        RenderBody = createPostHtml.AsHtmlFromTemplate(new
                        {
                            Error = "添加新文章失败",
                            Configuration = _tinyConfiguration
                        }),
                        Configuration = _tinyConfiguration
                    });
                }
            }
            else
            {
                return "";
            }
        }

        public string RenderPostList(int page, string action, long pid)
        {
            if (page < 1) page = 1;

            string error = null;
            string success = null;

            if (action == "delete")
            {
                bool rs = _pressService.DeletePost(pid);
                if (!rs)
                {
                    error = "删除失败";
                }
                else
                {
                    success = "删除ID为" + pid + "文章成功";
                }
            }
            else if (action == "visible")
            {
                bool rs = _pressService.SwitchVisibleByID(pid);
                if (!rs)
                {
                    error = "设置失败";
                }
                else
                {
                    success = "设置成功";
                }
            }

            var allPosts = _pressService.ReadPostsFromDatabase();

            int pageSize = 8;

            int maxPage = 0;

            maxPage = (int)Math.Ceiling((double)allPosts.Count / pageSize);

            var tmpPageList = allPosts.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            List<Post> pageList = new List<Post>();

            if (!TinyfxCore.Configuration.Encryption)
            {
                pageList = tmpPageList;
            }
            else
            {
                Faes faes = new Faes();

                foreach (var item in tmpPageList)
                {
                    var citem = new Post();
                    citem.Id = item.Id;
                    citem.Visible = item.Visible;
                    citem.Content = faes.Decrypt(item.Content);
                    citem.Title = faes.Decrypt(item.Title);
                    pageList.Add(citem);
                }
            }

            List<string[]> pageData = new List<string[]>();

            for (int i = 0; i < pageList.Count(); i++)
            {
                pageData.Add(new string[] {
                    pageList[i].Id.ToString(),
                    pageList[i].Title ,
                    new DateTime(pageList[i].Id).ToString(),
                    pageList[i].Visible.ToString()
                });
            }

            List<string[]> naviData = new List<string[]>();

            if (page > maxPage) page = maxPage;

            if (maxPage >= 1)
            {
                PageNumber pageNumber = new PageNumber();
                pageNumber.UrlPrefix = "/admin/post-list?page=";
                naviData = pageNumber.GetPageNumbers(page, maxPage);
            }

            string postListHtmlTmpl = ResourceHelper.LoadStringResource("postlist.html");
            string adminHtmlTmpl = ResourceHelper.LoadStringResource("admin.html");
            return adminHtmlTmpl.AsHtmlFromTemplate(new
            {
                Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                RenderBody = postListHtmlTmpl.AsHtmlFromTemplate(new
                {
                    PageData = pageData.ToArray(),
                    NaviData = naviData,
                    CurrentPage = page.ToString(),
                    Error = error,
                    Success = success,
                    Configuration = _tinyConfiguration
                }),
                Configuration = _tinyConfiguration
            });
        }

        public string RenderChangePassword(string method, string username, string password, string repassword)
        {
            string changePasswordTmpl = ResourceHelper.LoadStringResource("changepassword.html");
            string adminHtmlTmpl = ResourceHelper.LoadStringResource("admin.html");

            if (method == "GET")
            {
                return adminHtmlTmpl.AsHtmlFromTemplate(new
                {
                    RenderBody = changePasswordTmpl.AsHtmlFromTemplate(new
                    {
                        Configuration = _tinyConfiguration
                    }),
                    Configuration = _tinyConfiguration
                });
            }

            Regex regUsername = new Regex(@"^[a-zA-Z][a-zA-Z0-9_]{4,15}$");
            Regex regPassword = new Regex(@"^[a-zA-Z]\w{5,17}$");
            bool valid = true;

            string error = null, success = null;

            if (!regUsername.IsMatch(username))
            {
                error = "请输入正确的用户名";
                valid = false;
            }

            if (!regPassword.IsMatch(password))
            {
                error = "请输入有效的密码";
                valid = false;
            }

            if (repassword != password)
            {
                error = "两次密码不一致";
                valid = false;
            }

            if (valid)
            {
                _tinyConfiguration.Username = username;
                _tinyConfiguration.Password = PBKDF2.Encrypt(password);
                if (TinyfxCore.UpdateConfig(_tinyConfiguration))
                {
                    success = "修改密码成功，下次登录生效。";
                }
                else
                {
                    error = "修改密码失败";
                }
            }

            return adminHtmlTmpl.AsHtmlFromTemplate(new
            {
                RenderBody = changePasswordTmpl.AsHtmlFromTemplate(new
                {
                    Configuration = _tinyConfiguration,
                    Error = error,
                    Success = success
                }),
                Configuration = _tinyConfiguration
            });

        }

        public string RenderVersionHistory()
        {
            string html = ResourceHelper.LoadStringResource("admin.html");
            return html.AsHtmlFromTemplate(new
            {
                Version = TinyfxCore.TINYFX_VERSION,
                RenderBody = ResourceHelper.LoadStringResource("versionhistory.html"),
                Configuration = _tinyConfiguration
            });
        }

        public string RenderCustomerError(string title, string error)
        {
            string html = ResourceHelper.LoadStringResource("customerror.html");
            return html.AsHtmlFromTemplate(new
            {
                Title = title,
                Error = error,
                Configuration = _tinyConfiguration
            });
        }
    }
}
