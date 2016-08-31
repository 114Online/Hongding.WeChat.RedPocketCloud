using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RedPocketCloud.Models
{
    /// <summary>
    /// 卡券实体
    /// </summary>
    [DataNode("dn0,dn1,dn2,dn3")]
    public class Wallet
    {
        /// <summary>
        /// 卡券ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 校验码
        /// </summary>
        [MaxLength(20)]
        public string VerifyCode { get; set; } = GenerateVerifyCode();

        /// <summary>
        /// 所属商户ID
        /// </summary>
        public long MerchantId { get; set; }

        /// <summary>
        /// 所属商家ID
        /// </summary>
        public long ProviderId { get; set; }

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

        /// <summary>
        /// 生成校验码
        /// </summary>
        /// <returns></returns>
        private static string GenerateVerifyCode()
        {
            var rand = new Random();
            return rand.Next(1000, 9999) + "-" + rand.Next(1000, 9999) + "-" + rand.Next(1000, 9999) + "-" + rand.Next(1000, 9999);
        }
    }
}
