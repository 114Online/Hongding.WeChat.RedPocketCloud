using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RedPocketCloud.Models
{
    public class PayLog
    {
        public long Id { get; set; }
        
        public long UserId { get; set; }

        public double Price { get; set; }

        public double Current { get; set; }

        public DateTime Time { get; set; }
    }
}
