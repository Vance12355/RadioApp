using Microsoft.AspNetCore.SignalR;
using System;
using System.IO;
using System.Threading.Tasks;

namespace RadioApp.Hubs
{
    public class ChatHub : Hub
    {
        private static readonly Dictionary<string, string> ConnectedUsers = new Dictionary<string, string>();
        private static readonly string[] Moderators = { "moderator1", "moderator2" };

        static string GenerateRandomID()
        {
            string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            Random random = new Random();
            char[] idArray = new char[16];

            for (int i = 0; i < idArray.Length; i++)
            {
                idArray[i] = characters[random.Next(characters.Length)];
            }

            return new string(idArray);
        }

        public async Task SendMessage(string user, string message)
        {
            // Передаем сообщение всем подключенным клиентам
            if (!string.IsNullOrWhiteSpace(message)) // Проверяем, что сообщение не пустое
            {
                string messageID = GenerateRandomID();
                //await Clients.All.SendAsync("ReceiveMessage", user, message);
                await Clients.All.SendAsync("NewMessage", messageID, message, user);
            }
        }

        public async Task DeleteMessage(string messageId)
        {
            await Clients.All.SendAsync("MessageDeleted", messageId);
        }

        public async Task KickUser(string username)
        {
            string connectionId = ConnectedUsers.FirstOrDefault(x => x.Value == username).Key;

            await Clients.Client(connectionId).SendAsync("YouHaveBeenKicked");
            await Clients.All.SendAsync("UserKicked", username);
        }

        public async Task StartChat(string username)
        {
            ConnectedUsers[Context.ConnectionId] = username;
            //await Clients.All.SendAsync("StartChat", username);
            //await Clients.All.SendAsync("Join", username);
            await Clients.All.SendAsync("UpdateUserList", ConnectedUsers.Values);
            bool isModerator = Moderators.Contains(username);
            await Clients.Caller.SendAsync("ModeratorStatus", isModerator); // Отправляем статус модератора обратно клиенту

        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            ConnectedUsers.Remove(Context.ConnectionId);
            await Clients.All.SendAsync("UpdateUserList", ConnectedUsers.Values);
            await base.OnDisconnectedAsync(exception);
        }

    }
}
