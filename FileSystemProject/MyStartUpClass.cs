using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
namespace FileSystemProject
{
    // Below is the class which helps in configuring the server
    public class MyStartUpClass
    {
        //We cannot modify name of this function
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }

        //We cannot modify name of this function
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if(env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();//
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();//
            });
        }
    }
}
