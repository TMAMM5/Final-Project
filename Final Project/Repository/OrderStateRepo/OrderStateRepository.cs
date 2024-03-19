using Final_Project.Models;

namespace Final_Project.Repository.OrderStateRepo
{
    public class OrderStateRepository : IOrderStateRepository
    {
        ProjContext context;
        public OrderStateRepository(ProjContext context)
        {
            this.context = context;
        }
        public List<OrderState> GetOrders()
        {
            return context.OrderStates.ToList();
        }

        public List<OrderState> GetForEmp()
        {

            return context.OrderStates.Where(o => o.Id < 4 && o.Id > 1).ToList();

        }
        public OrderState getById(int id)
        {
            return context.OrderStates.FirstOrDefault(o => o.Id == id);
        }


    }
}
