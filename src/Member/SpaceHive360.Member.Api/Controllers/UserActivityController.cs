using Microsoft.AspNetCore.Mvc;
using SpaceHive360.Member.Application.Models;
using SpaceHive360.Member.Application.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace SpaceHive360.Member.Api.Controllers
{
    [ApiController]
    [Route("api/user-activity")]
    public class UserActivityController : ControllerBase
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public UserActivityController(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        [HttpPost]
        public async Task<IActionResult> LogActivity([FromBody] UserActivityRequest request)
        {
            // Non-blocking fire-and-forget logging using a new scope
            try 
            {
                _ = Task.Run(async () => {
                    using var scope = _scopeFactory.CreateScope();
                    var service = scope.ServiceProvider.GetRequiredService<IUserActivityService>();
                    try {
                        await service.LogActivityAsync(request);
                    } catch (Exception ex) {
                        Console.WriteLine($"[Backend Tracker] Error logging activity: {ex.Message}");
                    }
                });
            } catch (Exception ex) {
                Console.WriteLine($"[Backend Tracker] Failed to start background task: {ex.Message}");
            }
            
            return Ok(new { success = true });
        }

        [HttpPost("batch")]
        public async Task<IActionResult> LogActivitiesBatch([FromBody] List<UserActivityRequest> requests)
        {
            // Non-blocking fire-and-forget logging using a new scope
            try 
            {
                _ = Task.Run(async () => {
                    using var scope = _scopeFactory.CreateScope();
                    var service = scope.ServiceProvider.GetRequiredService<IUserActivityService>();
                    try {
                        await service.LogActivitiesBatchAsync(requests);
                    } catch (Exception ex) {
                        Console.WriteLine($"[Backend Tracker] Error logging batch: {ex.Message}");
                    }
                });
            } catch (Exception ex) {
                Console.WriteLine($"[Backend Tracker] Failed to start background batch task: {ex.Message}");
            }

            return Ok(new { success = true });
        }
    }
}
