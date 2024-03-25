using Final_Project.Models;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore;

namespace Final_Project.Repository.RepresintativeRepo
{
    public class RepresintativeRepository : IRepresintativeRepository
    {
        ProjContext context;
        public RepresintativeRepository(ProjContext context)
        {
            this.context = context;

        }
        public List<Representative> GetAll()
        {
            return context.Representatives.ToList();
        }
        public Representative GetById(string id)
        {
            return context.Representatives.FirstOrDefault(r => r.AppUserId == id);
        }
        public List<Representative> GetByBranch(int branchId)
        {
            return context.Representatives.Where(r => r.BranchId == branchId).ToList();
        }
        public void Add(Representative representative)
        {
            context.Representatives.Add(representative);
        }
        public void Update(Representative representative)
        {
            context.Representatives.Update(representative);
        }
        public void Delete(string id)
        {
            Representative rep = GetById(id);
            //rep.IsDeleted = true;
            context.Representatives.Remove(rep);
            Save();
        }
        public void Save()
        {
            context.SaveChanges();
        }

        }
    }
}
