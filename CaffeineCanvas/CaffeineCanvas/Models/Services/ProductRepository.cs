using CaffeineCanvas.Data;
using CaffeineCanvas.Models.Interfaces;

namespace CaffeineCanvas.Models.Services
{
    public class ProductRepository : IProductRepository
    {
        private CaffeineCanvasDBContext dbContext;

        public ProductRepository(CaffeineCanvasDBContext dbContext)
        {
            this.dbContext = dbContext;    
        }

        public IEnumerable<Product> GetAllProducts()
        {
            return dbContext.Products;
        }

        public Product? GetProductDetail(int id)
        {
            return dbContext.Products.FirstOrDefault(p => p.Id == id);
        }

        public IEnumerable<Product> GetTrendingProducts()
        {
            return dbContext.Products.Where(p => p.IsTrendingProduct);
        }
    }
}
