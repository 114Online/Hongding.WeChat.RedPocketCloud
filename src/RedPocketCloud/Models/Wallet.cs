using System;
using System.ComponentModel.DataAnnotations;

namespace RedPocketCloud.Models
{
    public class Wallet
    {
        public long Id { get; set; }

        public long MerchantId { get; set; }

        public DateTime Time { get; set; }

        public DateTime Expire { get; set; }

        public long CouponId { get; set; }

        [MaxLength(32)]
        public string OpenId { get; set; }
    }
}
