using System.Collections.Generic;
using System.Data.Entity;
using BitMex.Robot.DataLayer.Classes;
using Common.Classes.Robot;
using Common.Robots.Common;

namespace BitMex.Robot.DataLayer
{
    public class RobotContext : DbContext
    {
        public RobotContext()
            : base("BitMexRobotDatabase")
        {
            Database.SetInitializer(new RobotDatabaseInitializer());
        }

        public DbSet<DbRobot> Robots { get; set; }
        public DbSet<DbOrder> Orders { get; set; }
        public DbSet<DbTrade> Trades { get; set; }
        public DbSet<DbLogItem> Logs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbRobot>().ToTable("Robots");
            modelBuilder.Entity<DbOrder>().ToTable("Orders");
            modelBuilder.Entity<DbLogItem>().ToTable("Logs");

            modelBuilder.Entity<DbRobot>().HasKey<string>(s => s.RobotId);
            modelBuilder.Entity<DbOrder>().HasKey<string>(s => s.OrderId);
            modelBuilder.Entity<DbTrade>().HasKey<string>(s => s.TradeId);
            modelBuilder.Entity<DbLogItem>().HasKey<int>(s => s.Id);
            modelBuilder.Entity<DbLogItem>().Property(s => s.Text).IsMaxLength();
        }
    }

    public class RobotDatabaseInitializer : CreateDatabaseIfNotExists<RobotContext>
    {
        protected override void Seed(RobotContext context)
        {
            var robots = new List<DbRobot>()
            {
                new DbRobot
                {
                    RobotId =  RobotIdentifier.NetThrower_1min_075,
                    Mode = (int)RobotModes.Operative,
                    Status = (int)RobotOperationalStatuses.NotStarted}
            };

            foreach (var r in robots)
            {
                context.Robots.Add(r);
            }

            base.Seed(context);
        }
    }
}
