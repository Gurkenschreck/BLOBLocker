using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.Membership
{
    public enum PoolRight : int
    {
        InviteUser = 1 << 0,
        Removeuser = 1 << 1,
        ManageModules = 1 << 2,
        AssignRights = 1 << 3,
        ReadFileStorage = 1 << 4,
        UploadFileStorage = 1 << 5,
        ReadChat = 1 << 6,
        WriteChat = 1 << 7,
        ReadLinkRepository = 1 << 8,
        WriteLinkRepository = 1 << 9,
        RemoveAssignedMemory = 1 << 10,
        ChangeTitle = 1 << 11,
        ChangeDescription = 1 << 12,
        ChangeBanner = 1 << 13,
        ManageFileStorage = 1 << 14,
        ManageChat = 1 << 15,
        ViewPoolLog = 1 << 16,
        ViewParticipants = 1 << 17
    }
}
