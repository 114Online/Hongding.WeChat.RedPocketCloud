using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using RedPocketCloud.Models;
using RedPocketCloud.ViewModels;

namespace RedPocketCloud.Controllers
{
    public class BaseController : BaseController<RpcContext, Models.User, long>
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public override void Prepare()
        {
            base.Prepare();

            if (User.IsSignedIn())
            {
                var notifications = new List<NotificationViewModel>();
                if (User.IsInRole("Root"))
                    notifications.AddRange(DB.Activities
                        .Where(x => !x.End.HasValue)
                        .Select(x => new NotificationViewModel
                        {
                            Text = x.Title + " 正在进行",
                            Url = Url.Action("Activity", "RedPocket", new { id = x.Id })
                        })
                        .ToList());
                else
                    notifications.AddRange(DB.Activities
                        .Where(x => !x.End.HasValue && x.MerchantId == User.Current.Id)
                        .Select(x => new NotificationViewModel
                        {
                            Text = x.Title + " 正在进行",
                            Url = Url.Action("Activity", "RedPocket", new { id = x.Id })
                        })
                        .ToList());
                ViewBag.Notifications = notifications;
            }
        }
    }
}
