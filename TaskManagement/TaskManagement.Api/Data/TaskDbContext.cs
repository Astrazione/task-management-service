using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Models;

namespace TaskManagement.Api.Data
{
	public sealed class TaskDbContext(DbContextOptions<TaskDbContext> options) : DbContext(options)
	{
		public DbSet<TaskItem> Tasks { get; set; } = null!;

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<TaskItem>()
				.ToTable("Tasks", table => table.HasCheckConstraint("CK_Tasks_Status", "[Status] BETWEEN 0 AND 2"));
		}

		public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			var now = DateTimeOffset.Now;

			foreach (var entry in ChangeTracker.Entries<TaskItem>())
			{
				if (entry.State == EntityState.Added)
				{
					entry.Entity.CreatedAt = now;
					entry.Entity.UpdatedAt = now;
				}

				if (entry.State == EntityState.Modified)
					entry.Entity.UpdatedAt = now;
			}

			return base.SaveChangesAsync(cancellationToken);
		}
	}
}
