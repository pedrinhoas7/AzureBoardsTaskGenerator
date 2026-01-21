using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Json;


public class IAService : IIAService
{
    private readonly string _apiKey;
    private readonly string _model;

    public IAService(string apiKey, string model)
    {
        _apiKey = apiKey;
        _model = model;
    }

    public async Task<List<GeneratedTask>> GenerateTasksAsync(string cardTitle, string description)
    {
        var prompt = BuildPrompt(cardTitle, description);

        var aiResponse = await CallAIAsync(prompt);

        var rootObject = JObject.Parse(aiResponse); 

        var textJson = rootObject["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();

        if (string.IsNullOrWhiteSpace(textJson))
            throw new Exception("Campo 'text' vazio ou não encontrado.");

        var cleanJson = AiResponseCleaner.Clean(textJson); 

        var tasks = JsonConvert.DeserializeObject<List<GeneratedTask>>(cleanJson)
                    ?? throw new Exception("Falha ao converter resposta da IA");

        return tasks;
    }

    public static class AiResponseCleaner
    {
        public static string Clean(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                throw new Exception("Resposta da IA vazia");

            raw = raw.Trim();

            // remove ```json ou ```
            raw = raw.Replace("```json", "", StringComparison.OrdinalIgnoreCase)
                     .Replace("```", "")
                     .Trim();

            // tenta pegar só o array JSON
            var start = raw.IndexOf('[');
            var end = raw.LastIndexOf(']');

            if (start == -1 || end == -1 || end <= start)
                throw new Exception("JSON inválido retornado pela IA");

            return raw.Substring(start, end - start + 1);
        }
    }

    private async Task<string> CallAIAsync(string prompt)
    {
       var url =$"https://generativelanguage.googleapis.com/v1/models/{_model}:generateContent?key={_apiKey}";

        var body = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            }
        };

        var response = await new HttpClient().PostAsJsonAsync(url, body);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    private string BuildPrompt(string cardTitle, string description)
    {
        return $@"
Você é um assistente técnico de engenharia de software.

A partir do card abaixo, gere uma lista de no maximo tasks técnicas.
Cada task deve conter:
- title
- description

Formato de saída: JSON puro, sem markdown.

Card:
Título: {cardTitle}
Descrição:
{description}

Regras:
- Seja técnico e objetivo
- Divida em tasks pequenas
- Não invente contexto
";
    }
}
