using System.Net.Http.Json;
using System.Text.Json;

Console.WriteLine("WRITE AN USER NAME");

var username = Console.ReadLine();

HttpClient client = new();
client.DefaultRequestHeaders.Add("User-Agent", "C# App");
var response = await client.GetAsync($"https://api.github.com/users/{username}/events");
response.EnsureSuccessStatusCode();
string data = await response.Content.ReadAsStringAsync();
var events = JsonDocument.Parse(data).RootElement;

foreach (var item in events.EnumerateArray())
{
    string action = string.Empty;
    string type = item.GetProperty("type").GetString();
    string repoName = item.GetProperty("repo").GetProperty("name").GetString();
    switch (type)
    {
        case "Pushitem":
            int commitCount = item.GetProperty("payload").GetProperty("commits").GetArrayLength();
            action = $"Pushed {commitCount} commit(s) to {repoName}";
            break;
        case "Issuesitem":
            action = $"{char.ToUpper(item.GetProperty("payload").GetProperty("action").ToString()[0])}{item.GetProperty("payload").GetProperty("action").GetString().Substring(1)} an issue in {repoName}";
            break;
        case "Watchitem":
            action = $"Starred {repoName}";
            break;
        case "Forkitem":
            action = $"Forked {repoName}";
            break;
        case "Createitem":
            action = $"Created {item.GetProperty("payload").GetProperty("ref_type").GetString()} in {repoName}";
            break;
        default:
            action = $"{type.Replace("item", "")} in {repoName}";
            break;
    }

    Console.WriteLine(action);
}

Console.ReadLine();