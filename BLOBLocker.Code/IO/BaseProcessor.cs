using BLOBLocker.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.IO
{
    /// <summary>
    /// The base class of all data processors.
    /// A class deriving BaseProcessor must implement
    /// its own logic to process and restore the original
    /// data.
    /// </summary>
    public abstract class BaseProcessor : IDisposable
    {
        protected FileMode saveFileMode = FileMode.Create;
        public FileMode SaveFileMode
        {
            get
            {
                return saveFileMode;
            }
            set
            {
                saveFileMode = value;
            }
        }

        protected FileMode readFileMode = FileMode.Open;
        public FileMode ReadFileMode
        {
            get
            {
                return readFileMode;
            }
            set
            {
                readFileMode = value;
            }
        }

        ~BaseProcessor()
        {
            disposeImplementation(false);
            GC.SuppressFinalize(this);
        }

        public void Dispose()
        {
            disposeImplementation(true);
        }

        protected virtual void disposeImplementation(bool disposing)
        {

        }

        /// <summary>
        /// Processes the data using the overwritten
        /// implementation of the sub class.
        /// </summary>
        /// <param name="dataToProcess">The data process.</param>
        /// <returns>The processed data.</returns>
        public byte[] Process(byte[] dataToProcess)
        {
            if (dataToProcess == null)
                return null;
            if (dataToProcess.Length == 0)
                return dataToProcess;
            using (var originalStream = new MemoryStream(dataToProcess))
            {
                using (var processedStream = new MemoryStream())
                {
                    ProcessStream(originalStream, processedStream);
                    return processedStream.ToArray();
                }
            }
        }

        /// <summary>
        /// Restores the original data using the
        /// overwritten implementation of the sub class.
        /// </summary>
        /// <param name="processedData">The processed data to restore.</param>
        /// <returns>The original data.</returns>
        public byte[] GetOriginal(byte[] processedData)
        {
            if (processedData == null)
                return null;
            if (processedData.Length == 0)
                return processedData;
            using (var processedStream = new MemoryStream(processedData))
            {
                using (var originalStream = new MemoryStream())
                {
                    GetOriginalStream(processedStream, originalStream);
                    return originalStream.ToArray();
                }
            }
        }

        /// <summary>
        /// Processes the data using the overwritten
        /// implementation of the sub class.
        /// 
        /// Saves the output into a file.
        /// </summary>
        /// <param name="dataToProcess">The original data to process.</param>
        /// <param name="file">The output file.</param>
        /// <returns>If the method completed successfully.</returns>
        public bool ProcessAndSave(byte[] dataToProcess, FileInfo file)
        {
            if (dataToProcess == null)
                return false;
            if (dataToProcess.Length == 0)
                return false;
            using (var originalStream = new MemoryStream(dataToProcess))
            {
                using (var saveFileStream = new FileStream(file.FullName, saveFileMode, FileAccess.Write))
                {
                    ProcessStream(originalStream, saveFileStream);
                    return true;
                }
            }
        }

        /// <summary>
        /// Restores the original data using the
        /// overwritten implementation of the sub class.
        /// </summary>
        /// <param name="file">The file containing the processed data to restore.</param>
        /// <returns>The original data.</returns>
        public byte[] LoadAndGetOriginal(FileInfo file)
        {
            using (var readFileStream = new FileStream(file.FullName, readFileMode, FileAccess.Read))
            {
                using (var originalStream = new MemoryStream())
                {
                    GetOriginalStream(readFileStream, originalStream);
                    return originalStream.ToArray();
                }
            }
        }

        /// <summary>
        /// Processes the originalStream using the overwritten
        /// implementation of the sub class.
        /// </summary>
        /// <param name="originalStream">The original stream to process.</param>
        /// <param name="processedStream">The output.</param>
        protected abstract void ProcessStream(Stream originalStream, Stream processedStream);

        /// <summary>
        /// Restores the original data using the
        /// overwritten implementation of the sub class.
        /// </summary>
        /// <param name="processedStream">The processed stream.</param>
        /// <param name="originalStream">The stream to write the original data to.</param>
        protected abstract void GetOriginalStream(Stream processedStream, Stream originalStream);
    }
}
