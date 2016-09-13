using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RedPocketCloud.Models
{
    /// <summary>
    /// 红包类型（现金、链接、优惠券）
    /// </summary>
    public enum RedPocketType
    {
        Cash,
        Url,
        Coupon
    }

    /// <summary>
    /// 红包（单个）实体
    /// </summary>
    [DataNode("dn0,dn1,dn2,dn3")]
    public class RedPocket
    {
        /// <summary>
        /// 红包ID
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// 活动ID
        /// </summary>
        public long ActivityId { get; set; }

        /// <summary>
        /// 领取时间
        /// </summary>
        public DateTime? ReceivedTime { get; set; }

        /// <summary>
        /// 以分为单位
        /// </summary>
        public long Price { get; set; }

        /// <summary>
        /// 红包类型
        /// </summary>
        public RedPocketType Type { get; set; }

        /// <summary>
        /// 中奖URL
        /// </summary>
        [MaxLength(256)]
        public string Url { get; set; }

        /// <summary>
        /// 优惠券ID
        /// </summary>
        public long? CouponId { get; set; }

        /// <summary>
        /// 微信OpenID
        /// </summary>
        [MaxLength(64)]
        public string OpenId { get; set; }

        /// <summary>
        /// 微信头像URL
        /// </summary>
        [MaxLength(512)]
        public string AvatarUrl { get; set; }

        /// <summary>
        /// 微信昵称
        /// </summary>
        [MaxLength(128)]
        public string NickName { get; set; }

        /// <summary>
        /// 中奖IP（暂未使用）
        /// </summary>
        [MaxLength(64)]
        public string Ip { get; set; }

        /// <summary>
        /// 并发标识
        /// </summary>
        [ConcurrencyCheck]
        [MaxLength(64)]
        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
    }
}
