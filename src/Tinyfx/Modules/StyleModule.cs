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
    public class StyleModule : Nancy.NancyModule
    {
        public StyleModule() : base("/styles")
        {
            Get("/{name}", _ =>
            {
                string scriptName = _.name;
                string text = ResourceHelper.LoadStringResource("css."+scriptName);
                return Response
                .AsText(text, "text/css")
                .WithHeader("Cache-Control", "max-age=315360000");
            });
        }
    }
}
