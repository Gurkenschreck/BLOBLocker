using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BLOBLocker.Code.IO;
using System.Text;

namespace BLOBLocker.Code.Tests.IO
{
    [TestClass]
    public class CompressionProcessorTests
    {
        static readonly byte[] DATA_TO_COMPRESS = {
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
        public void ProcessData_Compress_Pass()
        {
            CompressionProcessor cp = new CompressionProcessor();

            byte[] compressedData = cp.Process(DATA_TO_COMPRESS);

            Assert.IsTrue(DATA_TO_COMPRESS.Length > compressedData.Length);
        }

        [TestMethod]
        public void GetOriginal_Decompress_Pass()
        {
            CompressionProcessor cp = new CompressionProcessor();
            byte[] compressedData;
            byte[] uncompressedData;

            compressedData = cp.Process(DATA_TO_COMPRESS);
            uncompressedData = cp.GetOriginal(compressedData);

            CollectionAssert.AreEqual(DATA_TO_COMPRESS, uncompressedData);
        }
    }
}
