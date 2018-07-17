using System.Reflection;
using Autofac;
using DocumentExplorer.Infrastructure.Repositories;

namespace DocumentExplorer.Infrastructure.IoC.Modules
{
    public class SqlModule : Autofac.Module
    {
        protected override void Load(Autofac.ContainerBuilder builder)
        {
            var assembly = typeof(SqlModule).GetTypeInfo().Assembly;

            builder.RegisterAssemblyTypes(assembly).Where(x => x.IsAssignableTo<ISqlRepository>())
                .AsImplementedInterfaces().InstancePerLifetimeScope();
        }
    }
}