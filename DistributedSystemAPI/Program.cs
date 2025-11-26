using DistributedSystemAPI.Abstractions;
using DistributedSystemAPI.Models;
using DistributedSystemAPI.Models.Cfg;
using DistributedSystemAPI.Models.Http;
using DistributedSystemAPI.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IFileManager, FileManager>();
builder.Services.Configure<FileLocation>(builder.Configuration.GetSection("FileLocation"));

var app = builder.Build();

app.MapGet("/", () => "Server started!");

app.MapPut("/replace", async (CancellationToken cancellationToken, ReplaceRequestModel request, [FromServices] IFileManager fileManager) =>
{
    try
    {
        await fileManager.WriteIntoFile(request.Payload, cancellationToken);
        return Results.Ok();
    }
    catch (Exception ex)
    {
        app.Logger.LogError("Ошибка во время выполнения запроса. Подробно {err}", ex.Message);
        return new InternalServiceErrorResult("Ошибка во время выполнения запроса");
    }
});

app.MapGet("/get", async (CancellationToken cancellationToken, [FromServices] IFileManager fileManager) => 
{
    try
    {
        return Results.Ok(await fileManager.ReadFromFile(cancellationToken));
    }
    catch (Exception ex)
    {
        app.Logger.LogError("Ошибка во время выполнения запроса. Подробно {err}", ex.Message);
        return new InternalServiceErrorResult("Ошибка во время выполнения запроса");
    }
});

app.Run();