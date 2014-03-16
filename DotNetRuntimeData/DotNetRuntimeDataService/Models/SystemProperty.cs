using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DotNetRuntimeDataService.Models
{
    public class SystemProperty
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Index]
        public DateTimeOffset? CreatedAt { get; set; }

        public bool Deleted { get; set; }

        public int balls { get; set; }

        [Key]
        public string Id { get; set; } 

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTimeOffset? UpdatedAt { get; set; }

        [Timestamp]
        public byte[] Version { get; set; }

        [Required]
        public virtual Order Order { get; set; }
    }
}