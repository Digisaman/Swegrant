using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swegrant.Server.Hubs
{
    public class ChatHub : Hub
    {
        public async Task AddToGroup(string groupName, string user)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("Entered", user);
        }

        public async Task RemoveFromGroup(string groupName, string user)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("Left", user);
        }

        public async Task SendMessageGroup(string groupName, string user, string message)
        {

            await Clients.Group(groupName).SendAsync(Swegrant.Shared.Models.ChatSettings.RecieveCommand, user, message);

            //await Clients.Group(groupName).SendAsync(Swegrant.Shared.Models.ChatSettings.RecieveCommand, user, "Recievesd");


        }
    }
}
