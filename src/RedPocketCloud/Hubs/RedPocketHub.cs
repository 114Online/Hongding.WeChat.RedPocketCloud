using Microsoft.AspNetCore.SignalR;

namespace RedPocketCloud.Hubs
{
    public class RedPocketHub : Hub
    {
        public void Join(string name)
        {
            Groups.Add(Context.ConnectionId, name);
        }
    }
}
