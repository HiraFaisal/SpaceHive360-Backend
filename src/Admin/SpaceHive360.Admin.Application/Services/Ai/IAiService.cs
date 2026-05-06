using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpaceHive360.Admin.Application.Services.Ai
{
    public interface IAiService
    {
        Task<(string Sentiment, double Score)> AnalyzeSentimentAsync(string text);
        Task<string> SummarizeFeedbackAsync(List<string> reviews);
    }
}
