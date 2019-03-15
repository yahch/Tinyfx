using Nancy;
using Nancy.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tinyfx.Common;
using Tinyfx.Configurations;
using Tinyfx.Cores;
using Tinyfx.Crypto;
using Tinyfx.Utils;

namespace Tinyfx.Modules
{
    public class AdminModule : Nancy.NancyModule
    {
        private TinyfxPageRender _tinyfxPageRender;

        public AdminModule() : base("/admin")
        {
            _tinyfxPageRender = new Cores.TinyfxPageRender(TinyfxCore.Configuration);

            this.RequiresAuthentication();

            // 全局文本替换
            Get("/global-replace", _ => {

                bool enable = false;

                if (enable)
                {
                    // 原始
                    string srcText = "/images/";
                    // 替换为
                    string dstTest = "/files/";

                    int bb = 0;
                    var ps = new PressService();
                    var data = ps.AllPosts;
                    var newpp = new List<Models.Post>();
                    foreach (var item in data)
                    {
                        var pc = item;
                        string ori = new Faes().Decrypt(item.Content);
                        int poc = ori.IndexOf(srcText);
                        if (poc >= 0)
                        {
                            string mr = ori.Replace(srcText, dstTest);
                            pc.Content = new Faes().Encrypt(mr);
                            bb++;
                        }

                        newpp.Add(pc);
                    }
                    string xml = new XmlSerializor().SerializorToString(newpp);
                    return xml;
                }
                else
                {
                    return new NotFoundResponse();
                }

            });


            Get("/", _ =>
           {
               return Response.AsText(_tinyfxPageRender.RenderAdminDashboard(), "text/html");
           });

            Get("/dashboard", _ =>
           {
               return Response.AsText(_tinyfxPageRender.RenderAdminDashboard(), "text/html");
           });

            Get("/edit-post", _ =>
            {
                long pid = 0;
                string pidstr = Request.Query.Pid;
                pid = pidstr.AsLong();
                return Response.AsText(_tinyfxPageRender.RenderCreatePost("GET", pid, null, null, false), "text/html");
            });

            Post("/edit-post", _ =>
           {
               long pid = 0;
               string pidstr = Request.Query.Pid;
               pid = pidstr.AsLong();

               string title = Request.Form.title;
               string content = Request.Form.content;
               bool isPublic = false;
               if (Request.Form.isPublic != null && Request.Form.isPublic == "on") isPublic = true;

               string html = _tinyfxPageRender.RenderCreatePost("POST", pid, title, content, isPublic);

               if (html != null)
                   return Response.AsText(html, "text/html");
               else return Response.AsRedirect(this.ModulePath + "/post-list");
           });

            Get("/post-list", _ =>
            {
                string pageStr = Request.Query.page;
                string pidStr = Request.Query.pid;
                string action = Request.Query.action + "";

                int page = pageStr.AsInt();
                long pid = pidStr.AsLong();

                return Response.AsText(_tinyfxPageRender.RenderPostList(page, action, pid), "text/html");
            });

            Post("/upload", _ =>
            {
                var faes = new Faes();
                var config = TinyfxCore.Configuration;
                var file = this.Request.Files.FirstOrDefault();
                if (file != null)
                {
                    try
                    {
                        DateTime now = DateTime.Now;

                        string ext = System.IO.Path.GetExtension(file.Name).ToLower();
                        if (!TinyfxCore.Mime.ContainsKey(ext))
                        {
                            return Response.AsJson(new { error = 3, url = "" });
                        }
                        string filename = now.Ticks.ToString() + ext;
                        string year = now.Year.ToString();
                        string month = now.Month.ToString();
                        string dir = System.IO.Path.Combine(config.DataDirectory, TinyfxCore.IMAGE_UPLOAD_DIR, year, month);
                        if (!string.IsNullOrEmpty(TinyfxCore.Configuration.DataDirectory))
                        {
                            dir = System.IO.Path.Combine(TinyfxCore.Configuration.DataDirectory, TinyfxCore.IMAGE_UPLOAD_DIR, year, month);
                        }
                        string fullname = System.IO.Path.Combine(dir, filename);
                        if (!System.IO.Directory.Exists(dir))
                        {
                            System.IO.Directory.CreateDirectory(dir);
                        }

                        if (TinyfxCore.Configuration.Encryption)
                        {
                            System.IO.MemoryStream ms = new System.IO.MemoryStream();
                            faes.Encrypt(file.Value, ms);
                            ms.Seek(0, System.IO.SeekOrigin.Begin);
                            using (var fs = System.IO.File.Open(fullname, System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite))
                            {
                                ms.CopyTo(fs);
                            }
                        }
                        else
                        {
                            using (var fs = System.IO.File.Open(fullname, System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite))
                            {
                                file.Value.CopyTo(fs);
                            }
                        }

                        string url = "/files/" + year + "_" + month + "_" + filename;
                        return Response.AsJson(new { error = 0, url = url });
                    }
                    catch (Exception)
                    {
                        return Response.AsJson(new { error = 1, url = "" });
                    }
                }
                else
                {
                    return Response.AsJson(new { error = 2, url = "" });
                }
            });

            Get("/change-password", _ =>
            {
                LogHelper.WriteLog(LogHelper.LogType.INFO, "HTTP GET /change-password", null);
                return Response.AsText(_tinyfxPageRender.RenderChangePassword(Request.Method, null, null, null), "text/html");
            });

            Post("/change-password", _ =>
            {
                LogHelper.WriteLog(LogHelper.LogType.INFO, "HTTP POST /change-password", null);

                string username = Request.Form.username + "";
                string password = Request.Form.password + "";
                string repassword = Request.Form.repassword + "";

                return Response.AsText(_tinyfxPageRender.RenderChangePassword(Request.Method, username, password, repassword), "text/html");
            });
        }
    }
}
