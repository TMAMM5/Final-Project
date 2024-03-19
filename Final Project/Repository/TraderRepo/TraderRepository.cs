using Final_Project.Models;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Final_Project.Repository.TraderRepo
{
    public class TraderRepository : ITraderRepository
    {
        ProjContext context;
        public TraderRepository(ProjContext context)
        {
            this.context = context;

        }
        public List<Trader> GetAll()
        {
            return context.Traders.ToList();
        }
        public Trader GetById(string id)
        {
            return context.Traders.FirstOrDefault(t => t.AppUserId == id);
        }
        public void Add(Trader trader)
        {
            context.Traders.Add(trader);
        }
        public void Update(Trader trader)
        {
            context.Traders.Update(trader);
        }
        public void Delete(string id)
        {
            Trader trader = GetById(id);
            context.Traders.Remove(trader);
            Save();
        }
        public void Save()
        {
            context.SaveChanges();
        }
    }
}
