using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMix.Models.Models
{
    
        public class UserLoginActivity
        {
            public int UserLoginActivityId { get; set; }
            public int? UserId{ get; set; }
            public string UserName { get; set; }
            public DateTime Timestamp { get; set; }
            public string IPAddress { get; set; }
            public string SessionID { get; set; }
            public int Status { get; set; }
            public string AdditionalInfo { get; set; }

        }

    }

