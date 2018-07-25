using System.ComponentModel.DataAnnotations;

namespace Toggler.Domain.Entities
{
    /// <summary>
    /// Service entity
    /// </summary>
    public class Service
    {
        /// <summary>
        /// Gets or sets the service name.
        /// </summary>
        /// <value>
        /// The service name.
        /// </value>
        [Key]
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the service version.
        /// </summary>
        /// <value>
        /// The version of service.
        /// </value>
        [Required]
        public string Version { get; set; }


        /// <summary>
        /// Gets or sets the service description.
        /// </summary>
        /// <value>
        /// The description of service.
        /// </value>
        public string Description { get; set; }
    }
}
