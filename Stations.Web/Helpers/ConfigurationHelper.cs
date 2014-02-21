namespace Stations.Web
{
    using System;
    using System.Configuration;

    /// <summary>
    /// Configuration helper.
    /// </summary>
    public class ConfigurationHelper
    {
        #region Private Fields

        #endregion Private Fields

        #region Constructors

        static ConfigurationHelper()
        {
            Instance = new ConfigurationHelper();
        }

        #endregion Constructors

        #region Public Properties

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static ConfigurationHelper Instance { get; private set; }

        /// <summary>
        /// Gets the DB connection String.
        /// </summary>
        public String DBConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["Default"].ConnectionString; }
        }

        public String YMapsApiKey
        {
            get { return this.GetValue("YMapsApiKey"); }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        /// Value of config by specified name.
        /// </returns>
        public String GetValue(String name)
        {
            return ConfigurationManager.AppSettings.Get(name);
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void SetValue(String key, String value)
        {
            ConfigurationManager.AppSettings.Set(key, value);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <returns>
        /// Casted value of config by specified name.
        /// </returns>
        public T GetValue<T>(String name)
        {
            String strValue = this.GetValue(name);

            T result;

            if (typeof(T) == typeof(Boolean))
            {
                result = (T)((Object)Boolean.Parse(strValue));
            }
            else
            {
                result = (T)((Object)strValue);
            }

            return result;
        }

        /// <summary>
        /// Gets the value or default.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <returns>
        /// Value of config by specified name or default.
        /// </returns>
        public T GetValueOrDefault<T>(String name)
        {
            T result = default(T);

            try
            {
                result = this.GetValue<T>(name);
            }
            catch (InvalidCastException)
            {
            }
            catch (NullReferenceException)
            {
            }
            catch (FormatException)
            {
            }

            return result;
        }

        #endregion Public Methods
    }
}
