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

app.MapPut("/replace", async (CancellationToken cancellationToken, ReplaceRequestModel request, [FromServices] IPayloadManager payloadManager) =>
{
    try
    {
        await payloadManager.RewritePayloadAsync(request, cancellationToken);
        return Results.Ok();
    }
    catch (Exception ex)
    {
        app.Logger.LogError("Ошибка во время выполнения запроса. Подробно {err}", ex.Message);
        return new InternalServiceErrorResult("Ошибка во время выполнения запроса");
    }
});

app.MapGet("/get", async (CancellationToken cancellationToken, [FromServices] IPayloadManager payloadManager) => 
{
    try
    {
        return Results.Content(await payloadManager.ReadPayloadAsync(cancellationToken), "text/plain", System.Text.Encoding.UTF8);
    }
    catch (Exception ex)
    {
        app.Logger.LogError("Ошибка во время выполнения запроса. Подробно {err}", ex.Message);
        return new InternalServiceErrorResult("Ошибка во время выполнения запроса");
    }
});

app.MapGet("/test", (CancellationToken cancellationToken) =>
{
    var htmlContent = @"
    <!DOCTYPE html>
    <html lang=""eu"">
    <head>
        <title>HW</title>
        <meta charset=""UTF-8"">
        <meta name=""viewport"" content=""width=device-width,initial-scale=1"">
        <meta http-equiv=""x-ua-compatible"" content=""crhome=1"">
        <link rel=""preconnect"" href=""https://fonts.googleapis.com"">
        <link rel=""preconnect"" href=""https://fonts.gstatic.com"" crossorigin>
        <link href=""https://fonts.googleapis.com/css2?family=Madimi+One&display=swap"" rel=""stylesheet"">
        <script type=""text/javascript"">

            async function refreshData() {
                const getRes = await fetch(""/get"");
                document.getElementById(""get"").textContent = await getRes.text();

                const vcRes = await fetch(""/vclock"");
                document.getElementById(""vclock"").textContent = await vcRes.text();
            }

            window.addEventListener(""load"", async function() {

                await refreshData();

                const form = document.forms[""replace""];
                form.addEventListener(""submit"", async function (e) {
                    e.preventDefault();

                    const raw = form[""text""].value;

                    let json;
                    try {
                        json = JSON.parse(raw);
                    } catch (e) {
                        alert(""Invalid JSON in textarea!"");
                        return;
                    }

                    const res = await fetch(""/replace"", {
                        method: ""PUT"",
                        headers: {
                            ""Content-Type"": ""application/json""
                        },
                        body: JSON.stringify(json)
                    });

                    if (!res.ok) {
                        alert(""Fetch error: "" + res.status);
                        return;
                    }

                    await refreshData();
                });
            });

        </script>
    </head>
    <body>

    <h2>/replace</h2>
    <form name=""replace"">
        <textarea name=""text"" style=""width:400px;height:150px;""></textarea>
        <br>
        <input type=""submit"" value=""Submit"">
    </form>

    <h2>/get</h2>
    <div id=""get""></div>

    <h2>/vclock</h2>
    <div id=""vclock""></div>

    </body>
    </html>";
    return Results.Content(htmlContent, "text/html", System.Text.Encoding.UTF8);
});

app.MapGet("/vclock", (CancellationToken cancellationToken, [FromServices] IPayloadManager payloadManager) =>
{
    return Results.Content(payloadManager.ClockTableJson, "text/plain", System.Text.Encoding.UTF8);
});

app.Run();