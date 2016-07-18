using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RedPacketCloud.Models;

namespace RedPacketCloud.Controllers
{
    public class BaseController : BaseController<RpcContext, User, string>
    {
    }
}
