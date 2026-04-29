using RouteX.UAF.Entities.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace RouteX.UAF.DAL
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("name=dbContext")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, RouteX.UAF.DAL.Migrations.Configuration>());
        }

        //---------------------

        // --- Core Domain Tables ---
        public DbSet<User> Users { get; set; }
        public DbSet<Bus> Buses { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<BusStop> BusStops { get; set; }
        public DbSet<LocationLog> LocationLogs { get; set; }

        // --- Lookup Tables ---
        public DbSet<Role> Roles { get; set; }
        public DbSet<RouteDirection> RouteDirections { get; set; }
        public DbSet<StopAction> StopActions { get; set; }

        // --- Session & Security Tables ---
        public DbSet<DeviceToken> DeviceTokens { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<LoginLog> LoginLogs { get; set; }
    }
}