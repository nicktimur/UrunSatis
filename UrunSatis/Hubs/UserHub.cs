using Microsoft.AspNetCore.SignalR;

public class UserHub : Hub
{
    public async Task NotifyUserAdded(string message)
    {
        await Clients.All.SendAsync("UserAdded", message);
    }
}
