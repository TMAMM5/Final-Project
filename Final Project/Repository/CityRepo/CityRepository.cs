using Final_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace Final_Project.Repository.CityRepo
{
    public class CityRepository : ICityRepository
    {
        ProjContext context;
        public CityRepository(ProjContext context)
        {
            this.context = context;
        }
        public void Add(City city)
        {
            context.Cities.Add(city);
        }

        public void Delete(int id)
        {
            City City = GetById(id);
            context.Cities.Remove(City);

        }

        public List<City> GetAllCitiesByGovId(int id)
        {
            return context.Cities.Where(c => c.GoverId == id).Select(c=>new City { Id=c.Id , Name=c.Name , PickUpCost=c.PickUpCost , ShippingCost=c.ShippingCost}).ToList();
        }

        public void Edit(City city)
        {
            context.Entry(city).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        }

        public List<City> GetAll()
        {
            return context.Cities.ToList();
        }

        public City GetById(int id)
        {
            return context.Cities.Find(id);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}
