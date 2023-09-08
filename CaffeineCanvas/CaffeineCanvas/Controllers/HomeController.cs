using CaffeineCanvas.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CaffeineCanvas.Controllers
{
    public class HomeController : Controller
    {
        private IProductRepository productRepository;

        public HomeController(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }
        public IActionResult Index()
        {
            return View(productRepository.GetTrendingProducts());
        
        }
    }
}
