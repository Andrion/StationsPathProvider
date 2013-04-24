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

        public DbSet<Station> Stations { get; set; }

        public DbSet<GStation> GStations { get; set; }

        public DbSet<Transport> Transports { get; set; }

        public DbSet<Direction> Directions { get; set; }

        public DbSet<DirectionStation> DirectionStations { get; set; }

        public DbSet<TransportType> TransportTypes { get; set; }

        public DbSet<Schedule> Schedules { get; set; }

        public DbSet<ScheduleItem> ScheduleItems { get; set; }

        #endregion DbSets
    }
}