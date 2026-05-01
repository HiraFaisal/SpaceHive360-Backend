using System;
using System.Collections.Generic;

namespace SpaceHive360.Member.Application.Models
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; }
        public List<string>? Errors { get; set; }
        public int StatusCode { get; set; }

        public static ApiResponse SuccessResponse(object? data, string message = "Success")
        {
            return new ApiResponse
            {
                Success = true,
                Message = message,
                Data = data,
                StatusCode = 200
            };
        }

        public static ApiResponse ErrorResponse(string message, int statusCode = 500, List<string>? errors = null)
        {
            return new ApiResponse
            {
                Success = false,
                Message = message,
                Errors = errors,
                StatusCode = statusCode
            };
        }
    }
}
