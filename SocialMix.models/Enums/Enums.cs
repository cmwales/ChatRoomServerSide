using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMix.Models.Enums
{
    public static  class Enums
    {
        public enum UserLoginStatus
        {
            Successful = 200,
            Failed = 403,
            AccountLocked = 900
        }
    }
}
