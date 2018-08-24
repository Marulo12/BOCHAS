﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using BOCHAS.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BOCHAS
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddSession();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options => { options.AccessDeniedPath = new PathString("/Usuarios/Index"); options.LoginPath = new PathString("/Usuarios/Index"); options.LogoutPath = new PathString("/Usuarios/Index"); });
            /*     .AddJwtBearer(jwtBearerOptions =>
                 {
                     jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters()
                     {
                         ValidateActor = true,
                         ValidateAudience = true,
                         ValidateLifetime = true,
                         ValidateIssuerSigningKey = true,
                          ValidIssuer = Configuration["ApiAuth:Issuer"],
                          ValidAudience = Configuration["ApiAuth:Audience"],
                         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["ApiAuth:SecretKey"]))
                     };
                 });*/
            services.Configure<CookiePolicyOptions>(options =>
            {               
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddSignalR();
            services.AddDbContext<BOCHASContext>();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                
            }
            
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSignalR();
            app.UseSession();
            app.UseAuthentication();        
            
           
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Presentacion}/{action=Presentacion}/{id?}");
            });
            Rotativa.AspNetCore.RotativaConfiguration.Setup(env);
        }
    }
}
