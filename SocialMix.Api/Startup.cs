using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SocialMix.BusinessLayer.Managers;
using SocialMix.DataLayer;


namespace SocialMix.Api
{
    public class Startup
    {

        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
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
                        .WithExposedHeaders("Content-Disposition") // Add this line if needed
                        .SetIsOriginAllowedToAllowWildcardSubdomains() // Add this line if needed
                        .WithOrigins("http://localhost:4200")
                        .SetIsOriginAllowed((host) => true); // Add this line if needed
                });
            });

            services.AddSignalR();

            services.AddSingleton<DataLayer.IConfigurationProvider>(provider => new DataLayer.ConfigurationProvider(_configuration));

            
            //Business Layer Managers
            services.AddScoped<UserManager>();
            services.AddScoped<ChatMessageManager>();
 
            
            //Repository layer
            services.AddScoped<PersonRepository>();
            services.AddScoped<ChatMessageRepository>();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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
                endpoints.MapHub<ChatHub>("/api/chathub"); // Configure the SignalR hub endpoint
            });
            app.UseWebSockets();

        }
    }
}
