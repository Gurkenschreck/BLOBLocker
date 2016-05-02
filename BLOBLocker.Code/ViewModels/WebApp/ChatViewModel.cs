using BLOBLocker.Code.Attributes;
using BLOBLocker.Entities.Models.Models.WebApp;
using BLOBLocker.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BLOBLocker.Code.ViewModels.WebApp
{
    public class ChatViewModel
    {
        [LocalizedDisplayName("Pool.PUID")]
        [Required]
        public string PUID { get; set; }
        [LocalizedDisplayName("Pool.Messages")]
        public ICollection<Message> Messages { get; set; }
        [LocalizedDisplayName("Chat.NewMessage")]
        public MessageViewModel NewMessage { get; set; }

        [Range(0, int.MaxValue, ErrorMessage="Invalid number of messages to show")]
        public int NextAmountShowLastMessageCount { get; set; }
        public PoolShare PoolShare { get; set; }
    }
}