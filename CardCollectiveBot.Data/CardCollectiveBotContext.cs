using CardCollectiveBot.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CardCollectiveBot.Data
{
    public class CardCollectiveBotContext : DbContext
    {
        public DbSet<Currency> Currency { get; set; }

        public CardCollectiveBotContext(DbContextOptions options) : base(options)
        {
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AddModifiedBy();
            return await base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            AddModifiedBy();
            return base.SaveChanges();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }

        private void AddModifiedBy()
        {
            var entities = ChangeTracker.Entries()
                .Where(x =>
                    x.Entity is ITrackedEntity
                    && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entity in entities)
            {
                ((ITrackedEntity)entity.Entity).DateModified = DateTime.Now;
            }
        }
    }
}
