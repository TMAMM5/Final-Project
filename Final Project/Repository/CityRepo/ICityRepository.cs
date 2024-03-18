using Final_Project.Models;

namespace Final_Project.Repository.CityRepo
{
    public interface ICityRepository
    {
        void Add(City city);
        void Delete(int id);
        void Edit(City city);
        List<City> GetAll();
        List<City> GetAllCitiesByGovId(int id);
        City GetById(int id);
        void Save();
    }
}