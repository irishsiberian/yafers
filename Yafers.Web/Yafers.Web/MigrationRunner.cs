
using EvolveDb;
using EvolveDb.Migration;
using Microsoft.Data.SqlClient;
using Serilog;
using Yafers.Web.Domain;

namespace Yafers.Web
{
    public class MigrationRunner : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;

        public MigrationRunner(IConfiguration configuration, IServiceScopeFactory scopeFactory) 
        {
            _configuration = configuration;
            _scopeFactory = scopeFactory;
        }
        

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var scope = _scopeFactory.CreateScope())
            {
                var _commonConfiguration = scope.ServiceProvider.GetRequiredService<CommonConfiguration>();
                using (var connection = new SqlConnection(connectionString))
                {
                    var evolve = new Evolve(connection, Log.Information)
                    {
                        Locations = new[]
                            { Path.Combine("Migrations", "EvolveFiles"), Path.Combine("Migrations", "Repeatable") },
                        StartVersion = new MigrationVersion(_commonConfiguration.MigrationRunnerStartVersion),
                        CommandTimeout = 60, // seconds, default is 30
                        OutOfOrder = true
                    };

                    evolve.Migrate();
                }
            }
        }
    }
}
