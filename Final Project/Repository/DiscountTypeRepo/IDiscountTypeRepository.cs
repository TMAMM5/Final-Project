using Final_Project.Models;

namespace Final_Project.Repository.DiscountTypeRepo
{
    public interface IDiscountTypeRepository
    {
        void Add(DiscountType discount);
        void Delete(int id);
        void Edit(DiscountType discount);
        List<DiscountType> GetAll();
        DiscountType GetById(int id);
        void Save();
    }
}