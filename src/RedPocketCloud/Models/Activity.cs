using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using RedPocketCloud.ViewModels;

namespace RedPocketCloud.Models
{
    public class Activity
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public JsonObject<List<RuleViewModel>> Rules { get; set; }

        public DateTime Begin { get; set; }

        public DateTime? End { get; set; }

        public double Ratio { get; set; }

        public long BriberiesCount { get; set; }

        public long Price { get; set; }

        public long Attend { get; set; }

        public int Limit { get; set; }

        [ForeignKey("Owner")]
        public long OwnerId { get; set; }

        public virtual User Owner { get; set; }
        
        public long? TemplateId { get; set; }
    }
}
