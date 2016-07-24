using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pomelo.AspNetCore.Extensions.BlobStorage.Models;

namespace RedPocketCloud.Models
{
    public enum TemplateType
    {
        Shake,
        Shoop
    }

    public class Template
    {
        public long Id { get; set; }

        public TemplateType Type { get; set; }

        [MaxLength(64)]
        public string RuleUrl { get; set; }
        
        public long? TopPartId { get; set; }

        public long? BottomPartId { get; set; }
        
        public long? BackgroundId { get; set; }
        
        public long? DrawnId { get; set; }
        
        public long? UndrawnId { get; set; }

        public long? PendingId { get; set; }

        [ForeignKey("User")]
        public long UserId { get; set; }

        public virtual User User { get; set; }
    }
}
