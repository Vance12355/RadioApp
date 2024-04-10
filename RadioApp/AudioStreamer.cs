using Microsoft.AspNetCore.SignalR;
using RadioApp.Hubs;

internal class AudioStreamer<IHostedService>
{
    private readonly IHubContext<RadioHub> _hubContext;

    public AudioStreamer(IHubContext<RadioHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _ = StreamAudioToClients(cancellationToken);
        return Task.CompletedTask;
    }

    private async Task StreamAudioToClients(CancellationToken cancellationToken)
    {
        var filePath = "wwwroot/audio/music.mp3";
        while (!cancellationToken.IsCancellationRequested)
        {
            var musicBytes = await File.ReadAllBytesAsync(filePath, cancellationToken);
            await _hubContext.Clients.Group("MusicRoom").SendAsync("ReceiveAudio", musicBytes, cancellationToken);
            await Task.Delay(TimeSpan.FromMinutes(5)); // Допустим, музыка длится 5 минут
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}