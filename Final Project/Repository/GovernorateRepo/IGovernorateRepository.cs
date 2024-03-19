using Final_Project.Models;

namespace Final_Project.Repository.GovernorateRepo
{
    public interface IGovernorateRepository
    {
        void Add(Governorate government);
        void Delete(int id);
        void Edit(Governorate governorate);
        List<Governorate> GetAll();
        Governorate GetById(int id);
        void Save();
    }
}