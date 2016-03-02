using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SpreadBase.Models
{
    public class CryptoConfig
    {
        [Key]
        public int ID { get; set; }
        
        public string PublicKey { get; set; }
        public byte[] PrivateKey { get; set; }
        public string PublicKeySignature { get; set; }
        public byte[] IV { get; set; }

        // Optional
        public int RSAKeySize { get; set; }
        public byte[] Key { get; set; }
        public int KeySize { get; set; }
        public int IterationCount { get; set; }
    }
}