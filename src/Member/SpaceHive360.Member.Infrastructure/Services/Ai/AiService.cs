using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SpaceHive360.Member.Application.Services.Ai;

namespace SpaceHive360.Member.Infrastructure.Services.Ai
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

        public async Task<(string Sentiment, float Score)> AnalyzeSentimentAsync(string text)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/analyze-sentiment", new { text });
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<SentimentResponse>();
                    return (result?.Label ?? "neutral", result?.Score ?? 0.5f);
                }
            }
            catch (Exception ex)
            {
                // Fallback to basic logic if AI service is down
                Console.WriteLine($"AI Service Error: {ex.Message}");
            }

            return ("neutral", 0.5f);
        }

        private class SentimentResponse
        {
            public string Label { get; set; }
            public float Score { get; set; }
        }
    }
}
