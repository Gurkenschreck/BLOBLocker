using Cipha.Security.Cryptography;
using Cipha.Security.Cryptography.Asymmetric;
using Cipha.Security.Cryptography.Symmetric;
using BLOBLocker.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BLOBLocker.Code.ModelHelper
{
    public class PoolShareHandler
    {
        HttpSessionStateBase Session;
        HttpContextBase HttpContext;
        int poolShareKeySize;

        public PoolShareHandler(HttpSessionStateBase Session, HttpContextBase HttpContext)
        {
            this.Session = Session;
            this.HttpContext = HttpContext;
            //poolShareKeySize = Convert.ToInt32(HttpContext.Application["security.PoolShareKeySize"]);
        }
        public PoolShare Connect(PoolShare curAccPoolShare, Account corAcc, Pool pool, byte[] accPrivateKey, int keySize)
        {
            PoolShare ps = new PoolShare();
            ps.Config = new CryptoConfiguration();

            ps.Pool = pool;
            ps.SharedWith = corAcc;
            ps.Rights = pool.DefaultRights;

            string curAccPrivateKeyString = Encoding.UTF8.GetString(accPrivateKey);

            using (var rsaCipher = new RSACipher<RSACryptoServiceProvider>(curAccPrivateKeyString))
            {
                byte[] curPSKey = rsaCipher.Decrypt(curAccPoolShare.Config.Key);
                byte[] curPSIV = rsaCipher.Decrypt(curAccPoolShare.Config.IV);

                using (var curPSCipher = new SymmetricCipher<AesManaged>(curPSKey, curPSIV))
                {
                    byte[] curPSPriKey = curPSCipher.Decrypt(curAccPoolShare.Config.PrivateKey);

                    ps.PoolKey = curAccPoolShare.PoolKey;

                    using (var corAccRSACipher = new RSACipher<RSACryptoServiceProvider>(corAcc.Config.PublicKey))
                    {
                        using (var corPSCipher = new SymmetricCipher<AesManaged>(keySize))
                        {
                            ps.Config.PrivateKey = corPSCipher.Encrypt(curPSPriKey);
                            ps.Config.Key = corAccRSACipher.Encrypt(corPSCipher.Key);
                            ps.Config.IV = corAccRSACipher.Encrypt(corPSCipher.IV);
                        }
                    }
                    Utilities.SetArrayValuesZero(curPSPriKey);
                }
                Utilities.SetArrayValuesZero(curPSKey);
                Utilities.SetArrayValuesZero(curPSIV);

            }
            corAcc.PoolShares.Add(ps);
            pool.Participants.Add(ps);
            return ps;
        }
        
    }
}
