﻿using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Persistance.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Persistance.Repositories
{
    public class MenuWriteRepository : WriteRepository<Menu>, IMenuWriteRepository
    {
        public MenuWriteRepository(ECommerceAPIDbContext context) : base(context)
        {
        }
    }
}
