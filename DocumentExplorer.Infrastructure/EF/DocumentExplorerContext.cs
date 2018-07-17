using DocumentExplorer.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace DocumentExplorer.Infrastructure.EF
{
    public class DocumentExplorerContext : DbContext
    {
        private readonly SqlSettings _sqlSettings;
        public DbSet<User> Users { get; set; }
        public DocumentExplorerContext(DbContextOptions<DocumentExplorerContext> options, 
            SqlSettings sqlSettings) : base(options)
        {
            _sqlSettings = sqlSettings;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(_sqlSettings.InMemory)
            {
                optionsBuilder.UseInMemoryDatabase("documentExplorer");
                return;
            }
            optionsBuilder.UseSqlServer(_sqlSettings.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var userBuilder = modelBuilder.Entity<User>();
            userBuilder.HasKey(x => x.Id);
        }
    }
}