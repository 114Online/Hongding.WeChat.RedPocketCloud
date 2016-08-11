namespace RedPocketCloud.Models
{
    /// <summary>
    /// 优惠券实体
    /// </summary>
    public class Coupon
    {
        /// <summary>
        /// 优惠券ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 优惠券标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 优惠券有效期（天）
        /// </summary>
        public int Time { get; set; }

        /// <summary>
        /// 优惠券介绍
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 优惠券提供商
        /// </summary>
        public string Provider { get; set; }

        /// <summary>
        /// 优惠券背景图
        /// </summary>
        public long ImageId { get; set; }
        
        /// <summary>
        /// 优惠券提供商logo图片BlobID
        /// </summary>
        public long ProviderImageId { get; set; }

        /// <summary>
        /// 商户ID（对应User.Id）
        /// </summary>
        public long MerchantId { get; set; }
    }
}
