using golden.raspberry.awards.api.Infra.Data.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace golden.raspberry.awards.api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            AddMovieListData(host);

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static void AddMovieListData(IHost host)
        {
            using (var serviceScope = host.Services.CreateScope())
            {
                var appDbContext = serviceScope.ServiceProvider.GetService<AppDbContext>();
                appDbContext.Database.EnsureDeleted();
                appDbContext.Database.EnsureCreated();
                appDbContext.MovieAwardNominations.AddRange(SeedHelper.GetMovieAwardNominations(";", ".\\Resources\\movielist.csv"));
                appDbContext.SaveChanges();
            }
        }
    }
}
