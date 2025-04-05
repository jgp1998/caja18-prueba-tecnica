using caja18_prueba_tecnica.Models;
using caja18_prueba_tecnica.Repositories.Interfaces;
using System.Text.Json;

namespace caja18_prueba_tecnica.Repositories
{
    public class DeviceRepository : IDeviceRepository
    {
        private readonly HttpClient _httpClient;

        public DeviceRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Device>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync("objects");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<Device>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;
        }

        public async Task<Device?> GetByIdAsync(string id)
        {
            var response = await _httpClient.GetAsync($"/objects/{id}");
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Device>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }

}
