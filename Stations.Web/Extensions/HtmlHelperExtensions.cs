namespace Stations.Web.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;

    /// <summary>
    /// Html helper extensions.
    /// </summary>
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Gets the validation attributes.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="html">The HTML.</param>
        /// <param name="property">The property.</param>
        /// <returns>String validation attributes.</returns>
        public static MvcHtmlString GetValidateAttributes<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> property)
        {
            String propertyName = html.NameFor(property).ToString();
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(property, html.ViewData);
            IDictionary<String, Object> attributes = html.GetUnobtrusiveValidationAttributes(propertyName, metadata);

            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<String, Object> keyValuePair in attributes)
            {
                String key = keyValuePair.Key;
                String str = HttpUtility.HtmlAttributeEncode(keyValuePair.Value.ToString());
                sb.Append(' ').Append(key).Append("=\"").Append(str).Append('"');
            }
            
            return MvcHtmlString.Create(sb.ToString());
        }
    }
}