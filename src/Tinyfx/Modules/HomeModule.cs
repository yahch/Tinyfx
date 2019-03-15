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
using Tinyfx.Configurations;
using Tinyfx.Cores;
using Tinyfx.Crypto;

namespace Tinyfx.Modules
{
    public class HomeModule : Nancy.NancyModule
    {
        private TinyfxPageRender _tinyfxPageRender;

        public HomeModule()
        {

            var config = TinyfxCore.Configuration;

            if (!config.IsSitePublic)
            {
                this.RequiresAuthentication();
            }

            _tinyfxPageRender = new Cores.TinyfxPageRender(config);

            Get("/", _ =>
            {
                return Response.AsText(_tinyfxPageRender.RenderPageOrPost(1, 0), "text/html");
            });

            Get("/page/{page}", _ =>
            {
                int page = 0;
                try
                {
                    page = _.page;
                }
                catch
                {
                    page = 1;
                }
                if (page < 1) page = 1;
                return Response.AsText(_tinyfxPageRender.RenderPageOrPost(page, 0), "text/html");
            });

            Get("/post/{post}", _ =>
            {
                long post = 0;
                try
                {
                    post = _.post;
                }
                catch
                {
                    post = 0;
                }
                return Response.AsText(_tinyfxPageRender.RenderPageOrPost(0, post), "text/html");
            });

            Get("/files/{filename}", _ =>
            {
                string filename = _.filename;
                if (filename == null || filename.Length < 1) return new NotFoundResponse();
                else
                {
                    string[] seqs = filename.Split(new char[] { '_' });
                    if (seqs.Length != 3)
                    {
                        return new NotFoundResponse();
                    }
                    else
                    {
                        string realfile = System.IO.Path.Combine(config.DataDirectory, TinyfxCore.IMAGE_UPLOAD_DIR, seqs[0], seqs[1], seqs[2]);
                        if (!String.IsNullOrEmpty(TinyfxCore.Configuration.DataDirectory))
                        {
                            realfile = System.IO.Path.Combine(config.DataDirectory, TinyfxCore.IMAGE_UPLOAD_DIR, seqs[0], seqs[1], seqs[2]);
                        }
                        if (System.IO.File.Exists(realfile))
                        {
                            string mime = "application/octet-stream";

                            string ext = System.IO.Path.GetExtension(filename);
                            if (!string.IsNullOrEmpty(ext))
                            {
                                if (TinyfxCore.Mime.ContainsKey(ext))
                                {
                                    mime = TinyfxCore.Mime[ext];
                                }
                            }

                            var fs = System.IO.File.OpenRead(realfile);

                            if (TinyfxCore.Configuration.Encryption)
                            {
                                var ms = new System.IO.MemoryStream();
                                Faes faes = new Faes();
                                faes.Decrypt(fs, ms);
                                ms.Seek(0, System.IO.SeekOrigin.Begin);

                                Nancy.Responses.StreamResponse streamResponse = new Nancy.Responses.StreamResponse(() => { return ms; }, mime);
                                if (ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".gif")
                                {
                                    return streamResponse.WithHeader("Cache-Control", "max-age=315360000");
                                }
                                else
                                {
                                    return streamResponse;
                                }
                            }
                            else
                            {
                                Nancy.Responses.StreamResponse streamResponse = new Nancy.Responses.StreamResponse(() => { return fs; }, mime);
                                if (ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".gif")
                                {
                                    return streamResponse.WithHeader("Cache-Control", "max-age=315360000");
                                }
                                else
                                {
                                    return streamResponse;
                                }
                            }
                        }
                        else
                        {
                            return new NotFoundResponse();
                        }
                    }
                }
            });

        }


    }
}
