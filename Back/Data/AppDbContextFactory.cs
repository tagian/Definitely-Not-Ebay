using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using DefNotEbay_API.Data;

namespace DefNotEbay_API.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();


            optionsBuilder.UseSqlServer("Server=localhost;Database=DefNotEbay_DB;Trusted_Connection=True;TrustServerCertificate=True");

            return new AppDbContext(optionsBuilder.Options);
        }
    }

}
