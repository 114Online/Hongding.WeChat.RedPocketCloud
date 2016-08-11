using System;
using System.ComponentModel.DataAnnotations;

namespace RedPocketCloud.Models
{
    /// <summary>
    /// 卡券实体
    /// </summary>
    public class Wallet
    {
        /// <summary>
        /// 卡券ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 所属商户ID
        /// </summary>
        public long MerchantId { get; set; }

        /// <summary>
        /// 获得时间
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime Expire { get; set; }

        /// <summary>
        /// 优惠券ID
        /// </summary>
        public long CouponId { get; set; }

        /// <summary>
        /// 优惠券拥有者的微信OpenID
        /// </summary>
        [MaxLength(32)]
        public string OpenId { get; set; }
    }
}
