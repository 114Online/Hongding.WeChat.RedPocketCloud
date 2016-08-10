using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedPocketCloud.ViewModels
{
    public class ActivityViewModel
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public DateTime Begin { get; set; }
        public DateTime? End { get; set; }
        public long Price { get; set; }
        public long BriberiesCount { get; set; }
        public double Ratio { get; set; }
        public string Merchant { get; set; }
    }
}
