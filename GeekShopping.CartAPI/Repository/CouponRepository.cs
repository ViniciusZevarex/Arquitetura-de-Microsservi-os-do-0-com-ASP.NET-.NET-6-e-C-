using AutoMapper;
using GeekShopping.CartAPI.Data.ValueObjects;
using GeekShopping.CartAPI.Model.Context;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Net;
using System.Text.Json;

namespace GeekShopping.CartAPI.Repository
{
    public class CouponRepository : ICouponRepository
    {
        private readonly HttpClient _httpClient;
        
        public const string BasePath = "api/v1/coupon";

        public CouponRepository(
            HttpClient httpClient
        )
        {
            _httpClient = httpClient;
        }

        public async Task<CouponVO> GetCouponByCouponCode(string couponCode, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"{BasePath}/{couponCode}");

            if (response.StatusCode != HttpStatusCode.OK)
                return new CouponVO();

            var content = await response.Content.ReadAsStringAsync();
            var couponVO = JsonSerializer.Deserialize<CouponVO>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return couponVO;
        }
    }
}
