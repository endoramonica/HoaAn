using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace VietCommerce.Data.Context
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // 🔹 Đọc file appsettings.json
            IConfigurationRoot configuration = new ConfigurationBuilder()
             .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../VietCommerce.Api")) // trỏ về thư mục Api
             .AddJsonFile("appsettings.json")
             .Build();


            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
