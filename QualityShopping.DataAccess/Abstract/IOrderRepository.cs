using System;
using System.Collections.Generic;
using System.Text;
using QualityShopping.Entity;

namespace QualityShopping.DataAccess.Abstract
{
    public interface IOrderRepository : IRepository<Order>
    {
        List<Order> GetOrders(string userId);
    }
}
