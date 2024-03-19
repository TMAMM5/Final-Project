using Final_Project.Models;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Final_Project.Repository.ProductRepo
{
    public class ProductRepositoy : IProductRepositoy
    {
        ProjContext _context;
        public ProductRepositoy(ProjContext context)
        {
            _context = context;
        }
        public List<Product> GetProducts()
        {
            return _context.Products.ToList();
        }
        public List<Product> GetByOrderNumber(string orderNumber)
        {
            return _context.Products.Where(p => p.OrderNO == orderNumber).ToList();
        }
        public Product GetById(int id)
        {
            return _context.Products.FirstOrDefault(p => p.Id == id);
        }
        public void Add(Product product)
        {
            _context.Products.Add(product);
        }
        public void Update(Product product)
        {
            _context.Products.Update(product);
        }
        public void Delete(int id)
        {
            Product product = GetById(id);
            _context.Products.Remove(product);
            Save();
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
