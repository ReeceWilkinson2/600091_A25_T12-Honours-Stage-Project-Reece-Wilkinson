using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;

namespace Honours_Project_CompetencyandSkillTracking.Canvas
{
    public class CanvasService
    {
        private readonly HttpClient _http;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public CanvasService(HttpClient httpClient, IConfiguration config)
        {
            _http = httpClient;

            var baseUrl = config["Canvas:BaseUrl"]
                ?? throw new InvalidOperationException("Canvas BaseUrl missing");

            var token = config["Canvas:Token"]
                ?? throw new InvalidOperationException("Canvas Token missing");

            _http.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<CanvasProfile> GetMyProfileAsync()
        {
            return await GetAsync<CanvasProfile>("api/v1/users/self/profile");
        }

        public async Task<List<CanvasCourse>> GetMyCoursesAsync()
        {
            return await GetPagedAsync<CanvasCourse>(
                "api/v1/courses?enrollment_state=active&include[]=syllabus_body&include[]=term");
        }

        public async Task<List<CanvasAssignment>> GetCourseAssignmentsAsync(long courseId)
        {
            return await GetPagedAsync<CanvasAssignment>(
                $"api/v1/courses/{courseId}/assignments?include[]=description");
        }

        //public async Task<List<CanvasSubmission>> GetAssignmentSubmissionsAsync(long courseId, long assignmentId)
        //{
        //    return await GetPagedAsync<CanvasSubmission>(
        //        $"courses/{courseId}/assignments/{assignmentId}/submissions?student_ids=self");
        //}

        private async Task<T> GetAsync<T>(string endpoint)
        {
            var requestUri = new Uri(_http.BaseAddress!, endpoint);
            Console.WriteLine($">>> Canvas GET: {requestUri}");

            using var response = await _http.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<T>(stream, JsonOptions)
                   ?? throw new InvalidOperationException("Empty Canvas response");
        }

        private async Task<List<T>> GetPagedAsync<T>(string endpoint)
        {
            var requestUri = new Uri(_http.BaseAddress!, endpoint);
            Console.WriteLine($">>> Canvas GET: {requestUri}");

            var results = new List<T>();
            string? url = endpoint;

            while (!string.IsNullOrEmpty(url))
            {
                using var response = Uri.IsWellFormedUriString(url, UriKind.Absolute)
                    ? await _http.GetAsync(url)
                    : await _http.GetAsync(new Uri(_http.BaseAddress!, url));

                response.EnsureSuccessStatusCode();

                using var stream = await response.Content.ReadAsStreamAsync();
                var page = await JsonSerializer.DeserializeAsync<List<T>>(stream, JsonOptions);
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

