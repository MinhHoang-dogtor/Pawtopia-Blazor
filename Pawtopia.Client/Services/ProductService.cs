using System.Net.Http.Json;
using Pawtopia.Client.DTOs;

namespace Pawtopia.Client.Services
{
    public class ProductService
    {
        private readonly HttpClient _http;
        public ProductService(HttpClient http) => _http = http;

        // Lấy tất cả sản phẩm
        public async Task<List<GetProduct>> GetProductsAsync()
            => await _http.GetFromJsonAsync<List<GetProduct>>("api/product/all") ?? new();

        // Thêm mới
        public async Task<bool> AddProductAsync(CreateProduct dto)
        {
            var res = await _http.PostAsJsonAsync("api/product/add", dto);
            return res.IsSuccessStatusCode;
        }

        // Cập nhật (Sửa thông tin hoặc Ẩn/Hiện qua IsActive)
        public async Task<bool> UpdateProductAsync(GetProduct dto)
        {
            var res = await _http.PutAsJsonAsync("api/product/update", dto);
            return res.IsSuccessStatusCode;
        }

        // Xóa vĩnh viễn
        public async Task<bool> DeletePermanentAsync(string id)
        {
            var res = await _http.DeleteAsync($"api/product/delete-permanent/{id}");
            return res.IsSuccessStatusCode;
        }

        // Bật/Tắt IsActive nhanh
        public async Task<bool> ToggleActiveAsync(string id)
        {
            var res = await _http.PatchAsync($"api/product/toggle-active/{id}", null);
            return res.IsSuccessStatusCode;
        }
    }
}