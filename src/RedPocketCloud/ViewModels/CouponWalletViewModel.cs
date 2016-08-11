using System;

namespace RedPocketCloud.ViewModels
{
    public class CouponWalletViewModel
    {
        public long CouponId { get; set; }

        public DateTime Expire { get; set; }

        public DateTime Time { get; set; }

        public long ImageId { get; set; }
        
        public string OpenId { get; set; }
    }
}
