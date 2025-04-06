using System.Text.Json;

namespace caja18_prueba_tecnica.Utils
{
    public static class JsonUtils
    {
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public static async Task<T?> DeserializeAsync<T>(HttpContent content)
        {
            var json = await content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, _options);
        }
    }
}