using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpaceHive360.Member.Application.Services.Ai
{
    public interface IAiService
    {
        Task<(string Sentiment, float Score)> AnalyzeSentimentAsync(string text);
    }
}
