using DistributedSystemAPI.Abstractions;
using DistributedSystemAPI.Models;
using DistributedSystemAPI.Models.Cfg;
using DistributedSystemAPI.Models.Http;
using DistributedSystemAPI.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IPayloadManager, PayloadManager>();
builder.Services.AddSingleton<ISnapshotManager, SnapshotManager>();

builder.Services.AddHttpClient();

builder.Logging.AddConsole();

builder.Services.Configure<FileManagmentOptions>(builder.Configuration.GetSection("FileManagmentOptions"));
builder.Services.Configure<NodeOptions>(builder.Configuration.GetSection("NodeOptions"));

var app = builder.Build();

app.MapGet("/", () => "Server started!");

app.MapPut("/replace", async (CancellationToken cancellationToken, ReplaceRequestModel request, [FromServices] IPayloadManager fileManager) =>
{
    try
    {
        await fileManager.RewritePayloadAsync(request, cancellationToken);
        return Results.Ok();
    }
    catch (Exception ex)
    {
        app.Logger.LogError("Ошибка во время выполнения запроса. Подробно {err}", ex.Message);
        return new InternalServiceErrorResult("Ошибка во время выполнения запроса");
    }
});

app.MapGet("/get", async (CancellationToken cancellationToken, [FromServices] IPayloadManager fileManager) => 
{
    try
    {
        return Results.Ok(await fileManager.ReadPayloadAsync(cancellationToken));
    }
    catch (Exception ex)
    {
        app.Logger.LogError("Ошибка во время выполнения запроса. Подробно {err}", ex.Message);
        return new InternalServiceErrorResult("Ошибка во время выполнения запроса");
    }
});

app.Run();