using System.Reflection;
using Autofac;
using DocumentExplorer.Infrastructure.Services;

namespace DocumentExplorer.Infrastructure.IoC.Modules
{
    public class ServiceModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {

            var assembly = typeof(MongoModule).GetTypeInfo().Assembly;

            builder.RegisterAssemblyTypes(assembly)
                .Where(x => x.IsAssignableTo<IService>())
                .AsImplementedInterfaces().InstancePerLifetimeScope();

        }
    }
}