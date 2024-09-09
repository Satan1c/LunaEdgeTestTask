using System.Data.Common;
using System.Runtime;
using Database.Contexts;
using Npgsql;

GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
GCSettings.LatencyMode                   = GCLatencyMode.Interactive;

namespace Database
{
	public static class Service
	{
		public static IServiceCollection AddDatabaseServices(this IServiceCollection services)
			=> services.AddEntityFrameworkNpgsql()
					   .AddSingleton<DbConnection>(new NpgsqlConnection(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")))
					   .AddDbContext<UsersContext>()
					   .AddDbContext<TasksContext>();
	}
}
