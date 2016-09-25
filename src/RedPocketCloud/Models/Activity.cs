using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RedPocketCloud.ViewModels;

namespace RedPocketCloud.Models
{
    /// <summary>
    /// 红包活动实体
    /// </summary>
    [DataNode("dn0,dn1,dn2,dn3")]
    public class Activity
    {
        /// <summary>
        /// 活动ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 活动类型
        /// </summary>
        public ActivityType Type { get; set; }

        [MaxLength(64)]
        public string Command { get; set; }

        /// <summary>
        /// 活动名称
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 活动规则
        /// </summary>
        public JsonObject<List<RuleViewModel>> Rules { get; set; }

        /// <summary>
        /// 活动开始时间
        /// </summary>
        public DateTime Begin { get; set; }

        /// <summary>
        /// 活动结束时间
        /// </summary>
        public DateTime? End { get; set; }

        /// <summary>
        /// 中奖率
        /// </summary>
        public double Ratio { get; set; }

        /// <summary>
        /// 红包个数（冗余）
        /// </summary>
        public long BriberiesCount { get; set; }

        /// <summary>
        /// 已经领取个数（冗余）
        /// </summary>
        public long ReceivedCount { get; set; }

        /// <summary>
        /// 现金红包总额（冗余）
        /// </summary>
        public long Price { get; set; }

        /// <summary>
        /// 参与人数（冗余）
        /// </summary>
        public long Attend { get; set; }

        /// <summary>
        /// 本次活动每用户中奖上限
        /// </summary>
        public int Limit { get; set; }
        
        /// <summary>
        /// 商户ID（对应User.Id）
        /// </summary>
        public long MerchantId { get; set; }
        
        /// <summary>
        /// 红包活动页面模板ID
        /// </summary>
        public long TemplateId { get; set; }

        /// <summary>
        /// 是否开始
        /// </summary>
        public bool IsBegin { get; set; }
    }
}
