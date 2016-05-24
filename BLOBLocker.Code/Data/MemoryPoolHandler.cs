using BLOBLocker.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.Data
{
    public class MemoryPoolHandler
    {
        Account currentAccount;
        public class MemoryPoolProperties
        {
            public int BasicMemory { get; set; }
            public int AdditionalMemory { get; set; }
        }

        public MemoryPoolHandler(Account currentAccount)
        {
            this.currentAccount = currentAccount;
        }

        public MemoryPool SetupNew(MemoryPoolProperties props)
        {
            if (currentAccount.MemoryPool != null)
                throw new InvalidOperationException("memory pool already set up");

            MemoryPool memPool = new MemoryPool();
            memPool.Owner = currentAccount;
            memPool.BasicSpace = props.BasicMemory;
            memPool.AdditionalSpace = props.AdditionalMemory;
            return memPool;
        }

        /// <summary>
        /// Assigns the specified space to a pool.
        /// 
        /// 1: Successfully assigned.
        /// 2: Not enough memory left of this type.
        /// 3: No memory added because space to assign is 0.
        /// </summary>
        /// <param name="pool">The pool to add memory to.</param>
        /// <param name="space">The amount of space to assign.</param>
        /// <param name="isBasic">If the memory is basic or additional.</param>
        /// <returns>The result as an int.</returns>
        public int AssignMemoryToPool(Pool pool, int space, bool isBasic)
        {
            int memoryLeft;
            if (isBasic)
                GetSpareBasicMemory(out memoryLeft);
            else
                GetSpareAdditionalMemory(out memoryLeft);

            if (space > 0)
            {
                if (memoryLeft >= space)
                {
                    var assignBasicMemory = new AssignedMemory();
                    assignBasicMemory.MemoryPool = currentAccount.MemoryPool;
                    assignBasicMemory.Space = space;
                    assignBasicMemory.IsBasic = isBasic;
                    pool.AssignedMemory.Add(assignBasicMemory);
                    currentAccount.MemoryPool.AssignedMemory.Add(assignBasicMemory);
                    return 1;
                }
                else
                {
                    return 2;
                }
            }
            return 3;
        }

        public void GetSpareMemory(out int basicMemoryLeft, out int additionalMemoryLeft)
        {
            GetSpareBasicMemory(out basicMemoryLeft);
            GetSpareAdditionalMemory(out additionalMemoryLeft);
        }

        public void GetSpareBasicMemory(out int basicMemoryLeft)
        {
            basicMemoryLeft = currentAccount.MemoryPool.BasicSpace - currentAccount.MemoryPool.AssignedMemory
                                                           .Where(p => p.IsBasic && p.IsEnabled)
                                                           .Select(p => p.Space)
                                                           .Sum();
        }

        public void GetSpareAdditionalMemory(out int additionalMemoryLeft)
        {
            additionalMemoryLeft = currentAccount.MemoryPool.AdditionalSpace - currentAccount.MemoryPool.AssignedMemory
                                                               .Where(p => !p.IsBasic && p.IsEnabled)
                                                               .Select(p => p.Space)
                                                               .Sum();
        }
    }
}
