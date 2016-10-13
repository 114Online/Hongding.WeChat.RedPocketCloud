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
            Response.Cookies.Append("x-OpenId", "ol9Liw5EMh4q7zN9UMwCyEalgb2k", new CookieOptions() { Expires = DateTime.Now.AddDays(7) });
            Response.Cookies.Append("x-AvatarUrl", "http://wx.qlogo.cn/mmopen/bVy2VQVTWzbUP1CS3aEHicwEpYrAUkHNwVibwHdnVuEC6wPhTs9LNepMW32U98CoJHOZahGibQONB2gnuIdvlVc7A/0", new CookieOptions() { Expires = DateTime.Now.AddDays(7) });
            Response.Cookies.Append("x-NickName", "あまみや ゆうこ", new CookieOptions() { Expires = DateTime.Now.AddDays(7) });
            return Content("OK");
        }

        public IActionResult TestEmoji()
        {
            Response.Cookies.Append("x-OpenId", "ol9Liw5EMh4q7zN9UMwCyEalgb2k", new CookieOptions() { Expires = DateTime.Now.AddDays(7) });
            Response.Cookies.Append("x-AvatarUrl", "http://wx.qlogo.cn/mmopen/bVy2VQVTWzbUP1CS3aEHicwEpYrAUkHNwVibwHdnVuEC6wPhTs9LNepMW32U98CoJHOZahGibQONB2gnuIdvlVc7A/0", new CookieOptions() { Expires = DateTime.Now.AddDays(7) });
            Response.Cookies.Append("x-NickName", "あまみや💁👌🎍😍", new CookieOptions() { Expires = DateTime.Now.AddDays(7) });
            return Content("OK");
        }

        public IActionResult Clear()
        {
            Response.Cookies.Delete("x-OpenId");
            Response.Cookies.Delete("x-AvatarUrl");
            Response.Cookies.Delete("x-NickName");
            return Content("OK");
        }
    }
}
