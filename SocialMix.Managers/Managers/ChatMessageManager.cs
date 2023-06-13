using SocialMix.DataLayer;
using System;
using System.Collections.Generic;
using System.Text;
using SocialMix.Models.Models;

namespace SocialMix.BusinessLayer.Managers
{
    public class ChatMessageManager
    {
        ChatMessageRepository chatMessageRepository;


        public ChatMessageManager(ChatMessageRepository chatMessageRepository)
        {
            this.chatMessageRepository = chatMessageRepository;
        }
        public void SaveMessage(ChatMessage message)
        {
            chatMessageRepository.InsertChatMessage(message);
        }
    }
}
