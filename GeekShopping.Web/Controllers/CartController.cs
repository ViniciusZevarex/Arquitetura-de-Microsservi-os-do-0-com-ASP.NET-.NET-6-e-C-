using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.Web.Controllers
{
    public class CartController : Controller
    {

        private readonly ILogger<CartController> _logger;
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        private readonly ICouponService _couponService;

        public CartController(
                ILogger<CartController> logger,
                IProductService productService,
                ICartService cartService,
                ICouponService couponService
        )
        {
            _logger = logger;
            _productService = productService;
            _cartService = cartService;
            _couponService = couponService;
        }

        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            var response = await findUserCart();

            return View(response);
        }


        [Authorize]
        [HttpPost]
        [ActionName("ApplyCoupon")]
        public async Task<IActionResult> ApplyCoupon(CartViewModel model)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;

            var response = await _cartService.ApplyCoupon(model, token);

            if (response)
            {
                return RedirectToAction(nameof(CartIndex));
            }

            return View(response);
        }

        [Authorize]
        [HttpPost]
        [ActionName("RemoveCoupon")]
        public async Task<IActionResult> RemoveCoupon()
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;

            var response = await _cartService.RemoveCoupon(userId, token);

            if (response)
            {
                return RedirectToAction(nameof(CartIndex));
            }

            return View(response);
        }


        [Authorize]
        public async Task<IActionResult> Remove(int id)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;

            var response = await _cartService.RemoveFromCart(id, token);
            
            if(response)
            {
                return RedirectToAction(nameof(CartIndex));
            }

            return View(response);
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var response = await findUserCart();

            return View(response);
        }
        

        
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Checkout(CartViewModel model)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;

            var response = await _cartService.Checkout(model.CartHeader, token);

            if(response != null)
            {
                return RedirectToAction(nameof(Confirmation));
            }


            return View(model);
        }



        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Confirmation()
        {
            return View();
        }



        private async Task<CartViewModel> findUserCart()
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;

            var response = await _cartService.FindCartByUserId(userId, token);

            if (response?.CartHeader != null)
            {
                if (!string.IsNullOrEmpty(response.CartHeader.CouponCode))
                {
                    var coupon = await _couponService.GetCoupon(response.CartHeader.CouponCode, token);
                    
                    if(coupon?.CouponCode != null)
                    {
                        response.CartHeader.DiscountAmount = coupon.DiscountAmount;
                    }
                    
                }

                foreach (var item in response.CartDetails)
                {
                    response.CartHeader.PurchaseAmount += item.Product.Price * item.Count;
                }

                response.CartHeader.PurchaseAmount -= response.CartHeader.DiscountAmount;
            }

            return response;
        }
    }
}
