using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace RedPocketCloud.Controllers
{
    public class WeChatController : BaseController
    {
        public IActionResult RedPocket(string Username)
        {
            return View(GetActivityByUserId(Username));
        }
    }
}
