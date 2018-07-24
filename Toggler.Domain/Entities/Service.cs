using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Toggler.Domain.Entities
{
    public class Service
    {
        [Key]
        [Required]
        public string Name { get; set; }
        [Required]
        public string Version { get; set; }
        public string Description { get; set; }
    }
}
