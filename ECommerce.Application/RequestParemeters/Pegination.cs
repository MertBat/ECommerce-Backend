﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.RequestParemeters
{
    public record Pegination
    {
        public int Page { get; set; } = 0;
        public int Size { get; set; } = 5;
    }
}
