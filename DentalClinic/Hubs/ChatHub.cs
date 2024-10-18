using Microsoft.AspNetCore.SignalR;


namespace DentalClinic.Hubs
{
    public class ChatHub : Hub
    {
        private static Dictionary<string, string> UserConnections = new Dictionary<string, string>();

        public override async Task OnConnectedAsync()
        {
            string userType = Context.GetHttpContext().Request.Query["userType"];
            UserConnections[Context.ConnectionId] = userType;
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            UserConnections.Remove(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessageToDoctor(string patientName, string message)
        {
            foreach (var connection in UserConnections)
            {
                if (connection.Value == "Doctor")
                {
                    await Clients.Client(connection.Key).SendAsync("ReceiveMessage", patientName, message);
                }
            }
        }

        public async Task SendMessageToPatient(string doctorName, string message, string patientConnectionId)
        {
            await Clients.Client(patientConnectionId).SendAsync("ReceiveMessage", doctorName, message);
        }
    }
}
