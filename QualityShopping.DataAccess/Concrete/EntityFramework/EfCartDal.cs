using Microsoft.EntityFrameworkCore;
using QualityShopping.DataAccess.Abstract;
using QualityShopping.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QualityShopping.DataAccess.Concrete.EntityFramework
{
    public class EfCartDal : EfGenericRepository<Cart, ShopContext>, ICartRepository
    {
        public override void Update(Cart entity)
        {
            using (var context = new ShopContext())
            {
                context.Carts.Update(entity);
                context.SaveChanges();
            }
        }
        public Cart GetByUserId(string userId)
        {
            using (var context = new ShopContext())
            {
                return context
                            .Carts
                            .Include(i => i.CartItems)
                            .ThenInclude(i => i.Product)
                            .FirstOrDefault(i => i.UserId == userId);
            }
        }

        public void DeleteFromCart(int cartId, int productId)
        {
            using (var context = new ShopContext())
            {
                var cmd = @"delete from CartItem where CartId=@p0 and ProductId=@p1";
                context.Database.ExecuteSqlRaw(cmd, cartId, productId);
            }
        }

        public void ClearCart(string cartId)
        {
            using (var context = new ShopContext())
            {
                var cmd = @"delete from CartItem where CartId=@p0";
                context.Database.ExecuteSqlRaw(cmd, cartId);
            }
        }
    }
}
