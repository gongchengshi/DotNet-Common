using System;
using System.IO.MemoryMappedFiles;

namespace Gongchengshi
{
    /// <summary>
    /// Transfers data (a structure of type T) atomically through a MemoryMappedFile.  This uses a Software Transactional Memory
    /// trick to function without the need for locking.
    /// </summary>
    public abstract class MemoryMappedIPC<T> : IDisposable
    {
        protected MemoryMappedFile _memoryMappedFile;
        protected MemoryMappedViewAccessor _viewAccessor;
        public void Dispose()
        {
            if (_viewAccessor != null)
                _viewAccessor.Dispose();
            if (_memoryMappedFile != null)
                _memoryMappedFile.Dispose();
        }
    }

    /// <summary>
    /// The server is allowed to write data to the shared memory.
    //// Note:  Only one MemoryMappedIPCServer should write to a given map.
    /// </summary>
    public class MemoryMappedIPCServer<T> : MemoryMappedIPC<T>
        where T : struct
    {
        public MemoryMappedIPCServer()
            : this(typeof(T).Name)
        {
        }
        public MemoryMappedIPCServer(string mapName)
        {
            try
            {
                _memoryMappedFile = MemoryMappedFile.CreateOrOpen(mapName,
                    sizeof(Int32) + 2 * System.Runtime.InteropServices.Marshal.SizeOf(new T()),
                    MemoryMappedFileAccess.ReadWrite);
                _viewAccessor = _memoryMappedFile.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Write);
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        public void Write(ref T value)
        {
            _transactionNumber++;
            if (_transactionNumber == Int32.MaxValue)
                _transactionNumber = 1;
            _viewAccessor.Write(0, -1);
            _viewAccessor.Write(sizeof(Int32), ref value);
            _viewAccessor.Write(0, _transactionNumber);
        }
        Int32 _transactionNumber = 1;
    }


    /// <summary>
    /// The client reads data from the shared memory.  The read is guaranteed to have integrity.
    /// Note:  There can be many readers for a given map.
    /// </summary>
    public class MemoryMappedIPCClient<T> : MemoryMappedIPC<T>
       where T : struct
    {
        public MemoryMappedIPCClient()
            : this(typeof(T).Name)
        {
        }
        public MemoryMappedIPCClient(string mapName)
        {
            _mapName = mapName;
        }
        String _mapName;

        /// <summary>
        /// Read the current value from the memory mapped file.
        /// Note:  This function is thread-safe.
        /// </summary>
        /// <param name="value">Value read.</param>
        /// <returns>True if read successful, false if server is not running or timed-out waiting for server to write.</returns>
        public bool Read(out T value)
        {
            value = default(T);
            if (!TryConnect())
                return false;

            Int32 startTransactionNumber, endTransactionNumber;
            do
            {
                // What until data is not being updated.
                var timeout = DateTime.Now + new TimeSpan(0, 0, 5);
                while ((startTransactionNumber = _viewAccessor.ReadInt32(0)) == -1)
                {
                    if (DateTime.Now > timeout)
                        return false;
                    System.Threading.Thread.Yield();
                }
                // Zero indicates the server hasn't written anything yet.
                if (startTransactionNumber == 0)
                    return false;
                _viewAccessor.Read(sizeof(Int32), out value);
                endTransactionNumber = _viewAccessor.ReadInt32(0);
            // If the transaction number was changed while reading, the data is corrupt.  Try again.
            } while (startTransactionNumber != endTransactionNumber);

            return true;
        }

        static object _openingLock = new object();
        bool TryConnect()
        {
            lock (_openingLock)
            {
                if (_memoryMappedFile == null)
                {
                    try
                    {
                        _memoryMappedFile = MemoryMappedFile.OpenExisting(_mapName, MemoryMappedFileRights.Read);
                        _viewAccessor = _memoryMappedFile.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);
                    }
                    catch
                    {
                        Dispose();
                        return false;
                    }
                }
                return true;
            }
        }
    }
}
