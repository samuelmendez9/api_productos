using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using webapi_ENVIA.Data;
using Microsoft.AspNetCore.Authentication;
using webapi_ENVIA.Handler;
using Microsoft.AspNetCore.Authorization;
using webapi_ENVIA.Services;
using webapi.Data;

namespace webapi_ENVIA
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<PedidosContexto>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("ConexionSQL")));
            services.AddDbContext<PedidosContextoMK>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("ConexionSQLMK")));
            //services.AddDbContext<PedidosContexto>(options => options.UseSqlServer(Configuration.GetConnectionString("ConexionSQL")));
            services.AddAuthentication("BasicAuthentication").AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
            //services.AddAuthentication("BasicAuthentication").AddScheme("BasicAuthentication", null);
            services.AddScoped<IUserService, UserService>();
            services.AddControllers();

            

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();//Add
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });



            

        }
    }
}
