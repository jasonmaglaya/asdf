using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Remy.Gambit.Api.Hubs
{
    [Authorize]
    public class EventHub : Hub
    {
        public Task JoinEvent(string group)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, group);
        }
    }
}
