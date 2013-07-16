using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.TinyIoc;
using Nancy.ViewEngines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CWSWeb
{
    public class ApplicationBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            nancyConventions.StaticContentsConventions.Add(Nancy.Embedded.Conventions.EmbeddedStaticContentConventionBuilder.AddDirectory("Static", GetType().Assembly));

            base.ConfigureConventions(nancyConventions);
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);
            Assembly assembly = GetType().Assembly;
            ResourceViewLocationProvider.RootNamespaces.Add(assembly, "CWSWeb.Views");
        }

        protected override NancyInternalConfiguration InternalConfiguration
        {
            get
            {
                return NancyInternalConfiguration.WithOverrides(OnConfigurationBuilder);
            }
        }

        void OnConfigurationBuilder(NancyInternalConfiguration x)
        {
            x.ViewLocationProvider = typeof (ResourceViewLocationProvider);
        }
    }
}
