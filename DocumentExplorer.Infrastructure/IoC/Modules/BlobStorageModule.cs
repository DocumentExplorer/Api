using Autofac;
using DocumentExplorer.Infrastructure.BlobStorage;
using DocumentExplorer.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DocumentExplorer.Infrastructure.IoC.Modules
{
    public class BlobStorageModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register((c, p) =>
            {
                var settings = c.Resolve<BlobStorageSettings>();
                return new BlobStorageContext(settings);

            }).SingleInstance();

            var assembly = typeof(MongoModule).GetTypeInfo().Assembly;

            builder.RegisterAssemblyTypes(assembly)
                .Where(x => x.IsAssignableTo<IBlobStorageRepository>())
                .AsImplementedInterfaces().InstancePerLifetimeScope();
        }
    }
}
