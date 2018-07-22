using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Toggler.Domain.Entities
{
    public class ServiceToggle
    {
        [Key]
        [Required]
        public string UniqueId { get; set; }
        public bool IsEnabled { get; set; }
        public virtual Service Service { get; set; }
        public virtual Toggle Toggle { get; set; }
     }
}
