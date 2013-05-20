namespace Stations.Web.Helpers
{
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// Camel case property names contract resolver.
    /// </summary>
    public class CamelCaseResolver : CamelCasePropertyNamesContractResolver
    {
        #region Protected Methods

        /// <summary>
        /// Resolves the name of the property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        /// The property name camel cased.
        /// </returns>
        protected override string ResolvePropertyName(string propertyName)
        {
            if (propertyName.Equals("ID"))
            {
                propertyName = propertyName.ToLower();
            }

            return base.ResolvePropertyName(propertyName);
        }

        #endregion Protected Methods
    }
}