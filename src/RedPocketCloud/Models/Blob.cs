using System.ComponentModel.DataAnnotations.Schema;
using Pomelo.AspNetCore.Extensions.BlobStorage.Models;

namespace RedPocketCloud.Models
{
    /// <summary>
    /// 二进制文件存储实体
    /// </summary>
    [DataNode("dn4,dn5,dn6,dn7")]
    public class Blob : Blob<long>
    {
    }
}
