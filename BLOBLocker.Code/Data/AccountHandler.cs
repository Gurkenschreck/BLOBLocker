using BLOBLocker.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLOBLocker.Code.Data;
using Cipha.Security.Cryptography.Symmetric;
using System.Security.Cryptography;
using Cipha.Security.Cryptography;
using System.Web;

namespace BLOBLocker.Code.Data
{
    public class AccountHandler
    {
        Account currentAccount;
        public class AccountProperties
        {
            public ICollection<AccountRole> Roles { get; set; }
            public string Alias { get; set; }
            public string Password { get; set; }
        }

        public AccountHandler()
        {        }

        public AccountHandler(Account account)
        {
            currentAccount = account;
        }

        public Account SetupAccount(AccountProperties accProps,
            CryptoConfigHandler.CryptoConfigProperties cryptoProps,
            AccountAdditionHandler.AccountAdditionProperties additionProps,
            MemoryPoolHandler.MemoryPoolProperties memPoolProps)
        {
            Account newAcc = new Account();
            newAcc.Alias = accProps.Alias;
            foreach (var role in accProps.Roles)
            {
                newAcc.Roles.Add(new AccountRoleLink
                {
                    Account = newAcc,
                    Role = role
                });
            }

            AccountAdditionHandler accAddHandler = new AccountAdditionHandler();
            newAcc.Addition = accAddHandler.SetupNew(additionProps);

            MemoryPoolHandler memPoolHandler = new MemoryPoolHandler(newAcc);
            newAcc.MemoryPool = memPoolHandler.SetupNew(memPoolProps);

            byte[] salt;
            CryptoConfigHandler cryptoConfigHandler = new CryptoConfigHandler();
            newAcc.Config = cryptoConfigHandler.SetupCryptoConfig(accProps.Password, cryptoProps, out salt);
            newAcc.Salt = salt;

            return newAcc;
        }

        /// <summary>
        /// Logs an account in. Returns a number to identify probable problems.
        /// 
        /// 1: Login success
        /// 2: False password
        /// 3: Account disabled
        /// </summary>
        /// <param name="account">The account to log in.</param>
        /// <param name="password">The login password.</param>
        /// <returns>Returns status of outcome.</returns>
        public int Login(Account account, string password, out byte[] priRSAKey)
        {
            priRSAKey = null;
            if (account.IsEnabled)
            {
                using (var symC = new SymmetricCipher<AesManaged>(password, account.Salt, account.Config.IV, account.Config.KeySize,
                    account.Config.IterationCount))
                {
                    try
                    {
                        priRSAKey = symC.Decrypt(account.Config.PrivateKey);
                        account.Addition.LastLogin = DateTime.Now;
                    }
                    catch (CryptographicException)
                    {
                        account.Addition.LastFailedLogin = DateTime.Now;
                        BLOBLocker.Code.ModelHelper.NotificationHelper.SendNotification(account,
                                    HttpContext.GetGlobalResourceObject(null, "Notification.LoginFailed").ToString(),
                                    DateTime.Now);
                        return 2;
                    }
                }
            }
            else
            {
                return 3;
            }

            return 1;
        }

        public Contact AddContact(Account addAccount)
        {
            if (currentAccount.Addition.Contacts.All(p => p.AccountID != addAccount.ID))
            {
                Contact contact = new Contact();
                contact.AccountID = addAccount.ID;
                currentAccount.Addition.Contacts.Add(contact);

                BLOBLocker.Code.ModelHelper.NotificationHelper.SendNotification(currentAccount,
                    HttpContext.GetGlobalResourceObject(null, "Notification.AccountAddedToContacts").ToString(), addAccount.Alias);
                BLOBLocker.Code.ModelHelper.NotificationHelper.SendNotification(addAccount,
                    HttpContext.GetGlobalResourceObject(null, "Notification.YouWereAddedToContacts").ToString(), currentAccount.Alias);

                return contact;
            }
            else
            {
                
                return null;
            }
        }

        public ICollection<Contact> ShareContactsWith(Account shareWith)
        {
            if (currentAccount == null)
                throw new InvalidOperationException("currentAccount is not set");

            ICollection<Contact> corAccContacts = shareWith.Addition.Contacts;
            ICollection<Contact> added = new List<Contact>();
            foreach (Contact contact in currentAccount.Addition.Contacts)
            {
                if (!corAccContacts.Any(c => c.AccountID == contact.AccountID))
                {
                    Contact c = new Contact();
                    c.AccountID = contact.AccountID;
                    corAccContacts.Add(c);
                    added.Add(c);
                }
            }

            BLOBLocker.Code.ModelHelper.NotificationHelper.SendNotification(shareWith,
                HttpContext.GetGlobalResourceObject(null, "Notification.ContactsWereSharedWithYou").ToString(), currentAccount.Alias);
            BLOBLocker.Code.ModelHelper.NotificationHelper.SendNotification(currentAccount,
                HttpContext.GetGlobalResourceObject(null, "Notification.ShareContactsWith").ToString(), shareWith.Alias);

            return added;
        }

        public bool ChangePassword(string currentPassword, string newPassword, CryptoConfigHandler.CryptoConfigProperties properties)
        {
            if (currentAccount == null)
                throw new InvalidOperationException("currentaccount is not set");

            byte[] newSalt;
            CryptoConfigHandler cryptoConfigHandler = new CryptoConfigHandler();
            if(cryptoConfigHandler.ExchangeSymmetricKey(currentPassword, newPassword, currentAccount.Salt,
                currentAccount.Config, properties, out newSalt))
            {
                currentAccount.Salt = newSalt;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
