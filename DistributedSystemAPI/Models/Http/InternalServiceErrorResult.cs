using System.Net.Mime;

namespace DistributedSystemAPI.Models.Http;

internal record InternalServiceErrorResult : IResult
{
    private readonly string? _message;

    public InternalServiceErrorResult(string? message = null) => _message = message;

    public async Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.ContentType = MediaTypeNames.Application.Json;
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(_message ?? "Внутренняя ошибка сервера"));
    }
}
