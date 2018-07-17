using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Common.Classes.Robot;
using Common.Robots.Common;
using Robot.DataLayer.Classes;

namespace Robot.DataLayer
{
    public class RobotContext : DbContext
    {
        public RobotContext()
            : base("RobotDatabase")
        {
            Database.SetInitializer(new RobotDatabaseInitializer());
        }

        public DbSet<DbRobot> Robots { get; set; }
        public DbSet<DbRobotState> RobotStates { get; set; }
        public DbSet<DbOrder> Orders { get; set; }
        public DbSet<DbLogItem> Logs { get; set; }
        public DbSet<DbRobotTrade> RobotTrades { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbRobot>().ToTable("Robots");
            modelBuilder.Entity<DbRobotState>().ToTable("RobotStates");
            modelBuilder.Entity<DbOrder>().ToTable("Orders");
            modelBuilder.Entity<DbLogItem>().ToTable("Logs");

            modelBuilder.Entity<DbRobot>().HasKey<int>(s => s.Id);
            modelBuilder.Entity<DbRobotState>().HasKey<int>(s => s.Id);
            modelBuilder.Entity<DbOrder>().HasKey<int>(s => s.Id);
            modelBuilder.Entity<DbLogItem>().HasKey<int>(s => s.Id);
            //modelBuilder.Entity<DbLogItem>().Property(s => s.Text).HasColumnType("text");
            modelBuilder.Entity<DbLogItem>().Property(s => s.Text).IsMaxLength();

            //modelBuilder.Entity<DbRobot>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
        }
    }

    public class RobotDatabaseInitializer : DropCreateDatabaseIfModelChanges<RobotContext>
    {
        protected override void Seed(RobotContext context)
        {
            var robots = Enum<RobotNames>
                .ToList()
                .Select(keyValue => new DbRobot
                {
                    Name = keyValue.Key.ToString(),
                    Mode = (int)RobotModes.Logging,
                    Status = (int)RobotOperationalStatuses.NotStarted
                });


            foreach (var r in robots)
            {
                context.Robots.Add(r);
            }

            base.Seed(context);
        }
    }
}
