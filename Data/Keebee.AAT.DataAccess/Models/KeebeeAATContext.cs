using System.Data.Entity;

namespace Keebee.AAT.DataAccess.Models
{
    public class KeebeeAATContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public KeebeeAATContext() : base("name=KeebeeAATContext")
        {
        }

        public DbSet<Config> Configurations { get; set; }

        public DbSet<ConfigDetail> ConfigurationDetails { get; set; }

        public DbSet<PhidgetType> PhidgetTypes { get; set; }

        public DbSet<PhidgetStyleType> PhidgetStyleTypes { get; set; }

        public DbSet<ResponseType> ResponseTypes { get; set; }

        public DbSet<ResponseTypeCategory> ResponseTypeCategories { get; set; }

        public DbSet<ActivityEventLog> ActivityEventLogs { get; set; }

        public DbSet<GameEventLog> GameEventLogs { get; set; }

        public DbSet<RfidEventLog> RfidEventLogs { get; set; }

        public DbSet<GameType> GameTypes { get; set; }

        public DbSet<MediaFile> MediaFiles { get; set; }

        public DbSet<MediaFileStream> MediaFileStreams { get; set; }

        public DbSet<Resident> Residents { get; set; }

        public DbSet<Caregiver> Caregivers { get; set; }

        public DbSet<User> Users { get; set; }
    }
}
