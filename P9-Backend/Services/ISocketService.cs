using P9_Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P9_Backend.Services
{
    public interface ISocketService
    {
        public abstract void Start();
        public abstract void Stop();
        public abstract void SendOne(string uuid, Message msg);
        public abstract void SendAll(Message msg);
    }
}
