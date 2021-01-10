using Microsoft.EntityFrameworkCore;
using QualityShopping.DataAccess.Abstract;
using QualityShopping.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QualityShopping.DataAccess.Concrete.EntityFramework
{
    public class EfOrderDal : EfGenericRepository<Order, ShopContext>, IOrderRepository
    {
        public List<Order> GetOrders(string userId)
        {
            using (var context = new ShopContext())
            {
                var orders = context.Orders
                    .Include(x => x.OrderItems)
                    .ThenInclude(x => x.Product)
                    .AsQueryable();
                if (!string.IsNullOrEmpty(userId))
                {
                    orders = orders.Where(x => x.UserId == userId);
                }
                return orders.ToList();
            }
        }
    }
}
