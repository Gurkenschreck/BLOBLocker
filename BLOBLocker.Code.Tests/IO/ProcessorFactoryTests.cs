using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BLOBLocker.Code.IO;
using BLOBLocker.Entities.Models.WebApp;
using System.Security.Cryptography;
using Cipha.Security.Cryptography.Symmetric;

namespace BLOBLocker.Code.Tests.IO
{
    [TestClass]
    public class ProcessorFactoryTests
    {
        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod]
        public void CreateProcessor_InstantiateEncryptionProcessor_Fail()
        {
            ProcessorFactory pf = new ProcessorFactory();

            var processor = pf.CreateProcessor(StoringMode.Encrypted);
        }

        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod]
        public void CreateProcessor_InstantiateCompressAndEncryptionProcessor_Fail()
        {
            ProcessorFactory pf = new ProcessorFactory();

            var processor = pf.CreateProcessor(StoringMode.CompressedAndEncrypted);
        }

        [TestMethod]
        public void CreateProcessor_InstantiateCompressionProcessor_Pass()
        {
            ProcessorFactory pf = new ProcessorFactory();

            var processor = pf.CreateProcessor(StoringMode.Compressed);

            Assert.IsInstanceOfType(processor, typeof(CompressionProcessor));
        }

        [TestMethod]
        public void CreateProcessor_InstantiatePlainProcessor_Pass()
        {
            ProcessorFactory pf = new ProcessorFactory();

            var processor = pf.CreateProcessor(StoringMode.Unprocessed);

            Assert.IsInstanceOfType(processor, typeof(PlainProcessor));
        }

        [TestMethod]
        public void CreateProcessor_InstantiateEncryptionProcessor_Pass()
        {
            SymmetricCipher<AesManaged> symCipher = new SymmetricCipher<AesManaged>();
            ProcessorFactory pf = new ProcessorFactory();

            var processor = pf.CreateProcessor(StoringMode.Encrypted, symCipher);

            Assert.IsInstanceOfType(processor, typeof(EncryptionProcessor));
        }

        [TestMethod]
        public void CreateProcessor_InstantiateCompressionAndEncryptionProcessor_Pass()
        {
            SymmetricCipher<AesManaged> symCipher = new SymmetricCipher<AesManaged>();
            ProcessorFactory pf = new ProcessorFactory();

            var processor = pf.CreateProcessor(StoringMode.CompressedAndEncrypted, symCipher);

            Assert.IsInstanceOfType(processor, typeof(CompressionAndEncryptionProcessor));
        }
    }
}
