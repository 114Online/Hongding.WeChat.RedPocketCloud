using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RedPocketCloud.Models
{
    [DataNode("dn0,dn1,dn2,dn3")]
    public class VerificationLog
    {
        public long Id { get; set; }

        public long ProviderId { get; set; }

        public DateTime Time { get; set; }

        public long WalletId { get; set; }

        public long CouponId { get; set; }
    }
}
