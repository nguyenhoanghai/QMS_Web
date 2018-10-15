using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace QMS_Website.Hubs
{
    public class ChatHub : Hub
    {
        public void Send(string name, string sms)
        {
            Clients.All.sendNewDataToPage(name, sms);
        }

        public void SendDateTime( )
        {
            Clients.All.sendDateTimeToPage(DateTime.Now.ToString("dd/MM/yyyy|HH : mm"));
        }
    }
}