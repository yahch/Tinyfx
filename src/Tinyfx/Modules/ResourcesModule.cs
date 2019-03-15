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
    public class ResourcesModule : Nancy.NancyModule
    {
        public ResourcesModule() : base("/resources")
        {
            Get("/{name}", _ =>
            {
                string name = _.name;
                if (name.Length < 1) return new NotFoundResponse();

                string mime = "application/octet-stream";
                if (name.EndsWith(".png")) mime = "image/png";
                else if (name.EndsWith(".jpg")) mime = "image/jpeg";

                var fontstream = ResourceHelper.LoadResource(name);
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
