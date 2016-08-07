using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
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
        [ValidateAntiForgeryToken]
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

        [HttpGet]
        public IActionResult Edit(long id)
        {
            var coupon = DB.Coupons.SingleOrDefault(x => x.Id == id);
            if (!User.IsInRole("Root") && coupon.UserId != User.Current.Id || coupon == null)
                return Prompt(x =>
                {
                    x.Title = "没有找到优惠券";
                    x.Details = "优惠券的相关信息没有找到，请返回重试！";
                });
            return View(coupon);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(long id, Coupon Model, IFormFile coup, IFormFile icon)
        {
            var coupon = DB.Coupons.SingleOrDefault(x => x.Id == id);
            if (!User.IsInRole("Root") && coupon.UserId != User.Current.Id || coupon == null)
                return Prompt(x =>
                {
                    x.Title = "没有找到优惠券";
                    x.Details = "优惠券的相关信息没有找到，请返回重试！";
                });
            coupon.Title = Model.Title;
            coupon.Description = Model.Description;
            coupon.Provider = Model.Provider;
            if (coup != null)
            {
                DB.Blobs
                    .Where(x => x.Id == coupon.ImageId)
                    .Delete();
                var _coupon = new Blob
                {
                    Bytes = coup.ReadAllBytes(),
                    ContentLength = coup.Length,
                    ContentType = coup.ContentType,
                    FileName = coup.FileName,
                    Time = DateTime.Now
                };
                DB.Blobs.Add(_coupon);
                DB.SaveChanges();
                coupon.ImageId = _coupon.Id;
            }
            if (icon != null)
            {
                DB.Blobs
                    .Where(x => x.Id == coupon.ProviderImageId)
                    .Delete();
                var _icon = new Blob
                {
                    Bytes = icon.ReadAllBytes(),
                    ContentLength = icon.Length,
                    ContentType = icon.ContentType,
                    FileName = icon.FileName,
                    Time = DateTime.Now
                };
                DB.Blobs.Add(_icon);
                DB.SaveChanges();
                coupon.ProviderImageId = _icon.Id;
            }
            DB.SaveChanges();
            return Prompt(x =>
            {
                x.Title = "修改成功";
                x.Details = "优惠券信息已更新成功！";
            });
        }
    }
}
