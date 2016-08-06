using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RedPocketCloud.Models;

namespace RedPocketCloud.Controllers
{
    [Authorize]
    public class CouponController : BaseController
    {
        public IActionResult Index()
        {
            IQueryable<Coupon> query = DB.Coupons;
            if (User.IsInRole("Root"))
            {
                var ret = query.Join(DB.Users, x => x.UserId, x => x.Id, (x, y) => new
                {
                    Title = x.Title,
                    Description = x.Description,
                    Id = x.Id,
                    UserId = y.Id,
                    UserName = y.UserName,
                    Merchant = y.Name,
                    Provider = x.Provider,
                    Time = x.Time
                });
                return PagedView(ret.OrderBy(x => x.UserId).ThenByDescending(x => x.Id));
            }
            else
            {
                query = query
                    .Where(x => x.UserId == User.Current.Id)
                    .OrderBy(x => x.Id);
                return PagedView(query.OrderByDescending(x => x.Id));
            }
        }
    }
}
