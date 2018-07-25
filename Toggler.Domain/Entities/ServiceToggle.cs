using System.ComponentModel.DataAnnotations;

namespace Toggler.Domain.Entities
{
    /// <summary>
    /// Mapping of service with toggle
    /// </summary>
    public class ServiceToggle
    {

        /// <summary>
        /// Gets or sets the unique identifier of the mapping table.
        /// </summary>
        /// <value>
        /// The unique identifier of the ServiceToggle mapping table.
        /// </value>
        [Key]
        public string UniqueId { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether this instance of mapping is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnabled { get; set; }
        
        /// <summary>
        /// Gets or sets the service which needs to be mapped with toggle.
        /// </summary>
        /// <value>
        /// The service.
        /// </value>
        public Service Service { get; set; }

        /// <summary>
        /// Gets or sets the toggle with mapped to service.
        /// </summary>
        /// <value>
        /// The toggle.
        /// </value>
        public Toggle Toggle { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether this instance is service excluded.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is service excluded; otherwise, <c>false</c>.
        /// </value>
        public bool IsServiceExcluded { get; set; } = false;
    }
}
