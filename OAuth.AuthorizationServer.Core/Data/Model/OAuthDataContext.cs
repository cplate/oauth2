using System.Data.Entity;

namespace OAuth.AuthorizationServer.Core.Data.Model
{
    // Core entity framework hook into the database
    public class OAuthDataContext : DbContext
    {
        public OAuthDataContext() : base("name=OAuth") {}

        public DbSet<Client> Clients { get; set; }
        public DbSet<Authorization> Authorizations { get; set; }
        public DbSet<Nonce> Nonces { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<SymmetricCryptoKey> SymmetricCryptoKeys { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Scope> Scopes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Name the many-many tables the way that makes more sense
            modelBuilder.Entity<Client>().HasMany(x => x.Scopes).WithMany(y => y.Clients).Map(m =>{
                m.ToTable("ClientScopes");
                m.MapLeftKey("ClientId");
                m.MapRightKey("ScopeId");
            });
            modelBuilder.Entity<Resource>().HasMany(x => x.Scopes).WithMany(y => y.Resources).Map(m =>
            {
                m.ToTable("ResourceScopes");
                m.MapLeftKey("ResourceId");
                m.MapRightKey("ScopeId");
            });
        }        
    }
}
