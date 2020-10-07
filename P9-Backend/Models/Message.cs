using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P9_Backend.Models
{
    public class Message
    {
        public string sender { get; set; }
        public string message { get; set; }
        public string target { get; set; }

        public Message(string sender, string message, string target)
        {
            this.sender = sender;
            this.message = message;
            this.target = target;
        }

        public byte[] toBytes()
        {
            var obj = JsonConvert.SerializeObject(this);
            return Encoding.UTF8.GetBytes(obj + '\n');
        }
    }
}
