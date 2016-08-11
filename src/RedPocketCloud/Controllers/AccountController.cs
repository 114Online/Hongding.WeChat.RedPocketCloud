using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RedPocketCloud.Models;

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
        public async Task<IActionResult> Login(string username, string password)
        {
            var result = await SignInManager.PasswordSignInAsync(username, password, false, false);
            if (result.Succeeded)
                return RedirectToAction("Index", "Home");
            else
                return RedirectToAction("Login", "Account");
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
        public async Task<IActionResult> Password(string old, string @new, string confirm)
        {
            if (@new != confirm)
                return Prompt(x =>
                {
                    x.Title = "修改失败";
                    x.Details = "两次密码不一致";
                });
            var result = await User.Manager.ChangePasswordAsync(User.Current, old, @new);
            if (result.Succeeded)
                return Prompt(x =>
                {
                    x.Title = "修改成功";
                    x.Details = "新密码已经生效！";
                });
            else
                return Prompt(x =>
                {
                    x.Title = "修改失败";
                    x.Details = result.Errors.First().Description;
                });
        }

        /// <summary>
        /// 展示创建商户界面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Root")]
        public IActionResult Create() => View();

        /// <summary>
        /// 处理创建商户请求
        /// </summary>
        /// <param name="balance"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Root")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(double balance, string username, string password, string role)
        {
            var user = new User { UserName = username, Balance = balance };
            await UserManager.CreateAsync(user, password);
            await UserManager.AddToRoleAsync(user, role);
            if (balance > 0)
            {
                DB.PayLogs.Add(new PayLog
                {
                    Balance = balance,
                    Price = balance,
                    Time = DateTime.Now,
                    MerchantId = user.Id
                });
                DB.SaveChanges();
            }
            return Prompt(x =>
            {
                x.Title = "创建成功";
                x.Details = $"用户{ user.UserName }已经成功创建";
            });
        }

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
        public async Task<IActionResult> ResetPwd(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            return View(user);
        }

        /// <summary>
        /// 处理强制修改密码请求
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Root")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPwd(string id, string pwd)
        {
            var user = await UserManager.FindByIdAsync(id);
            var token = await UserManager.GeneratePasswordResetTokenAsync(user);
            await UserManager.ResetPasswordAsync(user, token, pwd);
            return Prompt(x =>
            {
                x.Title = "修改成功";
                x.Details = $"{ user.UserName }的密码已经被重置成为了{ pwd }";
            });
        }

        /// <summary>
        /// 处理设置红包每日上限请求
        /// </summary>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Limit(int limit)
        {
            User.Current.Limit = limit;
            DB.SaveChanges();
            return Content("ok");
        }
    }
}
