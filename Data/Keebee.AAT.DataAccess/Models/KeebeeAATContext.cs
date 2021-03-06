﻿using System.Data.Entity;

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

        public DbSet<InteractiveActivityEventLog> InteractiveActivityEventLogs { get; set; }

        public DbSet<ActiveResidentEventLog> ActiveResidentEventLogs { get; set; }

        public DbSet<InteractiveActivityType> InteractiveActivityTypes { get; set; }

        public DbSet<MediaFile> MediaFiles { get; set; }

        public DbSet<MediaFileStream> MediaFileStreams { get; set; }

        public DbSet<Thumbnail> Thumbnails { get; set; }

        public DbSet<MediaPathType> MediaPathTypes { get; set; }

        public DbSet<MediaPathTypeCategory> MediaPathTypeCategories { get; set; }

        public DbSet<ResidentMediaFile> ResidentMediaFiles { get; set; }

        public DbSet<PublicMediaFile> PublicMediaFiles { get; set; }

        public DbSet<Resident> Residents { get; set; }

        public DbSet<ActiveResident> ActiveResidents { get; set; }

        public DbSet<AmbientInvitation> AmbientInvitations { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }
    }
}
