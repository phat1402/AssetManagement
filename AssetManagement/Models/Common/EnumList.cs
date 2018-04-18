using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssetManagement.Models.Common
{
    public class EnumList
    {
        public enum AssetStatus
        {
            Active = 1,
            Disposal = 2
        }

        public enum UserStatus
        {
            Active = 1,
            Blocked = 2
        }
    }
}