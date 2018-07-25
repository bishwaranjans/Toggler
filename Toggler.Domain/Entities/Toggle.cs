using System.ComponentModel.DataAnnotations;
using Toggler.Common;

namespace Toggler.Domain.Entities
{
    /// <summary>
    /// Toggle entity
    /// </summary>
    public class Toggle
    {
        /// <summary>
        /// Gets or sets the toggle name.
        /// </summary>
        /// <value>
        /// The name of the toggle.
        /// </value>
        [Key]
        [Required]
        public string Name { get; set; }
        
        /// <summary>
        /// Gets or sets the description of the toggle.
        /// </summary>
        /// <value>
        /// The description of toggle.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the type of the toggle.
        /// </summary>
        /// <value>
        /// The toggle type.
        /// </value>
        [Required]
        public Constants.WellKnownToggleType Type { get; set; }
    }
}
