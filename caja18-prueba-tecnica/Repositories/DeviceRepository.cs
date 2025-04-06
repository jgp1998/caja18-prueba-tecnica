using caja18_prueba_tecnica.Models;
using caja18_prueba_tecnica.Repositories.Interfaces;
using caja18_prueba_tecnica.Utils;

namespace caja18_prueba_tecnica.Repositories
{
    public class DeviceRepository : IDeviceRepository
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<DeviceRepository> _logger;
        public DeviceRepository(HttpClient httpClient, ILogger<DeviceRepository> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<Device>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("objects");
                response.EnsureSuccessStatusCode();

                var devices = await JsonUtils.DeserializeAsync<IEnumerable<Device>>(response.Content);
    
                return devices ?? new List<Device>();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error al realizar la solicitud HTTP para obtener los dispositivos.");
                throw new Exception("Error al obtener dispositivos desde la API externa.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener los dispositivos.");
                throw new Exception("Error inesperado al obtener dispositivos.");
            }
        }


        public async Task<Device?> GetByIdAsync(string id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"objects/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }
                return await JsonUtils.DeserializeAsync<Device>(response.Content);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"Error al realizar la solicitud HTTP para obtener el dispositivo con ID: {id}");
                throw new Exception("Error al obtener el dispositivo desde la API externa.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener el dispositivo.");
                throw new Exception("Error inesperado al obtener el dispositivo.");
            }
        }
    }
}
