using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BLOBLocker.Code.IO;
using System.Security.Cryptography;
using Cipha.Security.Cryptography.Symmetric;

namespace BLOBLocker.Code.Tests.IO
{
    [TestClass]
    public class CompressionAndEncryptionProcessorTests
    {
        static readonly byte[] DATA_TO_PROCESS = {
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
        public void Process_ProcessAndGetOriginal_Pass()
        {
            SymmetricCipher<AesManaged> symCipher = new SymmetricCipher<AesManaged>();
            var caep = new CompressionAndEncryptionProcessor(symCipher);
            byte[] processed; 
            byte[] original;
 
            processed = caep.Process(DATA_TO_PROCESS);
            original = caep.GetOriginal(processed);

            // create compress and encrypt processor -> look at output | compare
            CollectionAssert.AreEqual(DATA_TO_PROCESS, original);
        }
    }
}
