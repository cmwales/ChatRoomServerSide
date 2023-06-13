using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SocialMix.Models.Models;

namespace SocialMix.Api
{
    public class ChatHub : Hub
    {

        public async Task<string> Negotiate()
        {
            var httpContext = Context.GetHttpContext();
            var token = httpContext.Request.Query["token"];

            // Perform any necessary token validation or authentication checks here

            return await Task.FromResult(token).ConfigureAwait(false);
        }


        public async Task BroadcastMessage(ChatMessage message)
        {
            // Handle the received message here
            // You can perform any necessary logic, such as saving the message to a database, etc.

            // Broadcast the message to all connected clients
            await Clients.All.SendAsync("ReceiveMessage", message).ConfigureAwait(false);
        }
    }
}


