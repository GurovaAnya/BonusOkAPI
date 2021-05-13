using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BonusOkAPI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using AutoMapper;

namespace BonusOkAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Startup));
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BonusOkAPI", Version = "v1" });
            });
            var connectionString =
               "server=eu-cdbr-west-01.cleardb.com; port=3306; database =heroku_e5b4723a7afaa1f; uid=b49715331e90f3; password="
               + Environment.GetEnvironmentVariable("DB_PASSWORD");
            services.AddDbContext<BonusOkContext>
                (item => item.UseMySQL(connectionString));
            //Environment.GetEnvironmentVariable("CLEARDB_CONNECTION_STRING")));
            //Configuration.GetConnectionString("DefaultConnection"))); // 

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BonusOkAPI v1"));
            }
            
            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
