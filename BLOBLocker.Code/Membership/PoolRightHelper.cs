using BLOBLocker.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.Membership
{
    public class PoolRightHelper
    {
        public static bool IsPoolOwner(Account user, PoolShare share)
        {
            return share.Pool.Owner == user;
        }
        public static bool HashRight(PoolShare share, PoolRight right)
        {
            return HasRight(share, (int)right);
        }
        public static bool HasRight(PoolShare share, int right)
        {
            return HasRight(share.Rights, right);
        }
        public static bool HasRight(int rights, int right)
        {
            return ((rights & right) == right);
        }

        public static void AddRight(PoolShare share, PoolRight right)
        {
            AddRight(share, (int)right);
        }
        public static void AddRight(PoolShare share, int right)
        {
            share.Rights |= (int)right;
        }

        public static void RemoveRight(PoolShare share, PoolRight right)
        {
            if (HashRight(share, right))
                share.Rights = share.Rights ^ (int)right;
        }

        public static ICollection<PoolRightRepresentation> GetRights(PoolShare share)
        {
            return GetRights(share.Rights);
        }
        public static ICollection<PoolRightRepresentation> GetRights(int rights)
        {
            var poolRightRepresentation = new List<PoolRightRepresentation>();
            foreach (PoolRight right in Enum.GetValues(typeof(PoolRight)))
            {
                poolRightRepresentation.Add(new PoolRightRepresentation()
                {
                    Right = right,
                    IsChecked = HasRight(rights, (int)right)
                });
            }
            return poolRightRepresentation;
        }
        public static void SetRights(PoolShare share, ICollection<PoolRightRepresentation> rights)
        {
            share.Rights = 0;
            foreach(var right in rights)
            {
                if(right.IsChecked)
                    AddRight(share, right.Right);
            }
        }
    }
    public class PoolRightRepresentation
    {
        public PoolRight Right { get; set; }
        public bool IsChecked { get; set; }
    }
}
