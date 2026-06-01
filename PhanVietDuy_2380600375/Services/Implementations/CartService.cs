using Microsoft.EntityFrameworkCore;
using PhanVietDuy_2380600375.Data;
using PhanVietDuy_2380600375.Models.Domain;
using PhanVietDuy_2380600375.Models.DTOs;
using PhanVietDuy_2380600375.Services.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PhanVietDuy_2380600375.Services.Implementations
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;
        private static readonly ConcurrentDictionary<int, SemaphoreSlim> _productLocks = new();

        public CartService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CartViewModel> GetCartAsync(string sessionKey)
        {
            var items = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.SessionKey == sessionKey)
                .ToListAsync();

            decimal subTotal = items.Sum(i => (i.Product?.Price ?? 0) * i.Quantity);
            decimal shippingFee = subTotal >= 1000 ? 0 : 50; // $50 default, free over 1000

            return new CartViewModel
            {
                Items = items,
                Subtotal = subTotal,
                ShippingFee = shippingFee,
                Total = subTotal + shippingFee
            };
        }

        public async Task AddItemAsync(string sessionKey, AddToCartRequest req, string? userId = null)
        {
            if (!string.IsNullOrEmpty(req.SelectedVariant))
            {
                var parts = req.SelectedVariant.Split(new[] { " / " }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2)
                {
                    req.SelectedColor = parts[0].Trim();
                    req.SelectedSize = parts[1].Trim();
                }
                else if (parts.Length == 1)
                {
                    req.SelectedColor = parts[0].Trim();
                }
            }

            var semaphore = _productLocks.GetOrAdd(req.ProductId, _ => new SemaphoreSlim(1, 1));
            await semaphore.WaitAsync();

            try
            {
                var product = await _context.Products.FindAsync(req.ProductId);
                if (product == null || !product.IsActive)
                    throw new Exception("Product is not available.");

                var existingItem = await _context.CartItems.FirstOrDefaultAsync(c => 
                    c.SessionKey == sessionKey && 
                    c.ProductId == req.ProductId &&
                    c.SelectedColor == req.SelectedColor &&
                    c.SelectedSize == req.SelectedSize);

                int newQuantity = req.Quantity + (existingItem?.Quantity ?? 0);

                if (newQuantity > product.StockQuantity)
                    throw new Exception($"Cannot add more items. Only {product.StockQuantity} left in stock.");

                if (existingItem != null)
                {
                    existingItem.Quantity = newQuantity;
                }
                else
                {
                    _context.CartItems.Add(new CartItem
                    {
                        SessionKey = sessionKey,
                        UserId = userId,
                        ProductId = req.ProductId,
                        Quantity = req.Quantity,
                        SelectedColor = req.SelectedColor,
                        SelectedSize = req.SelectedSize
                    });
                }

                // Handling DbUpdateConcurrencyException in case stock was updated concurrently by checkout
                bool saved = false;
                int retries = 3;
                while (!saved && retries > 0)
                {
                    try
                    {
                        await _context.SaveChangesAsync();
                        saved = true;
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        retries--;
                        if (retries == 0) throw new Exception("Concurrency conflict occurred while updating cart.", ex);
                        
                        // Reload entity values and try again
                        foreach (var entry in ex.Entries)
                        {
                            await entry.ReloadAsync();
                        }
                    }
                }
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task UpdateQuantityAsync(string sessionKey, int cartItemId, int qty)
        {
            var cartItem = await _context.CartItems.Include(c => c.Product).FirstOrDefaultAsync(c => c.Id == cartItemId && c.SessionKey == sessionKey);
            if (cartItem == null) throw new Exception("Cart item not found or unauthorized");

            if (qty <= 0)
            {
                _context.CartItems.Remove(cartItem);
            }
            else
            {
                if (cartItem.Product != null && qty > cartItem.Product.StockQuantity)
                    throw new Exception($"Cannot update quantity. Only {cartItem.Product.StockQuantity} left in stock.");

                cartItem.Quantity = qty;
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveItemAsync(string sessionKey, int cartItemId)
        {
            var cartItem = await _context.CartItems.FirstOrDefaultAsync(c => c.Id == cartItemId && c.SessionKey == sessionKey);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetCountAsync(string sessionKey)
        {
            return await _context.CartItems
                .Where(c => c.SessionKey == sessionKey)
                .SumAsync(c => c.Quantity);
        }

        public async Task MergeAsync(string guestKey, string userKey, string userId)
        {
            if (guestKey == userKey) return;

            var guestItems = await _context.CartItems.Where(c => c.SessionKey == guestKey).ToListAsync();
            var userItems = await _context.CartItems.Where(c => c.SessionKey == userKey).ToListAsync();

            foreach (var guestItem in guestItems)
            {
                var existing = userItems.FirstOrDefault(u => 
                    u.ProductId == guestItem.ProductId && 
                    u.SelectedColor == guestItem.SelectedColor && 
                    u.SelectedSize == guestItem.SelectedSize);

                if (existing != null)
                {
                    existing.Quantity += guestItem.Quantity;
                    _context.CartItems.Remove(guestItem);
                }
                else
                {
                    guestItem.SessionKey = userKey;
                    guestItem.UserId = userId;
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task ClearAsync(string sessionKey)
        {
            var items = await _context.CartItems.Where(c => c.SessionKey == sessionKey).ToListAsync();
            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();
        }
    }
}
