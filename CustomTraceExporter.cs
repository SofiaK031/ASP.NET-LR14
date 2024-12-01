using OpenTelemetry;
using System.Diagnostics;
using System.Text;

namespace LR14
{
    public class CustomTraceExporter : BaseExporter<Activity>
    {
        private readonly HttpClient _httpClient;
        private readonly string _endpointUrl;

        // Конструктор експортера, який приймає URL бекенду для надсилання даних
        public CustomTraceExporter(string endpointUrl)
        {
            _httpClient = new HttpClient();
            _endpointUrl = endpointUrl;
        }

        // Реалізація методу Export для експорту даних
        public override ExportResult Export(in Batch<Activity> batch)
        {
            // Перетворення кожного Activity в JSON
            foreach (var activity in batch)
            {
                var jsonPayload = FormatActivityAsJson(activity);
                SendDataToBackendAsync(jsonPayload).Wait();
            }

            return ExportResult.Success;
        }

        // Форматування Activity у JSON
        private string FormatActivityAsJson(Activity activity)
        {
            var jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{");
            jsonBuilder.Append($"\"id\": \"{activity.Id}\", ");
            jsonBuilder.Append($"\"operationName\": \"{activity.DisplayName}\", ");
            jsonBuilder.Append($"\"startTime\": \"{activity.StartTimeUtc}\", ");
            jsonBuilder.Append($"\"duration\": \"{activity.Duration}\"");
            jsonBuilder.Append("}");

            return jsonBuilder.ToString();
        }

        // Форматування тегів у JSON
        private string FormatTagsAsJson(IEnumerable<KeyValuePair<string, object>> tags)
        {
            var tagJson = new StringBuilder();
            tagJson.Append("{");
            foreach (var tag in tags)
            {
                tagJson.Append($"\"{tag.Key}\": \"{tag.Value}\", ");
            }

            if (tagJson.Length > 1)
            {
                tagJson.Length--;
            }

            tagJson.Append("}");
            return tagJson.ToString();
        }

        // Надсилання даних через HTTP POST запит
        private async Task SendDataToBackendAsync(string jsonPayload)
        {
            try
            {
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(_endpointUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to send data: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}");
                }
                else
                {
                    Console.WriteLine("Data successfully sent!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending data: {ex.Message}");
            }
        }
    }
}