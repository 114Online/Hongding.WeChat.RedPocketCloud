using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;

namespace Microsoft.AspNetCore.Mvc.Rendering
{
    public static class MoneyDisplayExtensions
    {
        public static HtmlString Money(this IHtmlHelper self, double money)
        {
            return new HtmlString(money.ToString("￥0.00"));
        }

        public static HtmlString Money(this IHtmlHelper self, long money)
        {
            return new HtmlString((money / 100.0).ToString("0.00"));
        }
    }
}
