using Final_Project.Models;

namespace Final_Project.Repository.PaymentMethodRepo
{
    public interface IPaymentMethodRepository
    {
        List<PaymentMethod> GetAll();
    }
}