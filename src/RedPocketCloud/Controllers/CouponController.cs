using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using RedPocketCloud.Models;
using RedPocketCloud.ViewModels;

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
                var ret = query.Join(DB.Users, x => x.UserId, x => x.Id, (x, y) => new CouponViewModel
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

        [HttpGet]
        public IActionResult Add() => View();

        [HttpPost]
        public IActionResult Add(Coupon Model, IFormFile coupon, IFormFile icon)
        {
            var _coupon = new Blob
            {
                Bytes = coupon.ReadAllBytes(),
                ContentLength = coupon.Length,
                ContentType = coupon.ContentType,
                FileName = coupon.FileName,
                Time = DateTime.Now
            };
            DB.Blobs.Add(_coupon);

            var _icon = new Blob
            {
                Bytes = icon.ReadAllBytes(),
                ContentLength = coupon.Length,
                ContentType = coupon.ContentType,
                FileName = coupon.FileName,
                Time = DateTime.Now
            };
            DB.Blobs.Add(_icon);
            DB.SaveChanges();

            Model.ImageId = _coupon.Id;
            Model.ProviderImageId = _icon.Id;
            Model.UserId = User.Current.Id;
            DB.Coupons.Add(Model);
            DB.SaveChanges();
            return Prompt(x =>
            {
                x.Title = "创建成功";
                x.Details = $"优惠券{ Model.Title }已经成功创建！";
            });
        }
    }
}
