using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EnglishCopilot.Services;
/// <summary>
/// openai 请求服务
/// </summary>
public class OpenAIClient
{
    private readonly HttpClient Client = new HttpClient();

    public OpenAIClient(string key)
    {
        Client.BaseAddress = new Uri("https://api.openai.com/");
        Client.DefaultRequestHeaders.Add("Accept", "application/json");
        Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + key);
    }

    /// <summary>
    /// 对话
    /// </summary>
    /// <returns></returns>
    public async Task<List<Choice>> ResponseChatAsync(string content)
    {
        var requestData = new GPTRequest
        {
            Messages = new List<Message> {
                new Message("You are a wise and rational polymath who enjoys chatting with other people, your name is freedom, and You are simulating a real human being having a conversation!") {
                    Role = "system"
                },
                new Message("content"),
            },
            N = 1,
            Max_tokens = 100,
            Temperature = 0.1
        };
        var response = await Client.PostAsJsonAsync("/v1/chat/completions", requestData);

        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<JsonElement>();
            // get [choices field] from data JsonElement
            var choices = data.GetProperty("choices").Deserialize<List<Choice>>();
            return choices;
        }
        return default;
    }

    public class Message
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = "user";
        [JsonPropertyName("content")]
        public string Content { get; set; } = default!;

        public Message(string content)
        {
            Content = content;
        }
    }

    public class GPTRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = "gpt-3.5-turbo";
        [JsonPropertyName("messages")]
        public List<Message> Messages { get; set; }
        [JsonPropertyName("n")]
        public int N { get; set; } = 1;
        [JsonPropertyName("max_tokens")]
        public int Max_tokens { get; set; } = 1500;
        [JsonPropertyName("temperature")]
        public double Temperature { get; set; } = 1;
    }

    public class Choice
    {
        [JsonPropertyName("message")]
        public Message Message { get; set; } = default!;
        [JsonPropertyName("finish_reason")]
        public string FinishReason { get; set; } = default!;
        [JsonPropertyName("index")]
        public int Index { get; set; } = default!;
    }

}
