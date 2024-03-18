using Final_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace Final_Project.Repository.BranchRepo
{
    public class BranchRepository : IBranchRepository
    {
        ProjContext context;
        public BranchRepository(ProjContext context)
        {
            this.context = context;

        }
        public void Add(Branch branch)
        {
            context.Branches.Add(branch);
        }

        public void Delete(int id)
        {
            Branch branch = GetById(id);
            branch.IsDeleted = true;
        }

        public void Edit(Branch branch)
        {
            context.Entry(branch).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        }

        public List<Branch> GetAll()
        {
            return context.Branches.ToList();
        }

        public Branch GetById(int id)
        {
            return context.Branches.FirstOrDefault(i => i.Id == id)!;
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}
