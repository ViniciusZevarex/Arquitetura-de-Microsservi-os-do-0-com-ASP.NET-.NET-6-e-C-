using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
using System.Net.Http.Headers;
using System.Net.Http;
using GeekShopping.Web.Utils;
using System.Net;

namespace GeekShopping.Web.Services
{
    public class CouponService : ICouponService
    {
        private readonly HttpClient _httpClient;
        public const string BasePath = "api/v1/coupon";

        public CouponService(
            HttpClient httpClient
        )
        {
            _httpClient = httpClient;
        }


        public async Task<CouponViewModel> GetCoupon(string code, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"{BasePath}/{code}");

            if(response.StatusCode != HttpStatusCode.OK) { 
                return new CouponViewModel(); 
            }

            return await response.ReadContentAsync<CouponViewModel>();
        }
    }
}
