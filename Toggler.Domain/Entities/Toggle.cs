using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Toggler.Common;

namespace Toggler.Domain.Entities
{
    public class Toggle
    {
        [Key]
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public Constants.WellKnownToggleType Type { get; set; }
    }
}
