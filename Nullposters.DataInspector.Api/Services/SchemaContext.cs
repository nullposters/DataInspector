using Microsoft.EntityFrameworkCore;

namespace Nullposters.DataInspector.Api.Services
{
    public class SchemaContext(string connectionString, string provider) : DbContext
    {
        private readonly string _connectionString = connectionString;
        private readonly string _provider = provider;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            switch (_provider)
            {
                case "SqlServer":
                    optionsBuilder.UseSqlServer(_connectionString);
                    break;
                case "PostgreSQL":
                    optionsBuilder.UseNpgsql(_connectionString);
                    break;
                case "MySQL":
                    optionsBuilder.UseMySql(_connectionString, new MySqlServerVersion(new Version(8, 0, 21)));
                    break;
                default:
                    throw new NotSupportedException($"The provider {_provider} is not supported.");
            }
        }
    }
}
