﻿using FileSystemProject.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Data.SqlClient;
using System.Text;
//using Swashbuckle.AspNetCore.SwaggerUI;
//using Microsoft.OpenApi.Models;


namespace FileSystemProject
{
    // Below is the class which helps in configuring the server
    public class FileSystemStartup
    {
        public IConfiguration Configuration { get; }

        public FileSystemStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        //We cannot modify name of this function
        public void ConfigureServices(IServiceCollection services)
        {
            ////uncomment to use swagger
            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My Api", Version = "v1" });
            //});
            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = (long)10 * 1024 * 1024 * 1024;
            });
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddTransient<IFileBlobRepository, FileBlobRepository>();
            services.AddTransient<IFileRepository, FileRepository>();
            services.AddTransient<IFolderRepository, FolderRepository>();
            services.AddTransient<IDbConnection>((sp) => new SqlConnection(Configuration.GetConnectionString("DefaultConnection")));
            services.AddControllers();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    //ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                };
            });
        }

        //We cannot modify name of this function. It helps in  setting up http request pipeline
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            ////uncomment to use swagger
            //app.UseSwagger();
            //app.UseSwaggerUI(c =>
            //{
            //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your api v1");
            //    c.ConfigObject.DisplayRequestDuration = true;
            //    c.DocExpansion(DocExpansion.None);
            //});
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.Use(async (context, next) =>
            {
                context.Features.Get<IHttpMaxRequestBodySizeFeature>().MaxRequestBodySize = (long)10 * 1024 * 1024 * 1024;
                await next.Invoke();
            });
            app.UseHttpsRedirection(); // Redirects http to https
            app.UseRouting();
            app.UseAuthentication();
            app.UseMiddleware<FileSystemMiddleware>();// Custom middleware
            app.UseAuthorization();// need to figure out
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllerRoute("api", "api/[controller]");
            //});
            app.UseCors("CorsPolicy");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
