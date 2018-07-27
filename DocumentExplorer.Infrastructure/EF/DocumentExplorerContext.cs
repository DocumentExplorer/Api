using DocumentExplorer.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace DocumentExplorer.Infrastructure.EF
{
    public class DocumentExplorerContext : DbContext
    {
        private readonly SqlSettings _sqlSettings;
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Permissions> Permissions {get; set;}
        public DbSet<Log> Logs {get; set;}
        public DbSet<File> Files {get; set;}
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
            var orderBuilder = modelBuilder.Entity<Order>();
            orderBuilder.HasKey(x => x.Id);
            var permissionsBuilder = modelBuilder.Entity<Permissions>();
            permissionsBuilder.HasKey(x => x.Id);
            var logsBuilder = modelBuilder.Entity<Log>();
            logsBuilder.HasKey(x=> x.Id);
            var filesBuilder = modelBuilder.Entity<File>();
            filesBuilder.HasKey(x=>x.Id);

        }
    }
}