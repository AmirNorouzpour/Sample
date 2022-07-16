using Common;
using Microsoft.EntityFrameworkCore;
using Model.Entities;

namespace Data
{
    public class DataContext : DbContext
    {

        public DataContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var entitiesAssembly = typeof(IEntity).Assembly;
            modelBuilder.RegisterAllEntities<IEntity>(entitiesAssembly);
            modelBuilder.AddSequentialGuidForIdConvention();
            modelBuilder.AddPluralizingTableNameConvention();

            //Seed
            modelBuilder.Entity<User>().HasData(new User { Id = 1, UserName = "Amir", IsActive = true, FirstName = "Amir", LastName = "Norouzpour", InsertDateTime = DateTime.Now, CreatorUserId = 1 });
        }

        public override int SaveChanges()
        {
            var result = 0;
            try
            {
                result = base.SaveChanges();
            }
            catch (DbUpdateConcurrencyException exception)
            {
                Console.WriteLine(exception);
            }

            return result;
        }

    }
}