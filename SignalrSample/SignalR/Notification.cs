using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalrSample.SignalR
{
    [HubName("Notification")]
    public class Notification : Hub
    {
        public void SendServiceMessage(string level, string message)
        {
            Clients.All.ShowMessage(level, message);
        }
    }
}