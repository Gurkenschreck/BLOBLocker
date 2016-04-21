using BLOBLocker.Entities.Models.WebApp;
using Cipha.Security.Cryptography;
using Cipha.Security.Cryptography.Asymmetric;
using Cipha.Security.Cryptography.Symmetric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.Security.Cryptography
{
    public static class CryptoHelper
    {
        public static byte[] GetPoolKey(string curAccPrivateKeyString, PoolShare curAccPoolShare)
        {
            using (var rsaCipher = new RSACipher<RSACryptoServiceProvider>(curAccPrivateKeyString))
            {
                byte[] curPSKey = rsaCipher.Decrypt(curAccPoolShare.Config.Key);
                byte[] curPSIV = rsaCipher.Decrypt(curAccPoolShare.Config.IV);

                using (var curPSCipher = new SymmetricCipher<AesManaged>(curPSKey, curPSIV))
                {
                    byte[] curPSPriKey = curPSCipher.Decrypt(curAccPoolShare.Config.PrivateKey);
                    string encCurPSPriKey = Encoding.UTF8.GetString(curPSPriKey);

                    using (var poolKeyCipher = new RSACipher<RSACryptoServiceProvider>(encCurPSPriKey))
                    {
                        Utilities.SetArrayValuesZero(curPSKey);
                        Utilities.SetArrayValuesZero(curPSIV);
                        Utilities.SetArrayValuesZero(curPSPriKey);
                        byte[] poolKey = poolKeyCipher.Decrypt(curAccPoolShare.PoolKey);
                        return poolKey;
                    }
                }
            }
        }

        public static byte[] GetPoolKey(string curAccPrivateKeyString, PoolShare curAccPoolShare, out byte[] curAccPoolSharePriKey)
        {
            using (var rsaCipher = new RSACipher<RSACryptoServiceProvider>(curAccPrivateKeyString))
            {
                byte[] curPSKey = rsaCipher.Decrypt(curAccPoolShare.Config.Key);
                byte[] curPSIV = rsaCipher.Decrypt(curAccPoolShare.Config.IV);

                using (var curPSCipher = new SymmetricCipher<AesManaged>(curPSKey, curPSIV))
                {
                    curAccPoolSharePriKey = curPSCipher.Decrypt(curAccPoolShare.Config.PrivateKey);
                    string encCurPSPriKey = Encoding.UTF8.GetString(curAccPoolSharePriKey);

                    using (var poolKeyCipher = new RSACipher<RSACryptoServiceProvider>(encCurPSPriKey))
                    {
                        Utilities.SetArrayValuesZero(curPSKey);
                        Utilities.SetArrayValuesZero(curPSIV);
                        byte[] poolKey = poolKeyCipher.Decrypt(curAccPoolShare.PoolKey);
                        return poolKey;
                    }
                }
            }
        }
    }
}
