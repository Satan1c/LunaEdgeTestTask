using System.Data.Common;
using Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Database.Contexts;

public class BaseContext(DbConnection connection) : DbContext
{
	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseNpgsql(connection);
		base.OnConfiguring(optionsBuilder);
	}

	private void ApplyDates()
	{
		var now = DateTime.UtcNow;

		foreach (var rawEntity in ChangeTracker.Entries())
		{
			switch (rawEntity.State)
			{
				case EntityState.Added:
					((BaseModel)rawEntity.Entity).CreatedAt = now;
					((BaseModel)rawEntity.Entity).UpdatedAt = now;
					break;

				case EntityState.Modified:
					((BaseModel)rawEntity.Entity).UpdatedAt = now;
					break;
			}
		}
	}

	public override int SaveChanges()
	{
		ApplyDates();
		return base.SaveChanges();
	}

	public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
	{
		ApplyDates();
		return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
	}

	public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		ApplyDates();
		return base.SaveChangesAsync(cancellationToken);
	}
}
