using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RedPocketCloud.Models
{
    /// <summary>
    /// 红包活动页面模板类型（摇一摇、咻一咻）
    /// </summary>
    public enum TemplateType
    {
        Shake,
        Shoop
    }

    /// <summary>
    /// 红包活动页面模板实体
    /// </summary>
    [DataNode("dn0,dn1,dn2,dn3")]
    public class Template
    {
        /// <summary>
        /// 模板ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 模板类型
        /// </summary>
        public TemplateType Type { get; set; }

        /// <summary>
        /// 活动规则URL（暂未使用）
        /// </summary>
        [MaxLength(64)]
        public string RuleUrl { get; set; }
        
        /// <summary>
        /// [摇一摇]顶部图片BlobID / [咻一咻]咻一咻按钮图片BlobID
        /// </summary>
        public long? TopPartId { get; set; }

        /// <summary>
        /// [摇一摇]底部图片BlobID / [咻一咻]未使用
        /// </summary>
        public long? BottomPartId { get; set; }
        
        /// <summary>
        /// 北京图片BlobID
        /// </summary>
        public long? BackgroundId { get; set; }
        
        /// <summary>
        /// 中奖图片BlobID
        /// </summary>
        public long? DrawnId { get; set; }

        /// <summary>
        /// 未中奖BlobID
        /// </summary>
        public long? UndrawnId { get; set; }

        /// <summary>
        /// 活动未开始BlobID
        /// </summary>
        public long? PendingId { get; set; }

        /// <summary>
        /// 商户ID
        /// </summary>
        public long MerchantId { get; set; }
    }
}
