using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Toggler.Common;

namespace Toggler.Domain.Entities
{
    public class ServiceToggle
    {
        [Key]
        public string UniqueId { get; set; }
        public bool IsEnabled { get; set; }
        public Service Service { get; set; }
        public Toggle Toggle { get; set; }
        public bool IsServiceExcluded { get; set; } = false;
    }
}
