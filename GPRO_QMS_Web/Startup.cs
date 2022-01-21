using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Services.Description;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(QMS_Website.Startup))]

namespace QMS_Website
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR(); 
             
        } 
    }
}
