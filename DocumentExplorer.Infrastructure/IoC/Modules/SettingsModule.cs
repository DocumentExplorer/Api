using System.Reflection;
using Autofac;
using DocumentExplorer.Infrastructure.Extensions;
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
        }
    }
}