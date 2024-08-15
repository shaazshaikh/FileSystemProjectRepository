using FileSystemProject;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

public class Program
{
    // main method is the entry point of the application.
    public static void Main(string[] args)
    {
        FileSystemStartupServer(args).Build().Run();
    }


    // Below code will help in starting the server
    public static IHostBuilder FileSystemStartupServer(string[] args)
    {
        return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(builder => {
            builder.UseStartup<FileSystemStartup>();
            
            });
    }
}
