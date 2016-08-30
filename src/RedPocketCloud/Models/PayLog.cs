using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RedPocketCloud.Models
{
    /// <summary>
    /// 商户支付记录实体
    /// </summary>
    [DataNode("dn0,dn1,dn2,dn3")]
    public class PayLog
    {
        /// <summary>
        /// 支付ID
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// 商户ID（对应User.Id）
        /// </summary>
        public long MerchantId { get; set; }

        /// <summary>
        /// 金额变动
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// 当前余额
        /// </summary>
        public double Balance { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime Time { get; set; }
    }
}
