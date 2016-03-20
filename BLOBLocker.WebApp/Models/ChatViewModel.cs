using BLOBLocker.Entities.Models.Models.WebApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BLOBLocker.WebApp.Models
{
    public class ChatViewModel
    {
        public string PUID { get; set; }
        public ICollection<Message> Messages { get; set; }
        public MessageViewModel NewMessage { get; set; }
        public int NextAmountShowLastMessageCount { get; set; }
    }
}