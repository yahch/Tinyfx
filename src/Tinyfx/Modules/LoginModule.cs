using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Extensions;
using Nancy.Security;
using Tinyfx.Common;
using Tinyfx.Configurations;
using Tinyfx.Cores;
using Tinyfx.Utils;

namespace Tinyfx.Modules
{
    public class LoginModule : Nancy.NancyModule
    {

        private TinyfxPageRender _tinyfxPageRender;

        public LoginModule()
        {

            var config = TinyfxCore.Configuration;

            _tinyfxPageRender = new TinyfxPageRender(config);

            Get("/login", _ =>
            {
                bool islogin = false;
                if (Context.CurrentUser != null && Context.CurrentUser.Identity != null && Context.CurrentUser.Identity.Name != null && Context.CurrentUser.Identity.Name.Length > 0) islogin = true;
                return Response.AsText(_tinyfxPageRender.RenderLogin("GET", islogin, false), "text/html");
            });

            Post("/login", _ =>
            {
                DateTime _lastLoginFail = DateTime.MinValue;
                DateTime.TryParse(TStorage.GetInstance()["_last_login_fail"] + "", out _lastLoginFail);

                double tspan = (DateTime.Now - _lastLoginFail).TotalSeconds;

                if (tspan < TinyfxCore.Configuration.LoginRetryTimeSpanSeconds)
                {
                    return Response.AsText(ResourceHelper.LoadStringResource("login.html").AsHtmlFromTemplate(new
                    {
                        Error = "拒绝登录，请" + (TinyfxCore.Configuration.LoginRetryTimeSpanSeconds - (int)tspan) + "秒后重试!"
                    }), "text/html");
                }
                else
                {
                    string username = Request.Form.username;
                    string password = Request.Form.password;

                    LogHelper.WriteLog(LogHelper.LogType.INFO, "HTTP POST /login username=" + username + ",password=" + password, null);

                    var uobj = UserDatabase.ValidateUser(username, password);
                    if (uobj.HasValue)
                    {
                        TStorage.GetInstance()["_POSTS"] = null;
                        return this.LoginAndRedirect(uobj.Value, DateTime.Now.AddSeconds(config.AuthExpireSeconds));
                    }
                    else
                    {
                        TStorage.GetInstance()["_last_login_fail"] = DateTime.Now;
                        return Response.AsText(_tinyfxPageRender.RenderLogin("POST", false, false), "text/html");
                    }
                }
            });

            Get("/logout", _ =>
             {
                 TStorage.GetInstance()["_POSTS"] = null;
                 return this.LogoutAndRedirect("/login");
             });

        }


    }
}
