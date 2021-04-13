using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace P9_Backend.HubConfig
{
    public class DemoHub : Hub
    {
        private bool paused = true;

        public async Task PauseDemo()
        {
            paused = true;
            await Clients.All.SendAsync("DemoStatus", paused);
        }

        public async Task StartDemo()
        {
            paused = false;
            await Clients.All.SendAsync("DemoStatus", paused);
        }

        public async Task GetStatus()
        {
            await Clients.Caller.SendAsync("DemoStatus", paused);
        }

        public async Task RestartDemo()
        {
            await Clients.All.SendAsync("Restart");
        }

        public async Task ChangePredictive()
        {
            await Clients.All.SendAsync("PredictiveChange");
        }
    }
}
