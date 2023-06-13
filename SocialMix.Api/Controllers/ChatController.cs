using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SocialMix.BusinessLayer.Managers;
using SocialMix.Models.Models;

namespace SocialMix.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly ChatMessageManager _chatMessageManager;

        public ChatController(ChatMessageManager chatMessageManager)
        {
            _chatMessageManager = chatMessageManager;
        }

        [HttpPost("message")]
        public IActionResult Message([FromBody] ChatMessage message)
        {
            // Validate the message if needed

            // Save the message to the database using the ChatMessageManager
            _chatMessageManager.SaveMessage(message);

            // Broadcast the message to other chat participants using SignalR
          //  _hubContext.Clients.All.SendAsync("ReceiveMessage", message);

            return Ok();
        }
    }
}
