using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace DentalClinic.Hubs
{
    public class ChatHub : Hub
    {
        private static ConcurrentDictionary<string, string> UserConnections = new ConcurrentDictionary<string, string>();

        public override async Task OnConnectedAsync()
        {
            string userType = Context.GetHttpContext().Request.Query["userType"];
            if (!string.IsNullOrEmpty(userType))
            {
                UserConnections.TryAdd(Context.ConnectionId, userType);
                Console.WriteLine($"User connected: {Context.ConnectionId}, Type: {userType}");
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            UserConnections.TryRemove(Context.ConnectionId, out _);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessageToDoctor(string patientName, string message)
        {


            await Clients.All.SendAsync("ReceiveMessage", patientName, message);


        }

        public async Task SendMessageToPatient(string doctorName, string message, string patientConnectionId)
        {
            await Clients.All.SendAsync("ReceiveMessage", doctorName, message, patientConnectionId);
        }
    }
}
