namespace Tinyfx.Bootstrappers
{
    using Nancy;
    using Nancy.Authentication.Forms;
    using Nancy.Bootstrapper;
    using Nancy.Diagnostics;
    using Nancy.Extensions;
    using Nancy.TinyIoc;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Tinyfx.Configurations;
    using Tinyfx.Models;
    using Tinyfx.Modules;
    using Tinyfx.Utils;

    public class TinyfxBootstrapper : DefaultNancyBootstrapper
    {
        /// <summary>
        /// Favicon 资源
        /// </summary>
        private byte[] favicon;

        protected override byte[] FavIcon
        {
            get { return this.favicon ?? (this.favicon = LoadFavIcon()); }
        }

        protected override IEnumerable<ModuleRegistration> Modules
        {
            get
            {
                var mcs = this.TypeCatalog
                     .GetTypesAssignableTo<INancyModule>(TypeResolveStrategies.All)
                     .NotOfType<DiagnosticModule>()
                     .Select(t => new ModuleRegistration(t))
                     .ToArray();
                return mcs;
            }
        }

        /// <summary>
        /// 加载 Favicon
        /// </summary>
        /// <returns></returns>
        private byte[] LoadFavIcon()
        {
            //TODO: remember to replace 'AssemblyName' with the prefix of the resource
            using (var resourceStream = ResourceHelper.LoadResource("favicon.ico"))
            {
                var memoryStream = new MemoryStream();
                resourceStream.CopyTo(memoryStream);
                return memoryStream.GetBuffer();
            }
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            // We don't call "base" here to prevent auto-discovery of
            // types/dependencies
        }

        protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
        {
            base.ConfigureRequestContainer(container, context);
            container.Register<IUserMapper, UserDatabase>();
        }

        /// <summary>
        /// 使用 Form 表单验证身份
        /// </summary>
        /// <param name="requestContainer"></param>
        /// <param name="pipelines"></param>
        /// <param name="context"></param>
        protected override void RequestStartup(TinyIoCContainer requestContainer, IPipelines pipelines, NancyContext context)
        {
            var formsAuthConfiguration =
                new FormsAuthenticationConfiguration()
                {
                    RedirectUrl = "/login",
                    UserMapper = requestContainer.Resolve<IUserMapper>(),
                };

            FormsAuthentication.Enable(pipelines, formsAuthConfiguration);
        }
    }
}