using Final_Project.Models;

namespace Final_Project.Repository.RepresintativeRepo
{
    public interface IRepresintativeRepository
    {
        void Add(Representative representative);
        void Delete(string id);
        List<Representative> GetAll();
        List<Representative> GetByBranch(int branchId);
        Representative GetById(string id);
        void Save();
        void Update(Representative representative);
    }
}