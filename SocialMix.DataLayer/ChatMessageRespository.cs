using Microsoft.Extensions.Configuration;
using SocialMix.DataLayer;
using SocialMix.Models.Models;
using System.Data.SqlClient;

public class ChatMessageRepository : BaseRepository
{
    public ChatMessageRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public void InsertChatMessage(ChatMessage chatMessage)
    {
        using (var connection = this.CreateConnection())
        {
            connection.Open();

            var query = @"
            INSERT INTO ChatMessage (UserId, UserName, Message, Timestamp)
            VALUES (@UserId, @UserName, @Message, @Timestamp)";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserId", chatMessage.UserId);
                command.Parameters.AddWithValue("@UserName", chatMessage.UserName);
                command.Parameters.AddWithValue("@Message", chatMessage.Message);
                command.Parameters.AddWithValue("@Timestamp", chatMessage.Timestamp);

                command.ExecuteNonQuery();
            }
        }
    }

}
