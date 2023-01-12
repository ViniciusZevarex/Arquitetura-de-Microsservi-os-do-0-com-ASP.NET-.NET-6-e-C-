using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
using GeekShopping.Web.Utils;
using System.Net.Http.Headers;

namespace GeekShopping.Web.Services
{
    public class ProductService : IProductService
    {

        private readonly HttpClient _httpClient;
        public const string BasePath = "api/v1/product";

        public ProductService(
            HttpClient httpClient
        )
        {
            _httpClient = httpClient;
        }


        public async Task<ProductViewModel> CreateProduct(ProductViewModel product, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PostAsJson(BasePath, product);
            if (response.IsSuccessStatusCode) return await response.ReadContentAsync<ProductViewModel>();
            else throw new ApplicationException("Something went wrong when calling API");
        }

        public async Task<ProductViewModel> UpdateProduct(ProductViewModel product, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PutAsJson(BasePath, product);
            if (response.IsSuccessStatusCode) return await response.ReadContentAsync<ProductViewModel>();
            else throw new ApplicationException("Something went wrong when calling API");
        }

        public async Task<bool> DeleteProductById(long id, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.DeleteAsync($"{BasePath}/{id}");
            if (response.IsSuccessStatusCode)
                return await response.ReadContentAsync<bool>();
            else throw new Exception("Something went wrong when calling API");
        }

        public async Task<IEnumerable<ProductViewModel>> FindAllProducts(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync(BasePath);
            return await response.ReadContentAsync<List<ProductViewModel>>();
        }

        public async Task<ProductViewModel> FindProductById(long id, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"{BasePath}/{id}");
            return await response.ReadContentAsync<ProductViewModel>();
        }

    }
}
