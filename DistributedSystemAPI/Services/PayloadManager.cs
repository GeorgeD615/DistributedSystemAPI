using DistributedSystemAPI.Abstractions;
using DistributedSystemAPI.Models;
using DistributedSystemAPI.Models.Cfg;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
namespace DistributedSystemAPI.Services;

internal class PayloadManager : IPayloadManager
{
    private readonly ISnapshotManager _snapshotManager;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<PayloadManager> _logger;

    private readonly string _directoryPath;
    private readonly string _payloadFilePath;
    private readonly int _snapshotTimeInterval;

    private readonly bool _isMaster;
    private readonly List<string> _followers;

    private readonly ConcurrentDictionary<string, int> _clockTable = [];

    private DateTime _lastSnapshotTime;

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
        WriteIndented = true
    };

    public PayloadManager(IOptions<FileManagmentOptions> fileManagmentOptions, IOptions<NodeOptions> nodeOptions, ISnapshotManager snapshotManager, IHttpClientFactory httpClientFactory, ILogger<PayloadManager> logger)
    {
        _directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileManagmentOptions.Value.Directory);
        _payloadFilePath = Path.Combine(_directoryPath, fileManagmentOptions.Value.PayloadFileName);
        _snapshotTimeInterval = fileManagmentOptions.Value.SnapshotTimeInterval;

        _isMaster = nodeOptions.Value.IsMaster;
        _followers = nodeOptions.Value.Followers ?? [];

        _snapshotManager = snapshotManager;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task CreateFileAsync(CancellationToken cancellationToken)
    {
        if (!Directory.Exists(_directoryPath))
            Directory.CreateDirectory(_directoryPath);

        if (!File.Exists(_payloadFilePath))
            using (StreamWriter sw = new(_payloadFilePath))
            {
                await sw.WriteAsync("Initial text.");
            }
    }

    public async Task<string> ReadPayloadAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_payloadFilePath))
            await CreateFileAsync(cancellationToken);

        using StreamReader sr = new(_payloadFilePath);
        return await sr.ReadToEndAsync(cancellationToken);
    }

    public async Task RewritePayloadAsync(ReplaceRequestModel model, CancellationToken cancellationToken)
    {
        var maxClockValue = _clockTable.Select(el => el.Value).DefaultIfEmpty(0).Max();

        if (maxClockValue >= model.ID)
        {
            _logger.LogWarning("Запрос не выполнен. Максимальное логическое время в системе {maxClockValue}.", maxClockValue);
            return;
        }

        if (!File.Exists(_payloadFilePath))
            await CreateFileAsync(cancellationToken);


        using StreamWriter payloadWriter = new(_payloadFilePath);
        await payloadWriter.WriteAsync(new StringBuilder(model.Payload), cancellationToken);
        _clockTable[model.Source] = model.ID;

        if(DateTime.Now - _lastSnapshotTime >= TimeSpan.FromMinutes(_snapshotTimeInterval))
        {
            await _snapshotManager.TakeSnapshotAsync(model.Payload, cancellationToken);
            _lastSnapshotTime = DateTime.Now;
        }

        if (!_isMaster)
            return;

        await SendPayloadToFollowers(model, cancellationToken);
    }

    public async Task SendPayloadToFollowers(ReplaceRequestModel model, CancellationToken cancellationToken)
    {
        foreach (var f in _followers)
        {
            try
            {
                var content = JsonContent.Create(model);
                using var response = await _httpClientFactory.CreateClient().PutAsync($"{f}/replace", content, cancellationToken);

                if (response.IsSuccessStatusCode)
                    _logger.LogInformation("Сообщение успешно доставлено по адресу: {f}.", f);
                else
                    _logger.LogWarning("Сообщение не доставлено по адресу: {f}. Код ошибки: {er}.", f, response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка при отправке подписчикам. Подробно: {ex}.", ex.Message);
            }
        }
    }

    public string ClockTableJson => JsonSerializer.Serialize(_clockTable, _jsonSerializerOptions);
}
