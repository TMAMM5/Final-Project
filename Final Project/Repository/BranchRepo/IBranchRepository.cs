using Final_Project.Models;

namespace Final_Project.Repository.BranchRepo
{
    public interface IBranchRepository
    {
        void Add(Branch branch);
        void Delete(int id);
        void Edit(Branch branch);
        List<Branch> GetAll();
        Branch GetById(int id);
        void Save();
    }
}