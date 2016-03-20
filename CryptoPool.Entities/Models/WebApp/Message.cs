using CryptoPool.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPool.Entities.Models.Models.WebApp
{
    public class Message
    {
        public Message()
        {
            IsVisible = true;
            Sent = DateTime.Now;
        }
        [Key]
        public int ID { get; set; }
        public string Text { get; set; }
        public virtual Account Sender { get; set; }
        [ForeignKey("Pool")]
        public int PoolID { get; set; }
        public virtual Pool Pool { get; set; }
        public Nullable<DateTime> Sent { get; set; }
        public bool IsVisible { get; set; }
    }
}
