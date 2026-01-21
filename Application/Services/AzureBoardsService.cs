using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

public class AzureBoardsService : IAzureBoardsService
{
    private readonly HttpClient _http;
    private readonly string _org;
    private readonly string _project;

    public AzureBoardsService(string org, string project, string pat)
    {
        _org = org;
        _project = project;

        _http = new HttpClient();
        var auth = Convert.ToBase64String(Encoding.ASCII.GetBytes($":{pat}"));
        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Basic", auth);
    }

    public async Task<CardInfo> GetCardInfoAsync(int cardId)
    {
        var url = $"https://dev.azure.com/{_org}/{_project}/_apis/wit/workitems/{cardId}?api-version=7.0";
        var response = await _http.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = JObject.Parse(await response.Content.ReadAsStringAsync());
        var title = json["fields"]?["System.Title"]?.ToString();
        var description = json["fields"]?["System.Description"]?.ToString();

        return new CardInfo(cardId, title, description);
    }

    public async Task<int> CreateTaskAsync(int parentId, string title, string description)
    {
        var url = $"https://dev.azure.com/{_org}/{_project}/_apis/wit/workitems/$Task?api-version=7.0";

        var patch = new object[]
        {
            new { op = "add", path = "/fields/System.Title", value = title },
            new { op = "add", path = "/fields/System.Description", value = description },
            new { op = "add", path = "/fields/Microsoft.VSTS.Common.Activity", value = "Development" },
            new {
                op = "add",
                path = "/relations/-",
                value = new {
                    rel = "System.LinkTypes.Hierarchy-Reverse",
                    url = $"https://dev.azure.com/{_org}/{_project}/_apis/wit/workItems/{parentId}"
                }
            }
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(patch),
            Encoding.UTF8,
            "application/json-patch+json"
        );

        var response = await _http.PatchAsync(url, content);


        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("=== Azure DevOps ERROR ===");
            Console.WriteLine($"Status: {(int)response.StatusCode} - {response.StatusCode}");
            Console.WriteLine(responseBody);
            Console.WriteLine("=========================");
            Console.ResetColor();

            // opcional: relança com contexto útil
            throw new Exception(
                $"Azure DevOps error {(int)response.StatusCode}: {responseBody}"
            );
        }

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        dynamic workItem = JsonConvert.DeserializeObject(json);
        return (int)workItem.id;
    }
}
