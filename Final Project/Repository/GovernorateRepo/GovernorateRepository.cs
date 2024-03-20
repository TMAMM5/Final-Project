using Final_Project.Models;
namespace Final_Project.Repository.GovernorateRepo
{
    public class GovernorateRepository : IGovernorateRepository
    {
        private ProjContext _context;
        public GovernorateRepository(ProjContext context)
        {
            _context = context;
        }

        public void Add(Governorate government)
        {
            _context.governorates.Add(government);
        }

        public void Delete(int id)
        {
            Governorate governorate = GetById(id);
            governorate.IsDeleted = true;
            //_context.governorates.Remove(governorate);
            //Save();
        }

      public void Update(Governorate governorate)
        {
            _context.governorates.Update(governorate);
        }

        public List<Governorate> GetAll()
        {
            return _context.governorates.ToList();
        }

        public Governorate GetById(int id)
        {
            return _context.governorates.FirstOrDefault(g=>g.Id==id);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

    }
}
