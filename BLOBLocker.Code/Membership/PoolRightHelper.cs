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

        public static ICollection<PoolRightViewModel> GetRights(PoolShare share)
        {
            return GetRights(share.Rights);
        }
        public static ICollection<PoolRightViewModel> GetRights(int rights)
        {
            var poolRightRepresentation = new List<PoolRightViewModel>();
            foreach (PoolRight right in Enum.GetValues(typeof(PoolRight)))
            {
                poolRightRepresentation.Add(new PoolRightViewModel()
                {
                    Right = right,
                    IsChecked = HasRight(rights, (int)right)
                });
            }
            return poolRightRepresentation;
        }
        public static void SetRights(PoolShare share, ICollection<PoolRightViewModel> rights)
        {
            share.Rights = 0;
            foreach(var right in rights)
            {
                if(right.IsChecked)
                    AddRight(share, right.Right);
            }
        }
        public static int CalculateRights(ICollection<PoolRightViewModel> rights)
        {
            int r = 0;
            foreach (var right in rights)
            {
                if(right.IsChecked)
                    r |= (int)right.Right;
            }
            return r;
        }
        public static int GetMaxRights()
        {
            int rights = 0;
            foreach (PoolRight right in Enum.GetValues(typeof(PoolRight)))
            {
                rights += (int)right;
            }
            return rights;
        }
    }
    public class PoolRightViewModel
    {
        public PoolRight Right { get; set; }
        public bool IsChecked { get; set; }
    }
}
