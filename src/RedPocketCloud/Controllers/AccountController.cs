using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using RedPocketCloud.Models;
using Hongding.WeChat.UserCenter.SDK;

namespace RedPocketCloud.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        /// <summary>
        /// 展示登录界面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login() => View();

        /// <summary>
        /// 处理登录请求
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromServices] HongdingUC UC, [FromServices] SignInManager<User> SM, string username, string password)
        {
            var result = await UC.CheckTenantAccountAsync(username, password);
            if (result == null)
            {
                return View();
            }

            var user = await User.Manager.FindByNameAsync(username);
            if (user == null)
            {
                user = new User
                {
                    UserName = username,
                    Merchant = result.Name
                };
                await User.Manager.CreateAsync(user, password);
                await User.Manager.AddToRoleAsync(user, result.Role);
            }
            else
            {
                user.Merchant = result.Name;
                foreach (var x in user.Roles)
                    DB.UserRoles.Remove(x);
                DB.SaveChanges();
                await User.Manager.AddToRoleAsync(user, result.Role);
            }
            await SM.SignInAsync(user, true);
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// 注销登录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignOut()
        {
            await SignInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// 展示修改密码界面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Password() => View();

        /// <summary>
        /// 处理修改密码请求
        /// </summary>
        /// <param name="old"></param>
        /// <param name="new"></param>
        /// <param name="confirm"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Password([FromServices]HongdingUC UC, string old, string @new, string confirm)
        {
            if (@new != confirm)
            {
                return Prompt(x =>
                {
                    x.Title = "操作失败";
                    x.Details = "两次密码输入不一致，请返回重试！";
                });
            }
            var result = await UC.ChangeTenantPasswordAsync(User.Current.UserName, old, @new);
            if (!result)
            {
                return Prompt(x =>
                {
                    x.Title = "操作失败";
                    x.Details = "上游授权中心服务器拒绝了您的操作！";
                });
            }
            return Prompt(x =>
            {
                x.Title = "操作成功";
                x.Details = "您的新密码已经生效！";
            });
        }

        /// <summary>
        /// 展示创建商户界面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Root")]
        public IActionResult Create() => Prompt(x => 
        {
            x.Title = "操作失败";
            x.Details = "请在User Center中创建租户！";
        });

        /// <summary>
        /// 展示商户列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Root")]
        public IActionResult Index()
        {
            var ret = UserManager.Users.ToList();
            return View(ret);
        }

        /// <summary>
        /// 展示充值界面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Root")]
        public async Task<IActionResult> Charge(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            return View(user);
        }

        /// <summary>
        /// 处理为商户充值的请求
        /// </summary>
        /// <param name="id"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Root")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Charge(string id, double price)
        {
            var user = await UserManager.FindByIdAsync(id);
            lock (this)
            {
                user.Balance += price;
                DB.PayLogs.Add(new PayLog { MerchantId = User.Current.Id, Price = price, Time = DateTime.Now, Balance = user.Balance });
                DB.SaveChanges();
            }
            return Prompt(x =>
            {
                x.Title = "充值成功";
                x.Details = $"本次为 { user.UserName } 充入了 ￥{ price.ToString("0.00") }";
            });
        }

        /// <summary>
        /// 展示强制修改密码界面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Root")]
        public async Task<IActionResult> ResetPwd( string id)
        {
            return Prompt(x => 
            {
                x.Title = "操作失败";
                x.Details = "请进入User Center强制更改租户密码！";
                x.RedirectText = "访问User Center";
                x.RedirectUrl = string.IsNullOrEmpty(Startup.Config["UserCenter:Url"]) ? "http://uc.114-online.com" : Startup.Config["UserCenter:Url"];
            });
        }
        
        /// <summary>
        /// 处理设置红包每日上限请求
        /// </summary>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Limit(int limit, [FromServices]IDistributedCache Cache)
        {
            User.Current.Limit = limit;
            DB.SaveChanges();
            Cache.SetObject("MERCHANT_LIMIT_" + User.Current.UserName, limit);
            return Content("ok");
        }
    }
}
