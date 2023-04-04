using golden.raspberry.awards.api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace golden.raspberry.awards.api.Infra.Data.Context
{
    public class AppDbContext : DbContext
    {
        public DbSet<MovieAwardNomination> MovieAwardNominations { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}
