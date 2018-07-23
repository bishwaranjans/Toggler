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
        public string ServiceName { get; set; }
        public string ServiceVersion { get; set; }
        public string ServiceDescription { get; set; }
        public string ToggleName { get; set; }
        public string ToggleDescription { get; set; }
        public Constants.WellKnownToggleType ToggleType { get; set; }
        public bool IsServiceExcluded { get; set; } = false;
    }
}
