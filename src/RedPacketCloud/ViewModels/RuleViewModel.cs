﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedPacketCloud.ViewModels
{
    public class Rule
    {
        /// <summary>
        /// 以分为单位
        /// </summary>
        public int From { get; set; }

        /// <summary>
        /// 以分为单位
        /// </summary>
        public int To { get; set; }

        public double Ratio { get; set; }

        public long Count { get; set; }
    }
}
