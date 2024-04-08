using FileSystemProject;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

public class Program
{
    // main method is the entry point of the application.
    public static void Main(string[] args)
    {
        FunctionWhichHelpsStartingtheServer(args).Build().Run();
    }


    // Below code will help in starting the server
    public static IHostBuilder FunctionWhichHelpsStartingtheServer(string[] args)
    {
        return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(builder => {
            builder.UseStartup<MyStartUpClass>();
            
            });
    }
}
