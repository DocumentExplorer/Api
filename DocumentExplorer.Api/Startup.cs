﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using DocumentExplorer.Infrastructure.Mappers;
using DocumentExplorer.Infrastructure.IoC.Modules;
using DocumentExplorer.Infrastructure.Services;
using DocumentExplorer.Core.Repositories;
using DocumentExplorer.Infrastructure.Repositories;

namespace DocumentExplorer.Api
{
    public class Startup
    {
        public IConfiguration ConfigurationRoot { get; }
        public IContainer ApplicationContainer { get; private set; }

        public Startup(IConfiguration configuration)
        {
            ConfigurationRoot = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {

            services.AddSingleton(AutoMapperConfig.Initialize());
            services.AddScoped<IUserService,UserService>();
            services.AddScoped<IEncrypter,Encrypter>();
            services.AddScoped<IUserRepository,InMemoryUserRepository>();
            services.AddMvc();

            var builder = new ContainerBuilder();
            builder.Populate(services);
            builder.RegisterModule(new SettingsModule(ConfigurationRoot));
            builder.RegisterModule<CommandModule>();
            builder.RegisterType<Encrypter>().As<IEncrypter>().SingleInstance();
            ApplicationContainer = builder.Build();

            return new AutofacServiceProvider(ApplicationContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            appLifetime.ApplicationStopped.Register(()=> ApplicationContainer.Dispose());
        }
    }
}
