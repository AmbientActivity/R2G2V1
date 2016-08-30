using System.Data.Entity;

namespace Keebee.AAT.DataAccess.Models
{
    public class KeebeeContentContext : DbContext
    {
        public KeebeeContentContext()
            : base("name=KeebeeContentEntities")
        {
        }

        // EF Model
        public DbSet<Domain.ContentView> db_ContentViews { get; set; }

        // DTOs
        public DbSet<Content> Contents { get; set; }
    }
}

