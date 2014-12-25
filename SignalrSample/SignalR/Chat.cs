using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalrSample.SignalR
{
    [HubName("chat")]
    public class Chat : Hub
    {
        public void Send(string clientName, string message)
        {
            //var toSelfinfo = "You had sent message " + message;
            //Caller.addSomeMessage(clientName, toSelfinfo);
            var a = 0;
            // Call the addMessage method on all clients
            Clients.All.addSomeMessage(clientName, message);
            //Clients[Context.ConnectionId].addSomeMessage(clientName, data);        
        }
    }
}