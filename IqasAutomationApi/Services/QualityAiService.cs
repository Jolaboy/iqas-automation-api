using OpenAI.Chat;

namespace IqasAutomationApi.Services;

public class QualityAiService
{
    private readonly string _apiKey;

    public QualityAiService(IConfiguration configuration)
    {
        _apiKey = configuration["OpenAI:ApiKey"] ?? string.Empty;
    }

    public async Task<string> GenerateRootCauseSummaryAsync(string category, string rawDescription)
    {
        if (string.IsNullOrWhiteSpace(_apiKey) || _apiKey.StartsWith("YOUR_"))
            return "AI Service unconfigured: Raw defect description logged successfully.";

        var client = new ChatClient(model: "gpt-4o-mini", apiKey: _apiKey);

        string prompt = $@"
        You are an AI assistant for an operational quality management department. 
        Summarise the following process defect into a 2-sentence executive summary highlighting Root Cause and Remediation Steps.
        
        Category: {category}
        Description: {rawDescription}";

        ChatCompletion completion = await client.CompleteChatAsync(prompt);
        return completion.Content[0].Text;
    }
}