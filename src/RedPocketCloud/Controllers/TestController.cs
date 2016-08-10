﻿using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace RedPocketCloud.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Test()
        {
            HttpContext.Session.SetString("OpenId", "oHriRjg5tgcRD5f0jM7la5BMsC18");
            HttpContext.Session.SetString("AvatarUrl", "http://wx.qlogo.cn/mmopen/bVy2VQVTWzbUP1CS3aEHicwEpYrAUkHNwVibwHdnVuEC6wPhTs9LNepMW32U98CoJHOZahGibQONB2gnuIdvlVc7A/0");
            HttpContext.Session.SetString("Expire", DateTime.Now.AddDays(1).ToString());
            HttpContext.Session.SetString("Nickname", "あまみや ゆうこ");
            return Content("OK");
        }
    }
}
