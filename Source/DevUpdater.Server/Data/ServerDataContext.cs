using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevUpdater.Server.Data
{
    public class ServerDataContext : DbContext
    {
        public ServerDataContext()
            : base("DevUpdaterServer")
        {
            Database.SetInitializer(new DbInitializer());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            
            modelBuilder.Entity<PendingCertificate>();

            modelBuilder.Entity<Client>().HasMany<ClientGroup>(c => c.ClientGroups).WithMany(c => c.Clients)
                .Map(c => c.ToTable("ClientToClientGroup").MapLeftKey("ClientId").MapRightKey("ClientGroupId"));
            
            modelBuilder.Entity<ClientGroup>();

            modelBuilder.Entity<Repository>().HasMany<ClientGroup>(c => c.ClientGroups).WithMany(c => c.Repositories)
                .Map(c => c.ToTable("RepositoryToClientGroup").MapLeftKey("RepositoryId").MapRightKey("ClientGroupId"));
        }

        public DbSet<Repository> Repositories { get { return Set<Repository>(); } }

        public DbSet<PendingCertificate> PendingCertificates { get { return Set<PendingCertificate>(); } }

        public DbSet<ClientGroup> ClientGroups { get { return Set<ClientGroup>(); } }

        public DbSet<Client> Clients { get { return Set<Client>(); } }

    }
}
