using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace spreadbase.Models
{
    public class CryptoConfig
    {
        [Key]
        public int ID { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public string PublicKeySignature { get; set; }
        public byte[] IV { get; set; }
    }
}