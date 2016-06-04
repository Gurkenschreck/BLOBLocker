using BLOBLocker.Code.Exception;
using BLOBLocker.Code.Membership;
using BLOBLocker.Code.ModelHelper;
using BLOBLocker.Code.Security.Cryptography;
using BLOBLocker.Code.Text;
using BLOBLocker.Code.Web;
using BLOBLocker.Entities.Models.WebApp;
using Cipha.Security.Cryptography;
using Cipha.Security.Cryptography.Asymmetric;
using Cipha.Security.Cryptography.Hash;
using Cipha.Security.Cryptography.Symmetric;
using Cipha.Security.IO;
using System;
using System.Collections.Generic;
using System.IO;
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

        public PoolShare PoolShare
        {
            get
            {
                return currentAccountPoolShare;
            }
        }

        public Pool Pool
        {
            get
            {
                return currentPool;
            }
        }

        public Account Account
        {
            get
            {
                return currentAccount;
            }
        }

        public bool CanAccessPool
        {
            get
            {
                if (currentPool != null)
                {
                    return currentAccount.PoolShares.Any(p => p.PoolID == currentPool.ID && p.IsActive);
                }
                else
                {
                    return false;
                }
            }
        }

        public bool IsUserInPool(string user)
        {
            return Pool.Participants.Any(p => p.SharedWith.Alias == user);
        }

        public int FreePoolSpace
        {
            get
            {
                int used = 0;
                foreach (var file in Pool.Files.Where(p => !p.IsDeleted))
                {
                    used += file.FileSize;
                }
                return TotalPoolSpace - used;
            }
        }

        public int TotalPoolSpace
        {
            get
            {
                int available = 0;
                foreach (var assignedMemory in Pool.AssignedMemory.Where(p => p.IsEnabled))
                {
                    available += assignedMemory.Space;
                }
                return available * 1024 * 1024;
            }
        }

        public PoolHandler(Account currentAccount, string poolUniqueIdentifier, out bool canAccessPool)
        {
            this.currentAccount = currentAccount;
            this.currentAccountPoolShare = currentAccount.PoolShares
                .FirstOrDefault(p => p.Pool.UniqueIdentifier == poolUniqueIdentifier && p.IsActive);
            if (currentAccountPoolShare != null)
            {
                this.currentPool = currentAccountPoolShare.Pool;
            }

            canAccessPool = CanAccessPool;
        }

        public PoolHandler(Account currentAccount, Pool currentPool)
        {
            this.currentAccount = currentAccount;
            this.currentPool = currentPool;
            this.currentAccountPoolShare = currentAccount.PoolShares.FirstOrDefault(p => p.PoolID == currentPool.ID && p.IsActive);
        }

        ~PoolHandler()
        {
            Dispose(false);
            GC.SuppressFinalize(this);
        }

        public void Initialize(byte[] privateRSAAccountKey)
        {
            if (!CanAccessPool)
                throw new UnauthorizedPoolAccessException();
            this.privateRSAAccountKey = privateRSAAccountKey.Clone() as byte[];
            initialized = true;
        }
        
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

        public bool ToggleFile(string storedFileName)
        {
            if (!CanAccessPool)
                throw new UnauthorizedPoolAccessException();

            var poolFile = Pool.Files.FirstOrDefault(p => p.StoredFileName == storedFileName);

            if (poolFile == null)
            {
                throw new PoolFileNotFoundException(string.Format("there is no file called {0}", storedFileName));
            }

            poolFile.IsVisible = !poolFile.IsVisible;

            return poolFile.IsVisible;
        }

        public bool DeleteFile(string storedFileName, string baseFilePath)
        {
            if (!CanAccessPool)
                throw new UnauthorizedPoolAccessException();

            var poolFile = Pool.Files.FirstOrDefault(p => p.StoredFileName == storedFileName);

            if (poolFile == null)
            {
                throw new PoolFileNotFoundException(string.Format("there is no file called {0}", storedFileName));
            }

            string localFilePath = string.Format("{0}/{1}/{2}.locked",
                    baseFilePath, currentPool.UniqueIdentifier, storedFileName);

            if (!File.Exists(localFilePath))
                throw new PoolFileNotFoundException(string.Format("{0} is not stored on the local drive", localFilePath));

            File.Delete(localFilePath);

            poolFile.IsVisible = !poolFile.IsVisible;
            poolFile.IsDeleted = true;

            return poolFile.IsVisible;
        }

        public Message SendMessage(string message)
        {
            if (!initialized)
                throw new InvalidOperationException("poolhandler must be initialized");

            Message msg = new Message();
            msg.Pool = currentPool;
            msg.Sender = currentAccount;
            using (var poolCipher = GetPoolCipher())
            {
                msg.Text = poolCipher.EncryptToString(message);

                using (var rsaCipher = new RSACipher<RSACryptoServiceProvider>(Encoding.UTF8.GetString(privateRSAAccountKey)))
                {
                    msg.TextSignature = rsaCipher.SignStringToString<SHA256Cng>(msg.Text);
                }
                currentPool.Messages.Add(msg);
            }
            return msg;
        }

        public StoredFile StoreFile(VirtualFile file, bool encryptContent = true)
        {
            if (!initialized)
                throw new InvalidOperationException("poolhandler must be initialized");

            if(file.Content.Length > FreePoolSpace)
                throw new NotEnoughPoolSpaceException(string.Format("pool {0} has {1} free space and cannot store {2}", Pool.ID, FreePoolSpace, file.Content.Length));

            StoredFile sf = new StoredFile();
            sf.Owner = currentAccount;
            sf.Pool = currentPool;
            currentPool.Files.Add(sf);

            byte[] content = file.Content;

            sf.IPv4Address = file.IPv4Address;
            sf.OriginalFileName = file.FileName;
            sf.FileSize = content.Length;
            sf.MimeType = file.MimeType;
            sf.FileExtension = file.FileExtension;
            sf.OriginalFileName = file.FileName;
            sf.Description = file.Description;
            sf.Encrypted = encryptContent;
            sf.IsVisible = file.IsVisible;

            using (var md5Hasher = new Hasher<MD5CryptoServiceProvider>())
            {
                sf.MD5Checksum = BitConverter.ToString(md5Hasher.Hash(content)).Replace("-", "");
            }

            using (var sha1Hasher = new Hasher<SHA1CryptoServiceProvider>())
            {
                sf.SHA1Checksum = BitConverter.ToString(sha1Hasher.Hash(content)).Replace("-", "");
            }

            // Create unique filename
            string directory = string.Format("{0}/{1}", file.FilePath, currentPool.UniqueIdentifier);
            // Create directory if pool dir doesn't exist
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            string pathToBe;
            do
            {
                sf.StoredFileName = Base32.ToBase32String(Utilities.GenerateBytes(9));
                pathToBe = string.Format("{0}/{1}.locked", directory, sf.StoredFileName);
            } while (File.Exists(pathToBe));


            // Encrypt the storedFile content and store it
            using (var byteStream = new MemoryStream(content))
            {
                using (var fileStream = new FileStream(pathToBe, FileMode.Create))
                {
                    if (encryptContent)
                    {
                        using (var poolCipher = GetPoolCipher())
                        {
                            using (var cipherStream = new CipherStream<AesManaged>(poolCipher))
                            {
                                cipherStream.EncryptStream(byteStream, fileStream);
                            }
                        }
                    }
                    else
                    {
                        byteStream.WriteTo(fileStream);
                    }
                }
            }

            return sf;
        }

        public ICollection<StoredFile> GetFiles()
        {
            StoredFile[] fileList;

            if (PoolRightHelper.HasRight(PoolShare, PoolRight.ManageFileStorage))
            {
                fileList = currentPool.Files.Where(p => !p.IsDeleted).ToArray();
            }
            else
            {
                fileList = currentPool.Files.Where(p => p.IsVisible && !p.IsDeleted).ToArray();
            }

            return fileList;
        }

        public VirtualFile GetFile(string storedFileName, string filePath, bool decryptContent)
        {
            return GetFile(storedFileName, filePath, decryptContent, null, -1);
        }

        public VirtualFile GetFile(string storedFileName, string filePath,
            bool decryptContent, string[] previewAble, int maxByteSizeForPreview)
        {
            if (!initialized)
                throw new InvalidOperationException("poolhandler must be initialized");

            var storedFile = currentPool.Files.FirstOrDefault(p => p.StoredFileName == storedFileName);
            if (storedFile != null)
            {
                VirtualFile vf = new VirtualFile();
                vf.FileName = storedFile.OriginalFileName;
                vf.FileExtension = storedFile.FileExtension;
                vf.IPv4Address = storedFile.IPv4Address;
                vf.MimeType = storedFile.MimeType;
                vf.Description = storedFile.Description;
                vf.MD5Checksum = storedFile.MD5Checksum;
                vf.SHA1Checksum = storedFile.SHA1Checksum;
                vf.Owner = storedFile.Owner.Alias;
                vf.UploadedOn = (DateTime)storedFile.UploadedOn;
                vf.FileSizeInByte = storedFile.FileSize;
                vf.IsVisible = storedFile.IsVisible;

                string localFilePath = string.Format("{0}/{1}/{2}.locked",
                    filePath, currentPool.UniqueIdentifier, storedFile.StoredFileName);

                if (!storedFile.IsDeleted && File.Exists(localFilePath))
                {
                    if ((storedFile.FileSize <= maxByteSizeForPreview && previewAble.Contains(storedFile.FileExtension))
                        || (maxByteSizeForPreview == -1 && previewAble == null))
                    {
                        using (var poolCipher = GetPoolCipher())
                        {
                            using (var fileStream = new FileStream(localFilePath, FileMode.Open))
                            {
                                using (var memoryStream = new MemoryStream())
                                {
                                    if (decryptContent)
                                    {
                                        using (var cipherStream = new CipherStream<AesManaged>(poolCipher))
                                        {
                                            cipherStream.DecryptStream(fileStream, memoryStream);
                                            vf.Content = memoryStream.ToArray();
                                        }
                                    }
                                    else
                                    {
                                        vf.Content = memoryStream.ToArray();
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    throw new FileNotFoundException(string.Format("{0} does not exist or db entry is invalid {1}", localFilePath, storedFile.IsDeleted));
                }

                return vf;
            }
            else
            {
                return null;
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
