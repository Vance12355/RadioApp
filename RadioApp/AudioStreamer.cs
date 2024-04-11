using Microsoft.AspNetCore.SignalR;
using RadioApp.Hubs;

internal class AudioStreamer<IHostedService>
{
    //private readonly IHubContext<RadioHub> _hubContext;

    //public AudioStreamer(IHubContext<RadioHub> hubContext)
    //{
    //    _hubContext = hubContext;
    //}

    //public Task StartAsync(CancellationToken cancellationToken)
    //{
    //    Console.WriteLine($"ппппппппппппппппппппп");
    //    _ = StreamAudioToClients(cancellationToken);
    //    return Task.CompletedTask;
    //}

    //private async Task StreamAudioToClients(CancellationToken cancellationToken)
    //{
    //    var filePath = "wwwroot/audio/music.mp3";
    //    Console.WriteLine($"фвыфывфывфыввфыв");
    //    while (!cancellationToken.IsCancellationRequested)
    //    {
    //        var musicBytes = await File.ReadAllBytesAsync(filePath, cancellationToken);
    //        Console.WriteLine($"Read {musicBytes.Length} bytes from audio file.");
    //        await _hubContext.Clients.Group("MusicRoom").SendAsync("ReceiveAudio", musicBytes, cancellationToken);
    //        await Task.Delay(TimeSpan.FromMinutes(5)); // Допустим, музыка длится 5 минут
    //    }
    //}

    //public Task StopAsync(CancellationToken cancellationToken)
    //{
    //    return Task.CompletedTask;
    //}
    private readonly IHubContext<RadioHub> _hubContext;

    public AudioStreamer(IHubContext<RadioHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _ = Task.Run(() => StreamAudioToClients(cancellationToken));
        return Task.CompletedTask;
    }

    private async Task StreamAudioToClients(CancellationToken cancellationToken)
    {
        var filePath = "wwwroot/audio/music.mp3";
        using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await _hubContext.Clients.Group("MusicRoom").SendAsync("ReceiveAudioStream", fileStream.Name, fileStream);
                await Task.Delay(TimeSpan.FromMinutes(5)); // Do you want the music to play for exactly 5 minutes?
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}