using System.Net.Http.Json;
using Pawtopia.Client.DTOs; // Đảm bảo đúng namespace chứa GetProduct, CreateProduct...

namespace Pawtopia.Client.Services
{
    public class ProductService
    {
        private readonly HttpClient _http;

        public ProductService(HttpClient http)
        {
            _http = http;
        }

        // 1. Lấy toàn bộ sản phẩm để hiện lên Shop
        public async Task<List<GetProduct>> GetAllAsync()
        {
            var result = await _http.GetFromJsonAsync<List<GetProduct>>("api/product/all");
            return result ?? new List<GetProduct>();
        }

        // 2. Thêm sản phẩm mới (Yêu cầu Role Admin)
        public async Task<bool> AddAsync(CreateProduct product)
        {
            var response = await _http.PostAsJsonAsync("api/product/add", product);
            return response.IsSuccessStatusCode;
        }

        // 3. Xóa sản phẩm
        public async Task<bool> DeleteAsync(Guid id)
        {
            var response = await _http.DeleteAsync($"api/product/delete/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}