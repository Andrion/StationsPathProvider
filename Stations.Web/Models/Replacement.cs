namespace Stations.Web.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Replacement entity.
    /// </summary>
    public class Replacement : Entity
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [Required]
        public String Value { get; set; }

        /// <summary>
        /// Gets or sets the replace value.
        /// </summary>
        /// <value>
        /// The replace value.
        /// </value>
        [Required]
        public String ReplaceValue { get; set; }
    }
}