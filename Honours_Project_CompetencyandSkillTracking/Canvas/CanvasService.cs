using Honours_Project_CompetencyandSkillTracking.Canvas.Classes;
using Honours_Project_CompetencyandSkillTracking.Canvas.DTOs;
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

        public async Task<CanvasProfile> GetMyProfileAsync() => await GetAsync<CanvasProfile>("api/v1/users/self/profile");

        public async Task<List<CanvasCourse>> GetMyCoursesAsync() => await GetPagedAsync<CanvasCourse>("api/v1/courses?enrollment_state=active&include[]=syllabus_body&include[]=term");

        public async Task<List<CanvasAssignment>> GetCourseAssignmentsAsync(long courseId) => await GetPagedAsync<CanvasAssignment>($"api/v1/courses/{courseId}/assignments?include[]=description&include[]=submission_types");

        public async Task<List<CanvasSubmission>> GetAssignmentSubmissionsAsync(long courseId, long assignmentId)
        {
            string endpoint = $"api/v1/courses/{courseId}/assignments/{assignmentId}/submissions/self";
            Console.WriteLine($">>> Canvas GET: {endpoint}");

            try
            {
                var response = await _http.GetAsync(endpoint);

                Console.WriteLine($"Status Code: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($">>> Submission fetch failed: {response.StatusCode}");
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Response content: {responseContent}");
                    return new List<CanvasSubmission>();
                }

                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine(json);

                var submission = JsonSerializer.Deserialize<CanvasSubmission>(json, JsonOptions);

                return submission != null
                    ? new List<CanvasSubmission> { submission }
                    : new List<CanvasSubmission>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($">>> Exception fetching submissions for {assignmentId}: {ex.Message}");
                return new List<CanvasSubmission>();
            }
        }

        public async Task<List<CanvasOutcome>> GetCourseOutcomesAsync(long courseId)=> await GetPagedAsync<CanvasOutcome>($"api/v1/courses/{courseId}/outcomes");

        public async Task<List<CanvasOutcomeResult>> GetOutcomeResultsAsync(long courseId)
        {
            var results = new List<CanvasOutcomeResult>();
            string endpoint =
                $"api/v1/courses/{courseId}/outcome_results?include[]=outcomes";

            string? url = endpoint;

            while (!string.IsNullOrEmpty(url))
            {
                using var response = Uri.IsWellFormedUriString(url, UriKind.Absolute)
                    ? await _http.GetAsync(url)
                    : await _http.GetAsync(new Uri(_http.BaseAddress!, url));

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"OUTCOME RESULTS JSON: {json}");

                var wrapper = JsonSerializer.Deserialize<CanvasOutcomeResultsResponse>(json, JsonOptions);

                if (wrapper?.OutcomeResults != null)
                    results.AddRange(wrapper.OutcomeResults);

                url = GetNextPageUrl(response);
            }

            return results;
        }

        private async Task<T> GetAsync<T>(string endpoint)
        {
            Console.WriteLine($">>> Canvas GET: {endpoint}");

            var response = await _http.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
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

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($">>> Paged fetch failed: {response.StatusCode}");
                    break;
                }

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