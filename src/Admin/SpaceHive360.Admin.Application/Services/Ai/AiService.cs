using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SpaceHive360.Admin.Application.Services.Ai
{
    public class AiService : IAiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public AiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["AiService:BaseUrl"] ?? "http://localhost:8000";
        }

        public async Task<(string Sentiment, double Score)> AnalyzeSentimentAsync(string text)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(text)) return ("neutral", 0);

                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/analyze-sentiment", new { text });
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<SentimentResponse>();
                    return (result?.Sentiment ?? "neutral", result?.Confidence ?? 0);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AI Service Error (Sentiment): {ex.Message}");
            }
            return ("neutral", 0);
        }

        public async Task<string> SummarizeFeedbackAsync(List<string> reviews)
        {
            try
            {
                if (reviews == null || reviews.Count == 0) return "No feedback available.";

                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/summarize-feedback", new { reviews });
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<SummaryResponse>();
                    return result?.Summary ?? "No summary available.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AI Service Error (Summarization): {ex.Message}");
            }
            return "Unable to generate summary at this time.";
        }

        private class SentimentResponse
        {
            public string Sentiment { get; set; } = string.Empty;
            public double Confidence { get; set; }
        }

        private class SummaryResponse
        {
            public string Summary { get; set; } = string.Empty;
        }
    }
}
