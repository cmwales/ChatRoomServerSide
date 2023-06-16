
using Microsoft.Extensions.Configuration;
using SocialMix.Models.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SocialMix.DataLayer
{

    public class UserLoginActivityRepository : BaseRepository
    {
        private string connectionString;

        public UserLoginActivityRepository(IConfiguration configuration) : base(configuration)
        {

        }

        public int CreateUserLoginActivity(UserLoginActivity userLoginActivity)
        {
            using (SqlConnection connection = this.CreateConnection())
            {
                connection.Open();

                string query = @"
                INSERT INTO UserLoginActivity (UserLoginActivityId, UserID, Timestamp, IPAddress, UserAgent, SessionID, Status, AdditionalInfo, UserName)
                VALUES (@UserLoginActivityId, @UserID, @Timestamp, @IPAddress, @UserAgent, @SessionID, @Status, @AdditionalInfo, @UserName);
                SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserLoginActivityId", userLoginActivity.UserLoginActivityId);
                    command.Parameters.AddWithValue("@UserID", userLoginActivity.UserId);
                    command.Parameters.AddWithValue("@UserName", userLoginActivity.UserName);
                    command.Parameters.AddWithValue("@Timestamp", userLoginActivity.Timestamp);
                    command.Parameters.AddWithValue("@IPAddress", userLoginActivity.IPAddress);
                    command.Parameters.AddWithValue("@SessionID", userLoginActivity.SessionID);
                    command.Parameters.AddWithValue("@Status", userLoginActivity.Status);
                    command.Parameters.AddWithValue("@AdditionalInfo", userLoginActivity.AdditionalInfo);

                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public UserLoginActivity GetUserLoginActivityById(int userLoginActivityId)
        {
            using (SqlConnection connection = this.CreateConnection())
            {
                connection.Open();

                string query = "SELECT * FROM UserLoginActivity WHERE UserLoginActivityId = @UserLoginActivityId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserLoginActivityId", userLoginActivityId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return GetUserLoginActivityFromDataReader(reader);
                        }
                    }
                }
            }

            return null;
        }

        public List<UserLoginActivity> GetAllUserLoginActivities()
        {
            List<UserLoginActivity> userLoginActivities = new List<UserLoginActivity>();

            using (SqlConnection connection = this.CreateConnection())
            {
                connection.Open();

                string query = "SELECT * FROM UserLoginActivity";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            UserLoginActivity userLoginActivity = GetUserLoginActivityFromDataReader(reader);
                            userLoginActivities.Add(userLoginActivity);
                        }
                    }
                }
            }

            return userLoginActivities;
        }



        private UserLoginActivity GetUserLoginActivityFromDataReader(SqlDataReader reader)
        {
            UserLoginActivity userLoginActivity = new UserLoginActivity();
            userLoginActivity.UserLoginActivityId = Convert.ToInt32(reader["UserLoginActivityId"]);
            userLoginActivity.UserId = Convert.ToInt32(reader["UserID"]);
            userLoginActivity.UserName = Convert.ToString(reader["UserName"]);
            userLoginActivity.Timestamp = Convert.ToDateTime(reader["Timestamp"]);
            userLoginActivity.IPAddress = Convert.ToString(reader["IPAddress"]);
            userLoginActivity.SessionID = Convert.ToString(reader["SessionID"]);
            userLoginActivity.Status = Convert.ToInt32(reader["Status"]);
            userLoginActivity.AdditionalInfo = Convert.ToString(reader["AdditionalInfo"]);

            return userLoginActivity;
        }

        public int GetFailedLoginRequestsCount(string userName)
        {

            int failedAttempts = 0;

            using (var connection = this.CreateConnection())
            {
                connection.Open();

                string query = @"SELECT COUNT(*) AS FailedAttempts 
                         FROM UserLoginActivity
                         WHERE Username = @username AND Status != 200
                         AND Timestamp > @timestamp";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", userName);
                    command.Parameters.AddWithValue("@timestamp", DateTime.Now.AddMinutes(-15));

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            failedAttempts = reader.GetInt32(0);
                        }
                    }
                }
            }

            return failedAttempts;
        }

        public bool IsAccountLocked(string username)
        {
            using (var connection = this.CreateConnection())
            {
                connection.Open();

                string query = @"SELECT COUNT(*) 
                         FROM UserLoginActivity
                         WHERE Username = @username 
                         AND Status = 900 
                         AND Timestamp > DATEADD(minute, -15, GETUTCDATE())";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", username);

                    int count = (int)command.ExecuteScalar();

                    return count > 0;
                }
            }
        }

    }

}