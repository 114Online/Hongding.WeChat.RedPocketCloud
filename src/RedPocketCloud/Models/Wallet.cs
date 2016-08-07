using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedPocketCloud.Models
{
    public class Wallet
    {
        public long Id { get; set; }

        public long MerchantId { get; set; }

        public DateTime Time { get; set; }

        public DateTime Expire { get; set; }

        public long CouponId { get; set; }
    }
}
