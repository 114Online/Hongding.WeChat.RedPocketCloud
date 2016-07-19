using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using RedPacketCloud.ViewModels;

namespace RedPacketCloud.Models
{
    public class Activity
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public JsonObject<List<Rule>> Rules { get; set; }

        public DateTime Begin { get; set; }

        public DateTime? End { get; set; }

        public double Ratio { get; set; }

        public long BriberiesCount { get; set; }

        public long Price { get; set; }

        public long Attend { get; set; }

        public int Limit { get; set; }

        [ForeignKey("Owner")]
        public string OwnerId { get; set; }

        public virtual User Owner { get; set; }

        [ForeignKey("Template")]
        public Guid? TemplateId { get; set; }

        public virtual Template Template { get; set; }
    }
}
