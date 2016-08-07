using System;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static RedPocketCloud.Common.Wxpay;

namespace RedPocketCloud.Controllers
{
    public enum Operation
    {
        CallBack,
        RedPocket,
        Wallet
    }

    public class WeChatController : BaseController
    {
        #region Infrastructures
        private string OperationToRoute(string Merchant, Operation Operation)
        {
            var prefix = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + "/WeChat/";
            var postfix = "/" + Merchant;
            switch (Operation)
            {
                case Operation.CallBack:
                    return prefix + "CallBack" + postfix;
                case Operation.RedPocket:
                    return prefix + "RedPocket" + postfix;
                case Operation.Wallet:
                    return prefix + "Wallet" + postfix;
                default:
                    return null;
            }
        }

        private bool NeedAuthorize => string.IsNullOrWhiteSpace(HttpContext.Session.GetString("OpenId")) || Convert.ToDateTime(HttpContext.Session.GetString("Expire")) <= DateTime.Now;

        [NonAction]
        private IActionResult RedirectToEntry() => RedirectToAction("Entry", "WeChat", new { Merchant = RouteData.Values["Merchant"].ToString(), Operation = Enum.Parse(typeof(Operation), HttpContext.Request.Query["Operation"].ToString()) });

        [NonAction]
        private IActionResult RedirectToEntry(Operation operation) => RedirectToAction("Entry", "WeChat", new { Merchant = RouteData.Values["Merchant"].ToString(), Operation = operation });

        #endregion

        [HttpGet]
        [Route("[controller]/Entry/{Merchant}/{Operation}")]
        public IActionResult Entry(string Merchant, Operation Operation)
        {
            if (NeedAuthorize)
                return Redirect("https://open.weixin.qq.com/connect/oauth2/authorize?appid=" + Startup.Config["WeChat:AppId"] + "&redirect_uri=" + UrlEncoder.Default.Encode(OperationToRoute(Merchant, Operation.CallBack) + "?next=" + OperationToRoute(Merchant, Operation)) + "&response_type=code&scope=snsapi_userinfo");
            else
                return Redirect(OperationToRoute(Merchant, Operation));
        }

        [HttpGet]
        [Route("[controller]/CallBack/{Merchant}")]
        public async Task<IActionResult> CallBack(string Merchant, string code, string next)
        {
            try
            {
                var oid = await AuthorizeAsync(code);
                HttpContext.Session.SetString("OpenId", oid.Id);
                HttpContext.Session.SetString("AccessToken", oid.AccessToken);
                HttpContext.Session.SetString("Expire", oid.AccessTokenExpire.ToString());
                HttpContext.Session.SetString("Nickname", oid.NickName);
                HttpContext.Session.SetString("AvatarUrl", oid.AvatarUrl);
                return Redirect(next);
            }
            catch
            {
                return RedirectToEntry(Operation.RedPocket);
            }
        }

        [HttpGet]
        public IActionResult RedPocket(string Merchant)
        {
            if (NeedAuthorize)
                return RedirectToEntry(Operation.RedPocket);
            return View(GetActivityByUserId(Merchant));
        }
    }
}
