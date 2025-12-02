using DistributedSystemAPI.Abstractions;
using DistributedSystemAPI.Models;
using DistributedSystemAPI.Models.Cfg;
using DistributedSystemAPI.Models.Http;
using DistributedSystemAPI.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IPayloadManager, PayloadManager>();
builder.Services.AddSingleton<ISnapshotManager, SnapshotManager>();
builder.Services.Configure<FileManagmentOptions>(builder.Configuration.GetSection("FileManagmentOptions"));

var app = builder.Build();

app.MapGet("/", () => "Server started!");

app.MapPut("/replace", async (CancellationToken cancellationToken, ReplaceRequestModel request, [FromServices] DistributedSystemAPI.Abstractions.IPayloadManager fileManager) =>
{
    try
    {
        await fileManager.RewritePayloadAsync(request.Payload, cancellationToken);
        return Results.Ok();
    }
    catch (Exception ex)
    {
        app.Logger.LogError("Ошибка во время выполнения запроса. Подробно {err}", ex.Message);
        return new InternalServiceErrorResult("Ошибка во время выполнения запроса");
    }
});

app.MapGet("/get", async (CancellationToken cancellationToken, [FromServices] DistributedSystemAPI.Abstractions.IPayloadManager fileManager) => 
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