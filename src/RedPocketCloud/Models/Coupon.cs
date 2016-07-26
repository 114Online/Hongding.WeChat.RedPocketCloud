using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedPocketCloud.Models
{
    public class Coupon
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public int Time { get; set; }

        public string Description { get; set; }

        public string Provider { get; set; }

        public long ImageId { get; set; }

        public long ProviderImageId { get; set; }
    }
}
