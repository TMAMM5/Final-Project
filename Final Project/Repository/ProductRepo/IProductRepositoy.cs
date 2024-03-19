using Final_Project.Models;

namespace Final_Project.Repository.ProductRepo
{
    public interface IProductRepositoy
    {
        void Add(Product product);
        void Delete(int id);
        Product GetById(int id);
        List<Product> GetByOrderNumber(string orderNumber);
        List<Product> GetProducts();
        void Save();
        void Update(Product product);
    }
}