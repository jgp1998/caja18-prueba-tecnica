using caja18_prueba_tecnica.Models;
using caja18_prueba_tecnica.Repositories.Interfaces;
using caja18_prueba_tecnica.Utils;
using System.Net.Http;

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
            return (await JsonUtils.DeserializeAsync<IEnumerable<Device>>(response.Content))!;
        }

        public async Task<Device?> GetByIdAsync(string id)
        {
            var response = await _httpClient.GetAsync($"objects/{id}");
            if (!response.IsSuccessStatusCode) return null;
            return await JsonUtils.DeserializeAsync<Device>(response.Content);
        }
    }
}
