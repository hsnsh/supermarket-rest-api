using HsNsH.SuperMarket.CatalogService.Application.Contracts.Services;
using HsNsH.SuperMarket.CatalogService.Application.Services;
using HsNsH.SuperMarket.CatalogService.Filters;
using Microsoft.OpenApi.Models;

namespace HsNsH.SuperMarket.CatalogService
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options =>
                {
                    options.Filters.Add(typeof(CustomExceptionFilterAttribute));
                })
                // Added for functional tests
                .AddApplicationPart(typeof(Startup).Assembly)
                .AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = true);

            services.AddTransient<ICategoryAppService, CategoryAppService>();

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Catalog Api", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog API v1"));
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}