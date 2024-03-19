using Final_Project.Models;
namespace Final_Project.Repository.DiscountTypeRepo
{
    public class DiscountTypeRepository : IDiscountTypeRepository
    {

        private ProjContext _context;
        public DiscountTypeRepository(ProjContext context)
        {
            _context = context;
        }

        public void Add(DiscountType discount)
        {
            _context.DiscountTypes.Add(discount);
        }

        public void Delete(int id)
        {
            DiscountType discount = GetById(id);
            _context.DiscountTypes.Remove(discount);
            Save();

        }

        public void Update(DiscountType discount)
        {
            _context.DiscountTypes.Update(discount);
        }

        public List<DiscountType> GetAll()
        {
            return _context.DiscountTypes.ToList();
        }

        public DiscountType GetById(int id)
        {
            return _context.DiscountTypes.FirstOrDefault(dt=>dt.Id == id);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

    }
}
