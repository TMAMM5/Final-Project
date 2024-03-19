using Final_Project.Models;
namespace Final_Project.Repository.PaymentMethodRepo
{
    public class PaymentMethodRepository : IPaymentMethodRepository
    {
        private ProjContext _context;
        public PaymentMethodRepository(ProjContext context)
        {
            _context = context;
        }
        public List<PaymentMethod> GetAll()
        {
            return _context.PaymentMethods.ToList();
        }
    }
}
