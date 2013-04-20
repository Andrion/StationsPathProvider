namespace Stations.Web.Models
{
    using System;
    using System.Configuration;
    using System.Data.Common;
    using System.Data.Entity.Infrastructure;

    /// <summary>
    /// Connection factory.
    /// </summary>
    public class ConnectionFactory : IDbConnectionFactory
    {
        #region Fields

        /// <summary>
        /// Name of default connection.
        /// </summary>
        private const String _defaultConnectionName = "DefaultConnection";

        /// <summary>
        /// Connection string name.
        /// </summary>
        private readonly String _connectionName = null;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionFactory"/> class.
        /// </summary>
        public ConnectionFactory()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionFactory"/> class.
        /// </summary>
        /// <param name="connectionName">Name of the connection.</param>
        public ConnectionFactory(String connectionName)
        {
            this._connectionName = connectionName;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Creates a connection based on the given database name or connection String.
        /// </summary>
        /// <param name="nameOrConnectionString">The database name or connection String. </param>
        /// <returns>
        /// An initialized DbConnection. 
        /// </returns>
        public DbConnection CreateConnection(String nameOrConnectionString)
        {
            String connectionString = ConfigurationManager.ConnectionStrings[this._connectionName ?? _defaultConnectionName]
                .ConnectionString;

            return new SqlConnectionFactory().CreateConnection(connectionString);
        }

        #endregion Methods
    }
}
