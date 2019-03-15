using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Tinyfx.Utils;

namespace Tinyfx.Cores
{
    public class PagesGenerater
    {

        static void EnsureFolders(params string[] dirs)
        {
            foreach (var dir in dirs)
            {
                if (!System.IO.Directory.Exists(dir)) System.IO.Directory.CreateDirectory(dir);
            }
        }

        static void WriteResource(string res, string filename)
        {
            try
            {
                using (var fStream = ResourceHelper.LoadResource(res))
                {
                    using (var fFileStream = System.IO.File.OpenWrite(filename))
                    {
                        fStream.Seek(0, SeekOrigin.Begin);
                        fStream.CopyTo(fFileStream);
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// 生成静态页
        /// </summary>
        /// <param name="outDataDir">输出目录</param>
        public static void GenerateStaticPages(string outDataDir, Action<string> logger)
        {
            if (!System.IO.Directory.Exists(outDataDir)) System.IO.Directory.CreateDirectory(outDataDir);

            void Log(string mesg)
            {
                logger(DateTime.Now.ToString() + " " + mesg);
            }


            // 目录
            string outPageDir = System.IO.Path.Combine(outDataDir, "page");
            string outPostDir = System.IO.Path.Combine(outDataDir, "post");
            string outUploaImagesDir = System.IO.Path.Combine(outDataDir, "files");
            string outStylesDir = System.IO.Path.Combine(outDataDir, "styles");
            string outScriptsDir = System.IO.Path.Combine(outDataDir, "scripts");
            string outResDir = System.IO.Path.Combine(outDataDir, "resources");
            string outFontsDir = System.IO.Path.Combine(outDataDir, "fonts");

            EnsureFolders(outPageDir, outPostDir, outUploaImagesDir, outStylesDir, outScriptsDir, outResDir, outFontsDir);

            var config = TinyfxCore.Configuration;
            Tinyfx.Cores.PressService pressService = new Tinyfx.Cores.PressService();
            Tinyfx.Cores.TinyfxPageRender render = new Tinyfx.Cores.TinyfxPageRender(config);

            if (!config.IsSitePublic)
            {
                Log("WARN:Not public site,can't generate static pages");
                return;
            }

            if (config.Encryption)
            {
                Log("WARN:Can't generate static pages for encrypted site");
                return;
            }

            #region resources
            WriteResource("images.logo.png", System.IO.Path.Combine(outResDir, "images.logo.png"));
            WriteResource("favicon.ico", System.IO.Path.Combine(outDataDir, "favicon.ico"));
            #endregion

            #region Page and Post

            int maxpage = 0;

            var pageData = pressService.GetPostInPage(1, ref maxpage, false, config.PageSize);

            for (int i = 0; i < maxpage; i++)
            {
                int page = (i + 1);

                Log("current write page " + page.ToString());

                int tmp = 0;

                pageData = pressService.GetPostInPage(page, ref tmp, false);

                string outCurrentPageDir = System.IO.Path.Combine(outPageDir, page.ToString());
                if (!System.IO.Directory.Exists(outCurrentPageDir)) System.IO.Directory.CreateDirectory(outCurrentPageDir);

                string pageHtml = render.RenderPageOrPost(page, 0);

                string outCurrentPageFilename = System.IO.Path.Combine(outCurrentPageDir, "index.html");
                System.IO.File.WriteAllText(outCurrentPageFilename, pageHtml, Encoding.UTF8);

                if (page == 1)
                {
                    string outIndexPageFilename = System.IO.Path.Combine(outDataDir, "index.html");
                    System.IO.File.Copy(outCurrentPageFilename, outIndexPageFilename, true);
                }

                if (pageData != null && pageData.Count > 0)
                {
                    foreach (var post in pageData)
                    {
                        long pid = post.Id;

                        Log("current write post " + pid.ToString());

                        string outCurrentPostDir = System.IO.Path.Combine(outPostDir, pid.ToString());
                        if (!System.IO.Directory.Exists(outCurrentPostDir)) System.IO.Directory.CreateDirectory(outCurrentPostDir);

                        string postHtml = render.RenderPageOrPost(0, pid);

                        string outCurrentPostFilename = System.IO.Path.Combine(outCurrentPostDir, "index.html");
                        System.IO.File.WriteAllText(outCurrentPostFilename, postHtml, Encoding.UTF8);

                    }
                }
            }

            #endregion

            #region Css And Scripts
            string[] csses = new string[]
            {
                "normalize.css",
                "screen.css",
                "font-awesome.min.css"
            };
            foreach (var css in csses)
            {
                string css1Filename = System.IO.Path.Combine(outStylesDir, css);
                string css1Text = ResourceHelper.LoadStringResource("css." + css);
                System.IO.File.WriteAllText(css1Filename, css1Text, Encoding.UTF8);
            }

            #endregion

            #region Scripts
            string[] scriptses = new string[]
            {
                 "particles.min.js"
            };
            foreach (var script in scriptses)
            {
                WriteResource("scripts." + script, System.IO.Path.Combine(outScriptsDir, script));
            }
            #endregion

            #region Fonts
            string[] fontes = new string[]
            {
                "fontawesome-webfont.eot",
                "fontawesome-webfont.svg",
                "fontawesome-webfont.ttf",
                "fontawesome-webfont.woff",
                "fontawesome-webfont.woff2",
                "FontAwesome.otf",
                "icons.svg",
                "icons.ttf",
                "icons.woff",
            };
            foreach (var font in fontes)
            {
                WriteResource("fonts." + font, System.IO.Path.Combine(outFontsDir, font));
            }

            #endregion

            #region Images

            string[] years = null;
            try
            {
                years = System.IO.Directory.GetDirectories(System.IO.Path.Combine(config.DataDirectory, "Uploads"));
            }
            catch
            {
                years = null;
            }
            if (years == null || years.Length < 1) return;
            foreach (var year in years)
            {
                string yearName = new System.IO.DirectoryInfo(year).Name;

                string[] months = System.IO.Directory.GetDirectories(year);

                foreach (var month in months)
                {
                    string monthName = new System.IO.DirectoryInfo(month).Name;

                    string[] imageFiles = System.IO.Directory.GetFiles(month);

                    foreach (var imageFile in imageFiles)
                    {
                        string newname = yearName + "_" + monthName + "_" + System.IO.Path.GetFileName(imageFile);
                        string newFullname = System.IO.Path.Combine(outUploaImagesDir, newname);
                        if (System.IO.File.Exists(newFullname)) continue;
                        Log("copy image " + newname);
                        System.IO.File.Copy(imageFile, newFullname);
                    }
                }
            }

            #endregion

            Log("finished ok!");
        }
    }
}
