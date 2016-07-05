using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Cryptography;
using Cipha.Security.Cryptography.Symmetric;
using BLOBLocker.Code.IO;

namespace BLOBLocker.Code.Tests.IO
{
    [TestClass]
    public class EncryptionProcessorTests
    {
        static readonly SymmetricCipher<AesManaged> symCipher = new SymmetricCipher<AesManaged>();

        static readonly byte[] DATA_TO_ENCRYPT = {
                                        0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,
                                        19,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,
                                        9,9,9,9,14,14,14,14,14,14,14,14,14,14,14,14,
                                        14,14,14,14,14,14,14,14,14,14,14,14,200,200,
                                        200,200,200,200,200,200,200,200,200,200,166,
                                        166,166,166,166,166,166,166,166,166,166,166,
                                        142,142,142,142,142,142,142,142,142,142,142,
                                        42,42,42,42,42,42,42,42,42,42,42,42,42,42,42,
                                        87,87,87,87,87,87,87,87,87,87,87,87,87,87,87,
                                        4,2,3,5,7,3,43,127,32,192,232,255,172,90,32
                                    };

        [TestMethod]
        public void Process_EncryptData_Pass()
        {
            EncryptionProcessor ep = new EncryptionProcessor(symCipher);

            byte[] encrypted = ep.Process(DATA_TO_ENCRYPT);

            CollectionAssert.AreNotEqual(DATA_TO_ENCRYPT, encrypted);
        }

        [TestMethod]
        public void GetOriginal_DecryptData_Pass()
        {
            EncryptionProcessor ep = new EncryptionProcessor(symCipher);

            byte[] encrypted = ep.Process(DATA_TO_ENCRYPT);
            byte[] decrypted = ep.GetOriginal(encrypted);

            CollectionAssert.AreEqual(DATA_TO_ENCRYPT, decrypted);
        }
    }
}
