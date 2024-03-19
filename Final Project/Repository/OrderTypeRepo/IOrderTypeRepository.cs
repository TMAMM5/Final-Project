using Final_Project.Models;

namespace Final_Project.Repository.OrderTypeRepo
{
    public interface IOrderTypeRepository
    {
        List<OrderType> GetAll();
    }
}