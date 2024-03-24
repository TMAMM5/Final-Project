using Final_Project.Models;

namespace Final_Project.Repository.OrderRepo
{
    public interface IOrderRepository
    {
        decimal? CalculateCityPrice(int? id);
        decimal CalculateDeliverToVillagePrice(Order order);
        decimal? CalculateDeliveryTypePrice(int? id);
        decimal? CalculateExtraWeightPrice(Order order);
        decimal? CalculateTotalPrice(Order order);
        void Delete(int id);
        List<Order> GetAll();
        void Add(Order order);
        public List<Order> GetByOrderState(int stateId);
        public List<Order> GetByRepresentativeId(string representativeId);
        Order GetById(int id);
        void Save();
        void Update(Order order);
    }
}