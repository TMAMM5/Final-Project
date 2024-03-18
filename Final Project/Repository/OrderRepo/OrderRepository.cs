using Final_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace Final_Project.Repository.OrderRepo
{
    public class OrderRepository : IOrderRepository
    {
        private ProjContext _context;
        public OrderRepository(ProjContext context)
        {
            _context = context;
        }

        public List<Order> GetAll()
        {
            return _context.Orders.Where(o => o.IsDeleted == false).ToList();
        }
        public Order GetById(int id)
        {
            return _context.Orders.FirstOrDefault(o => o.Id == id && o.IsDeleted == false);
        }

        public void Update(Order order)
        {
            _context.Orders.Update(order);
        }

        public void Delete(int id)
        {
            Order order = _context.Orders.Find(id);
            order.IsDeleted = true;
        }
        public void Save()
        {
            _context.SaveChanges();
        }

        public decimal? CalculateTotalPrice(Order order)
        {
            decimal? Price = 0;

            var CityId = order.ClientCityId;
            var DeliverTypeId = order.DeliveryTypeId;
            Price += CalculateCityPrice(CityId);
            Price += CalculateDeliveryTypePrice(order.DeliveryTypeId);
            Price += CalculateDeliverToVillagePrice(order);
            Price += CalculateExtraWeightPrice(order);
            return Price;

        }

        public decimal? CalculateCityPrice(int? id)
        {
            City? city = _context.Cities.SingleOrDefault(c => c.Id == id);
            decimal? cityPrice = city?.ShippingCost;
            return cityPrice;
        }

        public decimal? CalculateDeliveryTypePrice(int? id)
        {
            DeliverType deliverType = _context.DeliverTypes.SingleOrDefault(c => c.Id == id);
            decimal? deliveryPrice = deliverType.Price;
            return deliveryPrice;
        }

        public decimal CalculateDeliverToVillagePrice(Order order)
        {
            decimal deliverToVillagePrice;
            if (order.DeliverToVillage == true)
            {
                deliverToVillagePrice = 50;
            }
            else
            {
                deliverToVillagePrice = 0;
            }
            return deliverToVillagePrice;
        }

        public decimal? CalculateExtraWeightPrice(Order order)
        {
            var defaultWeight = _context.WeightSetting.Select(ws => ws.DefaultSize).FirstOrDefault();
            var priceForExtraKilo = _context.WeightSetting.Select(ws => ws.PriceForEachExtraKilo).FirstOrDefault();
            decimal? price = 0;

            if (defaultWeight < order.TotalWeight)
                price = (order.TotalWeight - defaultWeight) * priceForExtraKilo;
            else
            {
                price = 0;
            }
            return price;
        }

        public List<Order> GetByOrderState(int stateId)
        {
            return _context.Orders.Where(o=>o.OrderStateId == stateId && o.IsDeleted == false).ToList();
        }

        public List<Order> GetByRepresentativeId(string representativeId)
        {
            return _context.Orders.Where(o => o.RepresentativeId == representativeId && o.IsDeleted == false && o.OrderStateId != 4).ToList();
        }

    }
}
