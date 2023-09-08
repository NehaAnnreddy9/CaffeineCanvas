using CaffeineCanvas.Data;
using CaffeineCanvas.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CaffeineCanvas.Models.Services
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private CaffeineCanvasDBContext dbContext;

        public ShoppingCartRepository(CaffeineCanvasDBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public List<ShoppingCartItem>? ShoppingCartItems { get; set; }

        public string? ShoppingCartId { get; set; }

        public static ShoppingCartRepository GetCart(IServiceProvider services) //Can be executed automatically when an object of product repository class is created
        {
            ISession? session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext?.Session;

            CaffeineCanvasDBContext context = services.GetService<CaffeineCanvasDBContext>() ?? throw new Exception("Error initializing coffeeshopdbcontext");

            string cartId = session?.GetString("CartId") ?? Guid.NewGuid().ToString();

            session?.SetString("CartId", cartId);

            return new ShoppingCartRepository(context) { ShoppingCartId = cartId };
        }

        public void AddToCart(Product product)
        {
            var shoppingCartItem = dbContext.ShoppingCartItems.FirstOrDefault(s => s.Product.Id == product.Id && s.ShoppingCartId == ShoppingCartId);
            if (shoppingCartItem == null) 
            {
                shoppingCartItem = new ShoppingCartItem
                {
                    ShoppingCartId = ShoppingCartId,
                    Product = product,
                    Qty = 1  
                };
                dbContext.ShoppingCartItems.Add(shoppingCartItem);
            }
            else
            {
                shoppingCartItem.Qty++;
            }
            dbContext.SaveChanges();
        }

        public void ClearCart()
        {
            var cartItems = dbContext.ShoppingCartItems.Where(s => s.ShoppingCartId == ShoppingCartId);
            dbContext.ShoppingCartItems.RemoveRange(cartItems);
            dbContext.SaveChanges();
        }

        public List<ShoppingCartItem> GetShoppingCartItems()
        {
           return ShoppingCartItems ??= dbContext.ShoppingCartItems.Where(s => s.ShoppingCartId == ShoppingCartId)
                .Include(p=>p.Product).ToList();
        }

        public int RemoveFromCart(Product product)
        {
            var shoppingCartItem = dbContext.ShoppingCartItems.FirstOrDefault(s => s.Product.Id == product.Id && s.ShoppingCartId == ShoppingCartId);
            var quantity = 0;

            if (shoppingCartItem != null)
            {
                if (shoppingCartItem.Qty > 1)
                {
                    shoppingCartItem.Qty--;
                    quantity = shoppingCartItem.Qty;
                }
                else
                {
                    dbContext.ShoppingCartItems.Remove(shoppingCartItem);
                }
            }
            dbContext.SaveChanges();
            return quantity;
        }

        public decimal GetShoppingCartTotal()
        {
            var totalCost = dbContext.ShoppingCartItems.Where(s => s.ShoppingCartId == ShoppingCartId)
                .Select(s=>s.Product.Price * s.Qty).Sum();
            return totalCost;
        }

     
    }
}
