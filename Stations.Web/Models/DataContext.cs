namespace Stations.Web.Models
{
    using System.Data.Entity;

    /// <summary>
    /// Data context.
    /// </summary>
    public class DataContext : DbContext
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DataContext"/> class.
        /// </summary>
        public DataContext()
            : base("Default")
        {
        }

        #endregion Constructors

        #region DbSets

        public DbSet<Group> Groups { get; set; }

        public DbSet<Station> Stations { get; set; }

        public DbSet<Transport> Transports { get; set; }

        public DbSet<TransportType> TransportTypes { get; set; }

        public DbSet<ScheduleItem> ScheduleItems { get; set; }

        public DbSet<IFStation> IFStations { get; set; }

        #endregion DbSets
    }
}