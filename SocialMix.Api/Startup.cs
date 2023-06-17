using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SocialMix.BusinessLayer.Managers;
using SocialMix.BusinessLayer.Managers.Security;
using SocialMix.DataLayer;

namespace SocialMix.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithOrigins("http://localhost:4200")
                        .AllowCredentials()
                        .WithExposedHeaders("Content-Disposition")
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .WithOrigins("http://localhost:4200")
                        .SetIsOriginAllowed((host) => true);
                });
            });

            services.AddSignalR();

            services.AddSingleton<SocialMix.DataLayer.IConfigurationProvider>(provider => new SocialMix.DataLayer.ConfigurationProvider(Configuration));

            // JWT Configuration
            string secretKey = Configuration.GetValue<string>("Jwt:SecretKey");
            string issuer = Configuration.GetValue<string>("Jwt:Issuer");
            string audience = Configuration.GetValue<string>("Jwt:Audience");
            services.AddSingleton<JwtTokenGeneratorManager>(new JwtTokenGeneratorManager(secretKey, issuer, audience));

            // Business Layer Managers
            services.AddScoped<UserManager>();
            services.AddScoped<ChatMessageManager>();
            services.AddScoped<UserLoginActivityManager>();

            // Repository Layer
            services.AddScoped<UserRepository>();
            services.AddScoped<ChatMessageRepository>();
            services.AddScoped<UserLoginActivityRepository>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("CorsPolicy");
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/api/chathub");
            });

            app.UseWebSockets();
        }
    }
}
