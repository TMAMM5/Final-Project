using Final_Project.Models;
using Microsoft.EntityFrameworkCore;
namespace Final_Project.Repository.OrderTypeRepo
{
    public class OrderTypeRepository : IOrderTypeRepository
    {
        private ProjContext _context;
        public OrderTypeRepository(ProjContext context)
        {
            _context = context;
        }
        public List<OrderType> GetAll()
        {
            return _context.OrderTypes.ToList();
        }
    }
}
