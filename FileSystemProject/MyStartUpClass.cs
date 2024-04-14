using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
//using Swashbuckle.AspNetCore.SwaggerUI;
//using Microsoft.OpenApi.Models;


namespace FileSystemProject
{
    // Below is the class which helps in configuring the server
    public class MyStartUpClass
    {
        //We cannot modify name of this function
        public void ConfigureServices(IServiceCollection services)
        {
            ////uncomment to use swagger
            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My Api", Version = "v1" });
            //});
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });
            services.AddControllers();
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

            app.UseHttpsRedirection(); // Redirects http to https
            app.UseRouting();
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
