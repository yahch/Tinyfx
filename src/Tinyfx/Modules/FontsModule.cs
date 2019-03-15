using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Tinyfx.Cores;
using Tinyfx.Utils;

namespace Tinyfx.Modules
{
    public class FontsModule : Nancy.NancyModule
    {
        public FontsModule() : base("/fonts")
        {
            Get("/{name}", _ =>
            {
                string name = _.name;
                if (name.Length < 1) return new NotFoundResponse();

                string mime = "application/octet-stream";
                if (name.EndsWith(".eot")) mime = "application/vnd.ms-fontobject";
                else if (name.EndsWith(".svg")) mime = "image/svg+xml";
                else if (name.EndsWith(".ttf")) mime = "application/octet-stream";
                else if (name.EndsWith(".woff")) mime = "application/font-woff";
                else if (name.EndsWith(".woff2")) mime = "application/font-woff2";

                var fontstream = ResourceHelper.LoadResource("fonts." + name);
                if (fontstream == null)
                {
                    return new NotFoundResponse();
                }

                Nancy.Responses.StreamResponse streamResponse = new Nancy.Responses.StreamResponse(() => { return fontstream; }, mime);

                return streamResponse.WithHeader("Cache-Control", "max-age=315360000");
            });
        }
    }
}
