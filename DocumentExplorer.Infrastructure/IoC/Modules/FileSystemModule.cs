using Autofac;
using DocumentExplorer.Infrastructure.FileSystem;
using DocumentExplorer.Infrastructure.Repositories;
using System.Reflection;

namespace DocumentExplorer.Infrastructure.IoC.Modules
{
    public class FileSystemModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register((c, p) =>
            {
                var settings = c.Resolve<FileSystemSettings>();
                return new FileSystemContext(settings);

            }).InstancePerLifetimeScope();

            var assembly = typeof(MongoModule).GetTypeInfo().Assembly;

            builder.RegisterAssemblyTypes(assembly)
                .Where(x => x.IsAssignableTo<IFileSystemRepository>())
                .AsImplementedInterfaces().InstancePerLifetimeScope();
        }
    }
}
