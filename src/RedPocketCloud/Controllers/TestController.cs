using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace RedPocketCloud.Controllers
{
    public class TestController : Controller
    {
        /// <summary>
        /// 伪造微信授权信息以便测试
        /// </summary>
        /// <returns></returns>
        public IActionResult Test()
        {
            HttpContext.Session.SetString("OpenId", "ol9Liw5EMh4q7zN9UMwCyEalgb2k");
            HttpContext.Session.SetString("AvatarUrl", "http://wx.qlogo.cn/mmopen/bVy2VQVTWzbUP1CS3aEHicwEpYrAUkHNwVibwHdnVuEC6wPhTs9LNepMW32U98CoJHOZahGibQONB2gnuIdvlVc7A/0");
            HttpContext.Session.SetString("Expire", DateTime.Now.AddDays(1).ToString());
            HttpContext.Session.SetString("Nickname", "あまみや ゆうこ");
            return Content("OK");
        }
    }
}
