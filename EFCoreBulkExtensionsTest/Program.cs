using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EFCore.BulkExtensions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EFCoreBulkExtensionsTest
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var keepAliveConnection = new SqliteConnection("DataSource=:memory:");
            keepAliveConnection.Open();

            var options = new DbContextOptionsBuilder<TestContext>()
                .UseSqlite(keepAliveConnection)
                .Options;
            
            var context = new TestContext(options);

            var entities = new List<TestEntity>();

            for (var i = 0; i < 100; i++)
            {
                entities.Add(new TestEntity()
                {
                    TextField = $"Entity {i}"
                });
            }

            await context.BulkInsertAsync(entities);
            await context.SaveChangesAsync();

            await keepAliveConnection.CloseAsync();

            Console.WriteLine("Done...");
            Console.ReadLine();
        }
    }

    public class TestContext : DbContext
    {
        public TestContext(DbContextOptions<TestContext> options)
            :base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TestEntity>();
        }
    }

    public class TestEntity
    {
        [Key]
        [Column("EntityId")]
        public int Id { get; set; }

        public string TextField { get; set; }
    }
}