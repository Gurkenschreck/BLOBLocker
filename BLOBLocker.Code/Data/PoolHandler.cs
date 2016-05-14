using BLOBLocker.Code.Membership;
using BLOBLocker.Code.ModelHelper;
using BLOBLocker.Code.Security.Cryptography;
using BLOBLocker.Code.Text;
using BLOBLocker.Code.Web;
using BLOBLocker.Entities.Models.Models.WebApp;
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

namespace BLOBLocker.Code.Data
{
    public class PoolHandler : IDisposable
    {
        Account currentAccount;
        Pool currentPool;
        PoolShare currentAccountPoolShare;
        bool initialized = false;

        byte[] privateRSAAccountKey;

        public PoolShare CorrespondingPoolShare
        {
            get
            {
                return currentAccountPoolShare;
            }
        }

        public bool CanAccessPool
        {
            get
            {
                return currentAccount.PoolShares.Any(p => p.PoolID == currentPool.ID && p.IsActive);
            }
        }

        public PoolHandler(Account currentAccount, Pool currentPool)
        {
            this.currentAccount = currentAccount;
            this.currentPool = currentPool;
            currentAccountPoolShare = currentAccount.PoolShares.FirstOrDefault(p => p.PoolID == currentPool.ID);
        }

        ~PoolHandler()
        {
            Dispose(false);
            GC.SuppressFinalize(this);
        }

        public void Initialize(byte[] privateRSAAccountKey)
        {
            this.privateRSAAccountKey = privateRSAAccountKey.Clone() as byte[];
            initialized = true;
        }

        /*public void Initialize(CryptoKeyInformation cryptoKeyInformation)
        {
            this.storedKeyPart = cryptoKeyInformation.CryptoCookiePart.Clone() as byte[];
            this.sessionCookieKey = cryptoKeyInformation.SessionKey.Clone() as byte[];
            this.sessionCookieIV = cryptoKeyInformation.SessionIV.Clone() as byte[];
            this.sessionStoredKeyPart = cryptoKeyInformation.SessionStoredKeyPart.Clone() as byte[];
            initialized = true;
        }*/

        public SymmetricCipher<AesManaged> GetPoolCipher()
        {
            if (!initialized)
                throw new InvalidOperationException("poolhandler must be initialized");

            byte[] del;
            var cipher = GetPoolCipher(out del);
            Utilities.SetArrayValuesZero(del);
            return cipher;
        }

        public SymmetricCipher<AesManaged> GetPoolCipher(out byte[] poolSharePrivateRSAKey)
        {
            if (!initialized)
                throw new InvalidOperationException("poolhandler must be initialized");

            poolSharePrivateRSAKey = null;

            byte[] poolKey = CryptoHelper.GetPoolKey(Encoding.UTF8.GetString(privateRSAAccountKey),
                currentAccountPoolShare, out poolSharePrivateRSAKey);
            var cipher = new SymmetricCipher<AesManaged>(poolKey, currentPool.Config.IV);
            Utilities.SetArrayValuesZero(poolKey);
            return cipher;
        }

        public PoolShare AddToPool(Account addAccount, int poolShareSymmetricKeySize)
        {
            if (!initialized)
                throw new InvalidOperationException("poolhandler must be initialized");

            PoolShare ps = new PoolShare();
            ps.Config = new CryptoConfiguration();

            ps.Pool = currentPool;
            ps.SharedWith = addAccount;
            ps.Rights = currentPool.DefaultRights;

            string curAccPrivateKeyString = Encoding.UTF8.GetString(privateRSAAccountKey);

            using (var rsaCipher = new RSACipher<RSACryptoServiceProvider>(curAccPrivateKeyString))
            {
                byte[] curPSKey = rsaCipher.Decrypt(currentAccountPoolShare.Config.Key);
                byte[] curPSIV = rsaCipher.Decrypt(currentAccountPoolShare.Config.IV);

                using (var curPSCipher = new SymmetricCipher<AesManaged>(curPSKey, curPSIV))
                {
                    byte[] curPSPriKey = curPSCipher.Decrypt(currentAccountPoolShare.Config.PrivateKey);

                    ps.PoolKey = currentAccountPoolShare.PoolKey;

                    using (var corAccRSACipher = new RSACipher<RSACryptoServiceProvider>(addAccount.Config.PublicKey))
                    {
                        using (var corPSCipher = new SymmetricCipher<AesManaged>(poolShareSymmetricKeySize))
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
            addAccount.PoolShares.Add(ps);
            currentPool.Participants.Add(ps);
            return ps;
        }

        public PoolShare SetupNew(int puidByteLength,
            int defaultRights, int poolSaltByteLength,
            int poolShareKeySize, int poolShareRSAKeySize,
            int poolRSAKeySize, int poolSymKeySize)
        {
            currentPool.Owner = currentAccount;
            currentPool.UniqueIdentifier = Base32.ToBase32String(Utilities.GenerateBytes(puidByteLength));
            currentPool.Salt = Utilities.GenerateBytes(poolSaltByteLength);

            CryptoConfiguration poolConfig = new CryptoConfiguration();
            currentAccountPoolShare = new PoolShare();
            currentAccountPoolShare.Pool = currentPool;
            currentAccountPoolShare.SharedWith = currentAccount;

            CryptoConfiguration poolShareConfig = new CryptoConfiguration();
            poolShareConfig.RSAKeySize = poolShareRSAKeySize;
            poolShareConfig.KeySize = poolShareKeySize;

            using (var accRSACipher = new RSACipher<RSACryptoServiceProvider>(currentAccount.Config.PublicKey))
            {
                using (var poolShareCipher = new SymmetricCipher<AesManaged>(poolShareKeySize))
                {
                    // 1. PoolShare Key|IV generation
                    poolShareConfig.Key = accRSACipher.Encrypt(poolShareCipher.Key);
                    poolShareConfig.IV = accRSACipher.Encrypt(poolShareCipher.IV);
                    using (var poolRSACipher = new RSACipher<RSACryptoServiceProvider>(poolRSAKeySize))
                    {
                        using (var poolSymCipher = new SymmetricCipher<AesManaged>(poolSymKeySize))
                        {
                            poolConfig.PublicKey = poolRSACipher.ToXmlString(false);
                            poolConfig.IV = poolSymCipher.IV;

                            poolShareConfig.PrivateKey = poolShareCipher.Encrypt(poolRSACipher.ToXmlString(true));
                            currentAccountPoolShare.PoolKey = poolRSACipher.Encrypt(poolSymCipher.Key);

                            poolConfig.PublicKeySignature = poolRSACipher.SignStringToString<SHA256Cng>(poolConfig.PublicKey);
                        }
                    }
                }
            }

            currentPool.Config = poolConfig;
            currentPool.DefaultRights = defaultRights;
            currentAccountPoolShare.Config = poolShareConfig;
            currentAccountPoolShare.Rights = int.MaxValue;
            currentAccount.PoolShares.Add(currentAccountPoolShare);
            return currentAccountPoolShare;
        }

        public void GetChat(int amountOfMessagesToShow,
            out ICollection<Message> chatMessages)
        {
            if (!initialized)
                throw new InvalidOperationException("poolhandler must be initialized");
            ICollection<Message> encryptedMessageList;
            ICollection<Message> decryptedMessageList;

            if (currentAccountPoolShare.ShowSince != null)
            {
                DateTime showSince = (DateTime)currentAccountPoolShare.ShowSince;
                encryptedMessageList = currentPool.Messages
                                              .Skip(currentPool.Messages.Count - amountOfMessagesToShow)
                                              .Where(p => DateTime.Compare(showSince, (DateTime)p.Sent) < 0)
                                              .OrderByDescending(p => p.Sent)
                                              .ToList();

            }
            else
            {
                encryptedMessageList = currentPool.Messages
                                              .Skip(currentPool.Messages.Count - amountOfMessagesToShow)
                                              .OrderByDescending(p => p.Sent)
                                              .ToList();
            }

            if (encryptedMessageList.Count > 0)
            {
                decryptedMessageList = new List<Message>();
                using (var poolCipher = GetPoolCipher())
                {
                    foreach (var encMsg in encryptedMessageList)
                    {
                        Message curMsg = encMsg;
                        curMsg.Text = poolCipher.DecryptToString(curMsg.Text);
                        decryptedMessageList.Add(curMsg);
                    }
                }
                chatMessages = decryptedMessageList;
            }
            else
            {
                chatMessages = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if(privateRSAAccountKey != null)
                    Utilities.SetArrayValuesZero(privateRSAAccountKey);
            }
        }
    }
}
