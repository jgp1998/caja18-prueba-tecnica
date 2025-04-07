using caja18_prueba_tecnica.Models;
using caja18_prueba_tecnica.Repositories.Interfaces;
using caja18_prueba_tecnica.Utils;
using Polly;
using Polly.Retry;
using System.Net.Sockets;
using System.Net;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace caja18_prueba_tecnica.Repositories
{
    public class DeviceRepository : IDeviceRepository
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<DeviceRepository> _logger;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
        private const int MaxRetries = 2;
        private const string BaseEndpoint = "objects";
        private static readonly HttpStatusCode[] RetryStatusCodes = new[]
        {
            HttpStatusCode.TooManyRequests,
            HttpStatusCode.MethodNotAllowed,
            HttpStatusCode.RequestTimeout,
            HttpStatusCode.GatewayTimeout,
            HttpStatusCode.ServiceUnavailable
        };

        public DeviceRepository(HttpClient httpClient, ILogger<DeviceRepository> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            _retryPolicy = Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .Or<SocketException>()
                .Or<IOException>()
                .OrResult(r => RetryStatusCodes.Contains(r.StatusCode))
                .WaitAndRetryAsync(
                    retryCount: MaxRetries,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (result, timeSpan, retryCount, context) =>
                    {
                        var errorMessage = result.Exception?.Message ?? result.Result?.StatusCode.ToString();
                        _logger.LogWarning(
                            "Reintentando operación. Intento {RetryCount}. Esperando {TimeSpan} segundos. Error: {Error}",
                            retryCount,
                            timeSpan.TotalSeconds,
                            errorMessage);
                    });
        }

        public async Task<IEnumerable<Device>> GetAllAsync()
        {
            try
            {
                var response = await _retryPolicy.ExecuteAsync(async () =>
                {
                    var result = await _httpClient.GetAsync(BaseEndpoint);
                    result.EnsureSuccessStatusCode();
                    return result;
                });

                var json = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("API Response: {Json}", json);

                var data = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(json);

                if (data == null)
                {
                    return Enumerable.Empty<Device>();
                }

                return data.Select(MapToDevice);
            }
            catch (Exception ex) when (ex is HttpRequestException or JsonException)
            {
                _logger.LogError(ex, "Error al obtener los dispositivos: {Message}", ex.Message);
                return Enumerable.Empty<Device>();
            }
        }

        public async Task<Device?> GetByIdAsync(string id)
        {
            try
            {
                var response = await _retryPolicy.ExecuteAsync(async () =>
                {
                    var result = await _httpClient.GetAsync($"{BaseEndpoint}/{id}");

                    if (result.StatusCode == HttpStatusCode.NotFound)
                    {
                        _logger.LogWarning("Dispositivo con ID {Id} no encontrado", id);
                        return null;
                    }

                    result.EnsureSuccessStatusCode();
                    return result;
                });

                if (response == null) return null;

                var json = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("API Response for ID {Id}: {Json}", id, json);

                var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

                if (data == null)
                {
                    return null;
                }

                return MapToDevice(data);
            }
            catch (Exception ex) when (ex is HttpRequestException or JsonException)
            {
                _logger.LogError(ex, "Error al obtener el dispositivo con ID {Id}: {Message}", id, ex.Message);
                return null;
            }
        }

        public Device MapToDevice(Dictionary<string, object> data)
        {
            _logger.LogInformation("Mapping data: {Data}", JsonSerializer.Serialize(data));

            var device = new Device();

            if (data != null)
            {
                foreach (var keyValuePair in data)
                {
                    string key = keyValuePair.Key.ToLower().Trim();
                    object value = keyValuePair.Value;

                    _logger.LogInformation("Processing top-level key: {Key}, value: {Value}", key, value);

                    if (key == "id")
                    {
                        device.Id = value?.ToString() ?? string.Empty;
                    }
                    else if (key == "name")
                    {
                        device.Name = value?.ToString() ?? string.Empty;
                    }
                    else if (key == "data" && value != null)
                    {
                        try
                        {
                            if (value is JsonElement jsonElement)
                            {
                                var dataObject = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonElement.ToString());
                                if (dataObject != null)
                                {
                                    foreach (var dataKeyValuePair in dataObject)
                                    {
                                        string dataKey = dataKeyValuePair.Key.ToLower().Trim();
                                        object dataValue = dataKeyValuePair.Value;
                                        string dataValueString = dataValue?.ToString() ?? string.Empty;

                                        _logger.LogInformation("Processing data key: {Key}, value: {Value}", dataKey, dataValueString);

                                        if (Regex.IsMatch(dataKey, @"capacity", RegexOptions.IgnoreCase))
                                        {
                                            device.Capacity = dataValueString;
                                        }
                                        else if (Regex.IsMatch(dataKey, @"price", RegexOptions.IgnoreCase))
                                        {
                                            device.Price = dataValueString;
                                        }
                                        else if (Regex.IsMatch(dataKey, @"color", RegexOptions.IgnoreCase))
                                        {
                                            device.Color = dataValueString;
                                        }
                                        else if (Regex.IsMatch(dataKey, @"year", RegexOptions.IgnoreCase))
                                        {
                                            device.Year = dataValueString;
                                        }
                                        else if (Regex.IsMatch(dataKey, @"cpu\s*model", RegexOptions.IgnoreCase))
                                        {
                                            device.CPUModel = dataValueString;
                                        }
                                        else if (Regex.IsMatch(dataKey, @"hard\s*disk\s*size", RegexOptions.IgnoreCase))
                                        {
                                            device.HardDiskSize = dataValueString;
                                        }
                                        else if (Regex.IsMatch(dataKey, @"screen\s*size", RegexOptions.IgnoreCase))
                                        {
                                            device.ScreenSize = dataValueString;
                                        }
                                        else if (Regex.IsMatch(dataKey, @"generation", RegexOptions.IgnoreCase))
                                        {
                                            device.Generation = dataValueString;
                                        }
                                        else if (Regex.IsMatch(dataKey, @"strap\s*colour", RegexOptions.IgnoreCase))
                                        {
                                            device.StrapColour = dataValueString;
                                        }
                                        else if (Regex.IsMatch(dataKey, @"case\s*size", RegexOptions.IgnoreCase))
                                        {
                                            device.CaseSize = dataValueString;
                                        }
                                        else if (Regex.IsMatch(dataKey, @"description", RegexOptions.IgnoreCase))
                                        {
                                            device.Description = dataValueString;
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error al procesar el objeto data: {Message}", ex.Message);
                        }
                    }
                }
            }

            _logger.LogInformation("Mapped device: {Device}", JsonSerializer.Serialize(device));
            return device;
        }
    }
}
