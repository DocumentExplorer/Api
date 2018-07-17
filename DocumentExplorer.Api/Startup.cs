using System;
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
using DocumentExplorer.Infrastructure.EF;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using DocumentExplorer.Infrastructure.Settings;
using Microsoft.AspNetCore.Http;

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
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = ConfigurationRoot["jwt:issuer"],
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigurationRoot["jwt:key"])),
                SaveSigninToken = true
            };

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = tokenValidationParameters;
            });



            services.AddSingleton(AutoMapperConfig.Initialize());
            services.AddScoped<IUserService,UserService>();
            services.AddScoped<IEncrypter,Encrypter>();
            services.AddMemoryCache();
            services.AddAuthorization(x => x.AddPolicy("admin", p=>p.RequireRole("admin")));
            services.AddAuthorization(x => x.AddPolicy("user", p=>p.RequireRole("user")));
            services.AddTransient<TokenManagerMiddleware>();
            services.AddTransient<ITokenManager,TokenManager>();
            services.AddSingleton<IHttpContextAccessor,HttpContextAccessor>();
            services.AddMvc();
            services.AddEntityFrameworkSqlServer().AddEntityFrameworkInMemoryDatabase()
                .AddDbContext<DocumentExplorerContext>();

            var builder = new ContainerBuilder();
            builder.Populate(services);
            builder.RegisterModule(new SettingsModule(ConfigurationRoot));
            builder.RegisterModule<CommandModule>();
            builder.RegisterModule<SqlModule>();
            builder.RegisterType<Encrypter>().As<IEncrypter>().SingleInstance();
            builder.RegisterType<JwtHandler>().As<IJwtHandler>().SingleInstance();
            builder.RegisterType<DataInitializer>().As<IDataInitializer>().SingleInstance();
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
            app.UseMiddleware<TokenManagerMiddleware>();
            app.UseAuthentication();

            var generalSettings = app.ApplicationServices.GetService<GeneralSettings>();
            if(generalSettings.DataInitialize)
            {
                var dataInitilizer = app.ApplicationServices.GetService<IDataInitializer>();
                dataInitilizer.SeedAsync();
            }
            app.UseMvc();
            appLifetime.ApplicationStopped.Register(()=> ApplicationContainer.Dispose());
        }
    }
}
