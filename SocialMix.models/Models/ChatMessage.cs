using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMix.Models.Models
{
    public class ChatMessage
    {
        public int ChatMessageId { get; set; }
        public int PersonId { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
    }

}
