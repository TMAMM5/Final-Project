using Final_Project.Models;

namespace Final_Project.Repository.TraderRepo
{
    public interface ITraderRepository
    {
        void Add(Trader trader);
        void Delete(string id);
        List<Trader> GetAll();
        Trader GetById(string id);
        void Save();
        void Update(Trader trader);
    }
}