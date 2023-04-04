using golden.raspberry.awards.api.Application.Services;
using golden.raspberry.awards.api.Domain.Entities;
using golden.raspberry.awards.api.Domain.Interfaces;
using golden.raspberry.awards.api.Infra.Data.Context;
using golden.raspberry.awards.api.Infra.Data.Repository;
using golden.raspberry.awards.api.Infra.Settings;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace golden.raspberry.awards.api.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static void AddDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            AddDbContext(services, configuration);
            AddRepositories(services);
            AddServices(services);
        }

        private static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            var appSettings = configuration.LoadSettings<AppSettings>("AppSettings", services);
            var cnn = new SqliteConnection(appSettings.ConnectionString);
            cnn.Open();
            services.AddDbContext<AppDbContext>(o => o.UseSqlite(cnn));
        }

        private static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IBaseRepository<MovieAwardNomination>, BaseRepository<MovieAwardNomination>>();
        }

        private static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IBaseService<MovieAwardNomination>, BaseService<MovieAwardNomination>>();
            services.AddScoped<IProducerAwardService, ProducerAwardService>();
        }
    }
}
