using System.Linq;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Honours_Project_CompetencyandSkillTracking.Canvas
{
    public class CanvasService
    {
        private readonly HttpClient _http;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            ReadCommentHandling = JsonCommentHandling.Skip
        };

        public CanvasService(HttpClient httpClient, IConfiguration config)
        {
            _http = httpClient;

            var baseUrl = config["Canvas:BaseUrl"]
                ?? throw new InvalidOperationException("Canvas BaseUrl missing");

            var token = config["Canvas:Token"]
                ?? throw new InvalidOperationException("Canvas Token missing");

            _http.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<CanvasProfile> GetMyProfileAsync()
            => await GetAsync<CanvasProfile>("api/v1/users/self/profile");

        public async Task<List<CanvasCourse>> GetMyCoursesAsync()
            => await GetPagedAsync<CanvasCourse>("api/v1/courses?enrollment_state=active&include[]=syllabus_body&include[]=term");

        public async Task<List<CanvasAssignment>> GetCourseAssignmentsAsync(long courseId)
            => await GetPagedAsync<CanvasAssignment>($"api/v1/courses/{courseId}/assignments?include[]=description");

        public async Task<List<CanvasSubmission>> GetAssignmentSubmissionsAsync(long courseId, long assignmentId)
        {
            string endpoint = $"courses/{courseId}/assignments/{assignmentId}/submissions?student_ids=self";
            Console.WriteLine($">>> Canvas GET: {endpoint}");

            var json = await _http.GetStringAsync(endpoint);
            Console.WriteLine(json);  // Log the raw response for debugging

            return JsonSerializer.Deserialize<List<CanvasSubmission>>(json, JsonOptions)
                ?? throw new InvalidOperationException("Empty Canvas response for submissions");
        }


        private async Task<T> GetAsync<T>(string endpoint)
        {
            Console.WriteLine($">>> Canvas GET: {endpoint}");
            var json = await _http.GetStringAsync(endpoint);
            Console.WriteLine(json);
            return JsonSerializer.Deserialize<T>(json, JsonOptions)
                   ?? throw new InvalidOperationException("Empty Canvas response");
        }

        private async Task<List<T>> GetPagedAsync<T>(string endpoint)
        {
            var results = new List<T>();
            string? url = endpoint;

            while (!string.IsNullOrEmpty(url))
            {
                using var response = Uri.IsWellFormedUriString(url, UriKind.Absolute)
                    ? await _http.GetAsync(url)
                    : await _http.GetAsync(new Uri(_http.BaseAddress!, url));

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"PAGED ENDPOINT: {url}");
                Console.WriteLine(json);

                var page = JsonSerializer.Deserialize<List<T>>(json, JsonOptions);
                if (page != null)
                    results.AddRange(page);

                url = GetNextPageUrl(response);
            }

            return results;
        }

        private static string? GetNextPageUrl(HttpResponseMessage response)
        {
            if (!response.Headers.TryGetValues("Link", out var links))
                return null;

            foreach (var link in links.SelectMany(l => l.Split(',')))
            {
                if (link.Contains("rel=\"next\""))
                {
                    var start = link.IndexOf('<') + 1;
                    var end = link.IndexOf('>');
                    return link[start..end];
                }
            }

            return null;
        }
    }
}