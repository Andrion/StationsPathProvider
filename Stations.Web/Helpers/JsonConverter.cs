namespace Stations.Web.Helpers
{
    using System;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Json converter class.
    /// </summary>
    public static class JsonConverter
    {
        #region Private Fields

        /// <summary>
        /// Serializer settings.
        /// </summary>
        public static readonly JsonSerializerSettings SerializerSettings = null;

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Initializes the <see cref="JsonConverter"/> class.
        /// </summary>
        static JsonConverter()
        {
            SerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCaseResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            SerializerSettings.Converters.Add(new IsoDateTimeConverter());
            SerializerSettings.Converters.Add(new StringEnumConverter() { CamelCaseText = true });
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Serializes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>Serialized json data.</returns>
        public static String Serialize(Object data)
        {
            return JsonConvert.SerializeObject(data, Formatting.None, SerializerSettings);
        }

        /// <summary>
        /// Deserializes the specified json.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns>Deserialized Object.</returns>
        public static Object Deserialize(String json)
        {
            return JsonConvert.DeserializeObject(json, SerializerSettings);
        }

        /// <summary>
        /// Deserializes the specified json.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <param name="type">The type.</param>
        /// <returns>
        /// Deserialized Object.
        /// </returns>
        public static Object Deserialize(String json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type, SerializerSettings);
        }

        /// <summary>
        /// Deserializes the specified json.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json">The json.</param>
        /// <returns>Deserialized specified type Object.</returns>
        public static T Deserialize<T>(String json)
        {
            return JsonConvert.DeserializeObject<T>(json, SerializerSettings);
        }

        #endregion Public Methods
    }
}