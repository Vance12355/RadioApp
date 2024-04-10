using Microsoft.AspNetCore.SignalR;
using System;
using System.IO;
using System.Threading.Tasks;

namespace RadioApp.Hubs
{
    public class RadioHub:Hub
    {

        private readonly string _audioFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot/audio/music.mp3");

        public async Task JoinRoom(string roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            Console.WriteLine(roomId);
            // Отправка аудиофайла клиенту при подключении к комнате
            var audioFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot/audio/music.mp3");
            if (File.Exists(audioFilePath))
            {
                var audioData = File.ReadAllBytes(audioFilePath);
                await Clients.Caller.SendAsync("PlayAudio", Convert.ToBase64String(audioData));
            }
        }

        public async Task LeaveRoom(string roomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
        }
    }
}
