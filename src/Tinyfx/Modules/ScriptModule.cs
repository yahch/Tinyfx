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
    public class ScriptModule : Nancy.NancyModule
    {
        public ScriptModule() : base("/scripts")
        {
            Get("/{name}", _ =>
            {
                string scriptName = _.name;
                string text = ResourceHelper.LoadStringResource("scripts."+scriptName);
                return Response
                .AsText(text, "text/script")
                .WithHeader("Cache-Control", "max-age=315360000");
            });
        }
    }
}
