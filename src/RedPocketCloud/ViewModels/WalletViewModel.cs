using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedPocketCloud.ViewModels
{
    public class WalletViewModel
    {
        public long Id { get; set; }
        public long ImageId { get; set; }
        public string Description { get; set; }
        public string VerifyCode { get; set; }
    }
}
