using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pomelo.AspNetCore.Extensions.BlobStorage.Models;

namespace RedPacketCloud.Models
{
    public class Template
    {
        public Guid Id { get; set; }

        [MaxLength(32)]
        public string Title { get; set; }

        [MaxLength(64)]
        public string RuleUrl { get; set; }

        [ForeignKey("TopPart")]
        public Guid? TopPartId { get; set; }

        public virtual Blob TopPart { get; set; }

        [ForeignKey("BottomPart")]
        public Guid? BottomPartId { get; set; }

        public virtual Blob BottomPart { get; set; }

        [ForeignKey("Background")]
        public Guid? BackgroundId { get; set; }

        public virtual Blob Background { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }

        public virtual User User { get; set; }
    }
}
