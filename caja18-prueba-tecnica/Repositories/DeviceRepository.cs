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

                 if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests || response.StatusCode == System.Net.HttpStatusCode.MethodNotAllowed)
                {
                    _logger.LogWarning("Límite de solicitudes alcanzado. No se pueden obtener los dispositivos.");
                return new List<Device>();
                }

               response.EnsureSuccessStatusCode();

                var devices = await JsonUtils.DeserializeAsync<IEnumerable<Device>>(response.Content);

                return devices ?? new List<Device>(); 
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error al realizar la solicitud HTTP para obtener los dispositivos.");
               
                return new List<Device>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener los dispositivos.");
             
                return new List<Device>();
            }
        }

        public async Task<Device?> GetByIdAsync(string id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"objects/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Dispositivo con ID {id} no encontrado o no accesible.");
                    return null; 
                }
                return await JsonUtils.DeserializeAsync<Device>(response.Content);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"Error al realizar la solicitud HTTP para obtener el dispositivo con ID: {id}");
                return null; 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener el dispositivo.");
                return null; 
            }
        }
    }
}
