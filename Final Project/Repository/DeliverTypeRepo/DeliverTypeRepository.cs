using Final_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace Final_Project.Repository.DeliverTypeRepo
{
    public class DeliverTypeRepository : IDeliverTypeRepository
    {
        ProjContext context;
        public DeliverTypeRepository(ProjContext context)
        {
            this.context = context;
        }

        public List<DeliverType> GetAll()
        {
            return context.DeliverTypes.ToList();
        }
    }
}
