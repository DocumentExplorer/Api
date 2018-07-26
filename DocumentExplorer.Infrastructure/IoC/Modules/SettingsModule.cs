using System.Reflection;
using Autofac;
using DocumentExplorer.Infrastructure.BlobStorage;
using DocumentExplorer.Infrastructure.EF;
using DocumentExplorer.Infrastructure.Extensions;
using DocumentExplorer.Infrastructure.FileSystem;
using DocumentExplorer.Infrastructure.Mongo;
using DocumentExplorer.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;

namespace DocumentExplorer.Infrastructure.IoC.Modules
{
    public class SettingsModule : Autofac.Module
    {
        private readonly IConfiguration _configuration;

        public SettingsModule(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_configuration.GetSettings<JwtSettings>()).SingleInstance();
            builder.RegisterInstance(_configuration.GetSettings<GeneralSettings>()).SingleInstance();
            //builder.RegisterInstance(_configuration.GetSettings<SqlSettings>()).SingleInstance();
            builder.RegisterInstance(_configuration.GetSettings<MongoSettings>()).SingleInstance();
            builder.RegisterInstance(_configuration.GetSettings<BlobStorageSettings>()).SingleInstance();
            builder.RegisterInstance(_configuration.GetSettings<FileSystemSettings>()).SingleInstance();
        }
    }
}