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

        public DbSet<HandledStation> HandledStations { get; set; }

        public DbSet<Transport> Transports { get; set; }

        public DbSet<Direction> Directions { get; set; }

        public DbSet<DirectionStation> DirectionStations { get; set; }

        public DbSet<TransportType> TransportTypes { get; set; }

        public DbSet<Schedule> Schedules { get; set; }

        public DbSet<ScheduleItem> ScheduleItems { get; set; }

        public DbSet<Replacement> Replacements { get; set; }

        #endregion DbSets

        #region Methods

        /// <summary>
        /// This method is called when the model for a derived context has been initialized, but
        /// before the model has been locked down and used to initialize the context.  The default
        /// implementation of this method does nothing, but it can be overridden in a derived class
        /// such that the model can be further configured before it is locked down.
        /// </summary>
        /// <param name="modelBuilder">The builder that defines the model for the context being created.</param>
        /// <remarks>
        /// Typically, this method is called only once when the first instance of a derived context
        /// is created.  The model for that context is then cached and is for all further instances of
        /// the context in the app domain.  This caching can be disabled by setting the ModelCaching
        /// property on the given ModelBuidler, but note that this can seriously degrade performance.
        /// More control over caching is provided through use of the DbModelBuilder and DbContextFactory
        /// classes directly.
        /// </remarks>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }

        #endregion Methods
    }
}