﻿using QualityShopping.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace QualityShopping.Business.Abstract
{
    public interface IOrderService
    {
        void Create(Order entity);
        List<Order> GetOrders(string userId);
    }
}
