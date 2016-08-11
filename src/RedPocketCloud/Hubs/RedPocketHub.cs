using Microsoft.AspNetCore.SignalR;

namespace RedPocketCloud.Hubs
{
    public class RedPocketHub : Hub
    {
        /// <summary>
        /// 加入SignalR频道以接收中奖信息推送
        /// </summary>
        /// <param name="name"></param>
        public void Join(string name)
        {
            Groups.Add(Context.ConnectionId, name);
        }
    }
}
