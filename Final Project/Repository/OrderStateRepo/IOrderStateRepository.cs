using Final_Project.Models;

namespace Final_Project.Repository.OrderStateRepo
{
    public interface IOrderStateRepository
    {
        OrderState getById(int id);
        List<OrderState> GetForEmp();
        List<OrderState> GetOrders();
        List<OrderState> GetOrdersForRep();
    }
}