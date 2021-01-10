using QualityShopping.Business.Abstract;
using QualityShopping.DataAccess.Abstract;
using QualityShopping.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace QualityShopping.Business.Concrete
{
    public class OrderManager : IOrderService
    {
        private IOrderRepository _orderRepository;
        public OrderManager(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        public void Create(Order entity)
        {
            _orderRepository.Create(entity);
        }

        public List<Order> GetOrders(string userId)
        {
            return _orderRepository.GetOrders(userId);
        }
    }
}
